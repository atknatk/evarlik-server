using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Database.Entity.Transactions;
using EVarlik.Dto.Commisions;
using EVarlik.Dto.Transactions;
using EVarlik.Service.Commissions.Manager;
using EVarlik.Service.Wallets.Manager;
using log4net;
using LinqKit;

namespace EVarlik.Service.Transactions.BusinessLayer
{
    public class UserCoinTransactionOrderOperation
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VarlikResult MakeOrder(UserCoinTransactionOrderDto userCoinTransactionOrderDto, ref List<TransactinOrderListDto> push)
        {
            var result = HasBalance(userCoinTransactionOrderDto);
            if (!result.IsSuccess) return result;

            using (var ctx = new VarlikContext())
            using (var tx = ctx.Database.BeginTransaction())
            {
                //write main order
                var mainOrderResult = WriteMainOrder(ctx, tx, userCoinTransactionOrderDto.IdUser, userCoinTransactionOrderDto.CoinAmount,
                    userCoinTransactionOrderDto.IdCoinType, userCoinTransactionOrderDto.CoinUnitPrice,
                    userCoinTransactionOrderDto.IdTransactionType, TransactionStateEnum.Processing, userCoinTransactionOrderDto.MoneyAmount);

                if (!mainOrderResult.IsSuccess) return mainOrderResult;

                return MakeOrderDoIt(userCoinTransactionOrderDto, ctx, tx, mainOrderResult.ObjectId, ref push);
            }
        }

        private VarlikResult MakeOrderDoIt(UserCoinTransactionOrderDto userCoinTransactionOrderDto,
            VarlikContext ctx,
            DbContextTransaction tx,
            long requesterOrderId,
            ref List<TransactinOrderListDto> push)
        {
            var result = new VarlikResult();
            List<UserCoinTransactionOrder> orderList = null;
            if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.CoinPurchasing)
            {
                orderList = ctx.UserCoinTransactionOrder
                    .AsNoTracking()
                    .AsExpandable()
                    .Where(
                        l => l.IdTransactionType == TransactionTypeEnum.CoinSales
                             && l.CoinUnitPrice <= userCoinTransactionOrderDto.CoinUnitPrice
                             && l.IdCoinType == userCoinTransactionOrderDto.IdCoinType
                             && (l.IdTransactionState == TransactionStateEnum.Processing
                                 || l.IdTransactionState == TransactionStateEnum.PartialyCompleted)
                    )
                    .OrderBy(l => l.CoinUnitPrice)
                    .ThenBy(l => l.CreatedAt)
                    .ToList();

            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.CoinSales)
            {
                orderList = ctx.UserCoinTransactionOrder
                    .AsNoTracking()
                    .AsExpandable()
                    .Where(
                        l => l.IdTransactionType == TransactionTypeEnum.CoinPurchasing
                             && l.CoinUnitPrice >= userCoinTransactionOrderDto.CoinUnitPrice
                             && l.IdCoinType == userCoinTransactionOrderDto.IdCoinType
                             && (l.IdTransactionState == TransactionStateEnum.Processing
                                 || l.IdTransactionState == TransactionStateEnum.PartialyCompleted)
                    )
                    .OrderByDescending(l => l.CoinUnitPrice)
                    .ThenBy(l => l.CreatedAt)
                    .ToList();
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromBank
                || userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.ToBank)
            {
                return BankOrder(ctx, tx, userCoinTransactionOrderDto, requesterOrderId);
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromWallet
                     || userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.ToWallet)
            {
                return WalletOrder(ctx, tx, userCoinTransactionOrderDto, requesterOrderId);
            }
            else
            {
                result.Status = ResultStatus.UnsupportedOperation;
                return result;
            }


            //requester is a maker
            var txVolumeR = GetTransactionVolume(userCoinTransactionOrderDto.IdUser);
            if (!txVolumeR.IsSuccess)
            {
                return txVolumeR;
            }
            var commisionManager = new CommisionManager();
            var commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, txVolumeR.Data);

            if (!commisionR.IsSuccess)
            {
                return commisionR;
            }

            // UnSuccess rollback
            result = MakeOrderRecursive(orderList, userCoinTransactionOrderDto, ctx, tx, commisionR.Data, requesterOrderId, push);

            if (!result.IsSuccess)
            {
                tx.Rollback();
                return result;
            }

            try
            {
                tx.Commit();
                result.Success();
            }
            catch (Exception e)
            {
                result.Status = ResultStatus.UnknownError;
                Log.Error("MakeOrderDoIt", e);
            }
            return result;
        }

        private VarlikResult MakeOrderRecursive(List<UserCoinTransactionOrder> orderList, UserCoinTransactionOrderDto order,
            VarlikContext ctx,
            DbContextTransaction tx,
            CommisionDto commisionDto,
            long requesterOrderId,
            List<TransactinOrderListDto> push)
        {
            var result = new VarlikResult(); ;

            if (orderList.Count == 0)
            {
                result = WriteOrder(ctx, tx, order.IdUser, order.CoinAmount, order.IdCoinType, order.CoinUnitPrice,
                      order.IdTransactionType, TransactionStateEnum.Processing, 0, requesterOrderId, null, null);
                if (!result.IsSuccess)
                {
                    return result;
                }

                AddPushList(ref push, order.CoinUnitPrice, order.CoinAmount, order.CoinUnitPrice * order.CoinAmount, order.IdTransactionType);
                return result;
            }

            var item = orderList[0];

            decimal processCount = item.CoinAmount >= order.CoinAmount ? order.CoinAmount : item.CoinAmount;

            var coinCommision = commisionDto.MakerPercatange * processCount;
            // Purchasing
            result = WriteTransaction(ctx, tx, order.IdUser, processCount, order.IdCoinType, order.CoinUnitPrice,
                order.IdTransactionType, coinCommision, 0, requesterOrderId);
            if (!result.IsSuccess) return result;

            result = UpdateMainOrder(ctx, tx, requesterOrderId, TransactionStateEnum.PartialyCompleted, null);
            if (!result.IsSuccess) return result;

            // Sales
            var salesCoinCommision = commisionDto.TakerPercatange * processCount * order.CoinUnitPrice;
            result = WriteTransaction(ctx, tx, item.IdUser, processCount, order.IdCoinType, order.CoinUnitPrice,
                ViceVersa(order.IdTransactionType), 0, salesCoinCommision, item.IdMainOrderLog);
            if (!result.IsSuccess) return result;

            AddPushList(ref push, item.CoinUnitPrice, -processCount, item.CoinUnitPrice * processCount, ViceVersa(order.IdTransactionType));

            //delete order
            if (item.CoinAmount - processCount == 0)
            {
                result = DeleteOrder(ctx, tx, item.Id);
                if (!result.IsSuccess) return result;

                //update main order status
                result = UpdateMainOrder(ctx, tx, item.IdMainOrderLog, TransactionStateEnum.Completed, null);
            }
            else
            {
                var updateOrderResult = UpdateOrder(ctx, tx, item.Id, item.CoinAmount - processCount,
                    TransactionStateEnum.PartialyCompleted);
                if (!updateOrderResult.IsSuccess) return updateOrderResult;

                var updateMainOrderResult = UpdateMainOrder(ctx, tx, item.IdMainOrderLog, TransactionStateEnum.PartialyCompleted, null);
                if (!updateMainOrderResult.IsSuccess) return updateMainOrderResult;
            }

            // Us
            if (Math.Abs(order.CoinUnitPrice - item.CoinUnitPrice) > 0)
            {
                //TODO 1
                //result = WriteTransaction(ctx, tx, 1, Math.Abs(order.CoinUnitPrice - item.CoinUnitPrice) * processCount, order.IdCoinType,
                //    Math.Abs(order.CoinUnitPrice - item.CoinUnitPrice), order.IdTransactionType, 0, 0, 1);
                //if (!result.IsSuccess) return result;
            }

            orderList.RemoveAt(0);
            var remaining = order.CoinAmount - processCount;

            if (orderList.Count == 0 && remaining > 0)
            {
                result = WriteOrder(ctx, tx, order.IdUser, remaining, order.IdCoinType, order.CoinUnitPrice,
                      order.IdTransactionType, TransactionStateEnum.PartialyCompleted, 0, requesterOrderId, null, null);
                if (!result.IsSuccess) return result;

                result = UpdateMainOrder(ctx, tx, requesterOrderId, TransactionStateEnum.PartialyCompleted, null);
                if (!result.IsSuccess) return result;

                AddPushList(ref push, order.CoinUnitPrice, remaining, order.CoinUnitPrice * remaining, order.IdTransactionType);
                return result;
            }
            if (orderList.Count > 0 && (remaining > 0))
            {
                order.CoinAmount = remaining;
                return MakeOrderRecursive(orderList, order, ctx, tx, commisionDto, requesterOrderId, push);
            }
            if (remaining == 0)
            {
                result = UpdateMainOrder(ctx, tx, requesterOrderId, TransactionStateEnum.Completed, null);
                return result;
            }
            return result.Success();
        }


        private void AddPushList(ref List<TransactinOrderListDto> list,
            decimal coinUnitPrice,
            decimal coinAmount,
            decimal total,
            string idTransactionType)
        {
            list.Add(new TransactinOrderListDto()
            {
                CoinAmount = coinAmount,
                IdTransactionType = idTransactionType,
                CoinUnitPrice = coinUnitPrice,
                Total = total
            });
        }

        private VarlikResult WriteOrder(VarlikContext ctx,
            DbContextTransaction tx,
            long idUser,
            decimal coinAmout,
            string coinType,
            decimal coinUnitPrice,
            string transactionType,
            string transactionState,
            decimal moneyAmount,
            long idMainOrder,
            string fromAddress,
            string toAddress)
        {
            // TODO Releted User Id 

            var result = new VarlikResult();
            ctx.UserCoinTransactionOrder.Add(new UserCoinTransactionOrder()
            {

                IdUser = idUser,
                CoinAmount = coinAmout,
                IdCoinType = coinType,
                CoinUnitPrice = coinUnitPrice,
                IdTransactionType = transactionType,
                IdTransactionState = transactionState,
                MoneyAmount = moneyAmount,
                IdMainOrderLog = idMainOrder,
                FromWalletAddress = fromAddress,
                ToWalletAddress = toAddress
            });
            try
            {
                ctx.SaveChanges();
                return result.Success();
            }
            catch (Exception e)
            {
                Log.Error("WriteOrder", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }
        }

        private VarlikResult UpdateOrder(VarlikContext ctx,
            DbContextTransaction tx,
            long idOrder,
            decimal coinAmout,
            string transactionState)
        {
            var result = new VarlikResult();

            var order = ctx.UserCoinTransactionOrder.FirstOrDefault(l => l.Id == idOrder);
            if (order != null)
            {
                order.CoinAmount = coinAmout;
                order.IdTransactionState = transactionState;
            }
            try
            {
                ctx.SaveChanges();
                return result.Success();
            }
            catch (Exception e)
            {
                Log.Error("WriteOrder", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }
        }

        private VarlikResult DeleteOrder(VarlikContext ctx,
            DbContextTransaction tx,
            long idOrder)
        {
            var result = new VarlikResult();

            var order = ctx.UserCoinTransactionOrder.FirstOrDefault(l => l.Id == idOrder);
            if (order != null)
            {
                ctx.UserCoinTransactionOrder.Remove(order);
            }
            try
            {
                ctx.SaveChanges();
                return result.Success();
            }
            catch (Exception e)
            {
                Log.Error("DeleteOrder", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }
        }

        private VarlikResult WriteMainOrder(VarlikContext ctx,
            DbContextTransaction tx,
            long idUser,
            decimal coinAmout,
            string coinType,
            decimal coinUnitPrice,
            string transactionType,
            string transactionState,
            decimal moneyAmount)
        {
            // TODO Releted User Id 

            var result = new VarlikResult();
            var added = ctx.MainOrderLog.Add(new MainOrderLog()
            {

                IdUser = idUser,
                CoinAmount = coinAmout,
                IdCoinType = coinType,
                CoinUnitPrice = coinUnitPrice,
                IdTransactionType = transactionType,
                IdTransactionState = transactionState,
                MoneyAmount = moneyAmount
            });
            try
            {
                ctx.SaveChanges();
                result.ObjectId = added.Id;
                return result.Success();
            }
            catch (Exception e)
            {
                Log.Error("WriteOrder", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }

        }

        private VarlikResult UpdateMainOrder(VarlikContext ctx,
            DbContextTransaction tx,
            long idMainOrder,
            string transactionState,
            string pin)
        {
            // TODO Releted User Id 

            var result = new VarlikResult();
            var mainOrder = ctx.MainOrderLog.FirstOrDefault(l => l.Id == idMainOrder);
            if (mainOrder == null)
            {
                result.Status = ResultStatus.NoSuchObject;
                return result;
            }
            mainOrder.IdTransactionState = transactionState;
            mainOrder.UserCoinTransactionOrderGuid = pin;
            try
            {
                ctx.SaveChanges();
                return result.Success();
            }
            catch (Exception e)
            {
                Log.Error("WriteOrder", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }
        }

        private VarlikResult WriteTransaction(VarlikContext ctx,
            DbContextTransaction tx,
            long idUser,
            decimal coinAmout,
            string coinType,
            decimal coinUnitPrice,
            string transactionType,
            decimal coinCommision,
            decimal moneyCommision,
            long idMainOrder)
        {
            // TODO Releted User Id 

            var result = new VarlikResult();
            ctx.UserCoinTransactionLog.Add(new UserTransactionLog()
            {
                IdUser = idUser,
                CoinAmount = coinAmout,
                IdCoinType = coinType,
                CoinUnitPrice = coinUnitPrice,
                IsSucces = true,
                IdTransactionType = transactionType,
                CommissionMoney = moneyCommision,
                CommissionCoinCount = coinCommision,
                IdMainOrderLog = idMainOrder
            });
            try
            {
                ctx.SaveChanges();
                return result.Success();
            }
            catch (Exception e)
            {
                Log.Error("WriteTransaction", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }

        }

        private VarlikResult BankOrder(VarlikContext ctx,
            DbContextTransaction tx,
            UserCoinTransactionOrderDto userCoinTransactionOrderDto,
            long idMainOrder)
        {
            var result = new VarlikResult();
            Random rnd = new Random();
            int pin = rnd.Next(1000, 9999);

            userCoinTransactionOrderDto.UserCoinTransactionOrderGuid = pin.ToString();

            result = WriteOrder(ctx, tx, userCoinTransactionOrderDto.IdUser,
                0, null, 0, userCoinTransactionOrderDto.IdTransactionType, TransactionStateEnum.Processing,
                userCoinTransactionOrderDto.MoneyAmount, idMainOrder, null, null);

            if (!result.IsSuccess) return result;

            if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromBank)
            {
                result = UpdateMainOrder(ctx, tx, idMainOrder, TransactionStateEnum.Processing, pin.ToString());
                if (!result.IsSuccess) return result;
            }

            try
            {
                tx.Commit();
                result.Success();
            }
            catch (Exception e)
            {
                Log.Error("BankOrder", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }

            result.ObjectId = pin;
            result.Success();
            return result;
        }

        private VarlikResult WalletOrder(VarlikContext ctx,
            DbContextTransaction tx,
            UserCoinTransactionOrderDto userCoinTransactionOrderDto,
            long idMainOrder)
        {
            if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromWallet)
            {
                //is exists
                var exists = ctx.UserCoinTransactionOrder.Any(l =>
                    l.IdUser == userCoinTransactionOrderDto.IdUser &&
                    l.IdTransactionState == TransactionStateEnum.Processing &&
                    l.IdTransactionType == TransactionTypeEnum.FromWallet);
                if (exists)
                {
                    var preResult = new VarlikResult();
                    preResult.Status = ResultStatus.AlreadyAdded;
                    return preResult;
                }
            }


            var result = WriteOrder(ctx, tx, userCoinTransactionOrderDto.IdUser,
                userCoinTransactionOrderDto.CoinAmount,
                userCoinTransactionOrderDto.IdCoinType,
                userCoinTransactionOrderDto.CoinUnitPrice,
                userCoinTransactionOrderDto.IdTransactionType,
                TransactionStateEnum.Processing,
                0,
                idMainOrder,
                userCoinTransactionOrderDto.FromWalletAddress,
                userCoinTransactionOrderDto.ToWalletAddress);

            if (!result.IsSuccess) return result;

            try
            {
                tx.Commit();
                result.Success();
            }
            catch (Exception e)
            {
                Log.Error("WalletOrder", e);
                result.Status = ResultStatus.TransactionRollback;
                return result;
            }

            return result;
        }

        private string ViceVersa(string idTransactionType)
        {
            return idTransactionType == TransactionTypeEnum.CoinSales
                ? TransactionTypeEnum.CoinPurchasing
                : TransactionTypeEnum.CoinSales;
        }

        public VarlikResult Save(UserCoinTransactionOrderDto userCoinTransactionOrderDto, ref List<TransactinOrderListDto> transactinOrderListDtoList)
        {
            transactinOrderListDtoList = new List<TransactinOrderListDto>();


            var result = new VarlikResult();
            using (var ctx = new VarlikContext())
            {
                using (var tx = ctx.Database.BeginTransaction())
                {
                    var entity = userCoinTransactionOrderDto.ToEntity(userCoinTransactionOrderDto);
                    var persistent = ctx.UserCoinTransactionOrder.Add(entity);
                    persistent.IdTransactionState = TransactionStateEnum.Processing;
                    persistent.UserCoinTransactionOrderGuid = Guid.NewGuid().ToString();
                    try
                    {
                        ctx.SaveChanges();
                        // gelen alış emri ise satış emirlerini kontrol et
                        if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.CoinPurchasing)
                        {
                            var bill = userCoinTransactionOrderDto.CoinAmount * userCoinTransactionOrderDto.CoinUnitPrice;

                            var balanceR = GetMoneyBalanceOfUser(userCoinTransactionOrderDto.IdUser);
                            if (!balanceR.IsSuccess)
                            {
                                return result;
                            }

                            if (bill > balanceR.Data)
                            {
                                result.Status = ResultStatus.NotSufficentBalance;
                                return result;
                            }

                            var salesList = ctx.UserCoinTransactionOrder
                                .AsExpandable()
                                .Where(
                                    l => l.IdTransactionType == TransactionTypeEnum.CoinSales
                                         && l.CoinUnitPrice <= userCoinTransactionOrderDto.CoinUnitPrice
                                         && l.IdCoinType == userCoinTransactionOrderDto.IdCoinType
                                         && (l.IdTransactionState == TransactionStateEnum.Processing
                                         || l.IdTransactionState == TransactionStateEnum.PartialyCompleted)
                                )
                                .OrderBy(l => l.CoinUnitPrice)
                                .ThenBy(l => l.CreatedAt)
                                .ToList();

                            foreach (var salesRow in salesList)
                            {
                                //responser
                                var txVolumeR = GetTransactionVolume(salesRow.IdUser);
                                if (!txVolumeR.IsSuccess)
                                {
                                    return txVolumeR;
                                }
                                var commisionManager = new CommisionManager();
                                var commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, txVolumeR.Data);

                                if (!commisionR.IsSuccess)
                                {
                                    return commisionR;
                                }

                                //this responser is taker

                                if (salesRow.CoinAmount >= userCoinTransactionOrderDto.CoinAmount)
                                {
                                    TransactinOrderListDto transactinOrderListDto = new TransactinOrderListDto();
                                    transactinOrderListDto.CoinAmount = -1;
                                    transactinOrderListDto.CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice;
                                    transactinOrderListDtoList.Add(transactinOrderListDto);

                                    var ourCommissionFromRequester = userCoinTransactionOrderDto.CoinAmount
                                        * commisionR.Data.TakerPercatange
                                        * salesRow.CoinUnitPrice;

                                    salesRow.CoinAmount = salesRow.CoinAmount - userCoinTransactionOrderDto.CoinAmount;

                                    UserTransactionLog userCoinTransactionLog =
                                        new UserTransactionLog
                                        {
                                            IdUser = userCoinTransactionOrderDto.IdUser,
                                            CoinAmount = userCoinTransactionOrderDto.CoinAmount,
                                            IdCoinType = salesRow.IdCoinType,
                                            CoinUnitPrice = salesRow.CoinUnitPrice,
                                            IsSucces = true,
                                            IdTransactionType = TransactionTypeEnum.CoinPurchasing
                                        };

                                    var requesterLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                    userCoinTransactionLog = new UserTransactionLog
                                    {
                                        IdUser = salesRow.IdUser,
                                        CoinAmount = userCoinTransactionOrderDto.CoinAmount,
                                        IdCoinType = salesRow.IdCoinType,
                                        CoinUnitPrice = salesRow.CoinUnitPrice,
                                        IsSucces = true,
                                        IdTransactionType = TransactionTypeEnum.CoinSales
                                    };

                                    var responserLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                    responserLog.CommissionMoney = ourCommissionFromRequester;

                                    if (salesRow.CoinAmount == 0)
                                    {
                                        ctx.UserCoinTransactionOrder.Remove(salesRow);
                                    }

                                    ctx.UserCoinTransactionOrder.Remove(persistent);

                                    //Commission
                                    //requester

                                    txVolumeR = GetTransactionVolume(userCoinTransactionOrderDto.IdUser);
                                    if (!txVolumeR.IsSuccess)
                                    {
                                        return txVolumeR;
                                    }

                                    commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, txVolumeR.Data);

                                    if (!commisionR.IsSuccess)
                                    {
                                        return commisionR;
                                    }

                                    //this requester is a maker
                                    ourCommissionFromRequester = userCoinTransactionOrderDto.CoinAmount * commisionR.Data.MakerPercatange;
                                    requesterLog.CommissionCoinCount = ourCommissionFromRequester;

                                    break;
                                }
                                else
                                {
                                    persistent.CoinAmount = persistent.CoinAmount - salesRow.CoinAmount;
                                    persistent.IdTransactionState = TransactionStateEnum.PartialyCompleted;

                                    var ourCommissionFromRequester = salesRow.CoinAmount *
                                        commisionR.Data.TakerPercatange *
                                        salesRow.CoinUnitPrice;


                                    UserTransactionLog userCoinTransactionLog =
                                        new UserTransactionLog
                                        {
                                            IdUser = userCoinTransactionOrderDto.IdUser,
                                            CoinAmount = salesRow.CoinAmount,
                                            IdCoinType = salesRow.IdCoinType,
                                            CoinUnitPrice = salesRow.CoinUnitPrice,
                                            IsSucces = true,
                                            IdTransactionType = TransactionTypeEnum.CoinPurchasing
                                        };

                                    var requesterLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                    userCoinTransactionLog = new UserTransactionLog
                                    {
                                        IdUser = salesRow.IdUser,
                                        CoinAmount = salesRow.CoinAmount,
                                        IdCoinType = salesRow.IdCoinType,
                                        CoinUnitPrice = salesRow.CoinUnitPrice,
                                        IsSucces = true,
                                        IdTransactionType = TransactionTypeEnum.CoinSales
                                    };

                                    userCoinTransactionOrderDto.CoinAmount = userCoinTransactionOrderDto.CoinAmount - salesRow.CoinAmount;

                                    var responserLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);
                                    responserLog.CommissionMoney = ourCommissionFromRequester;

                                    //Commission
                                    //requester

                                    txVolumeR = GetTransactionVolume(userCoinTransactionOrderDto.IdUser);
                                    if (!txVolumeR.IsSuccess)
                                    {
                                        return txVolumeR;
                                    }

                                    commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, txVolumeR.Data);

                                    if (!commisionR.IsSuccess)
                                    {
                                        return commisionR;
                                    }
                                    TransactinOrderListDto transactinOrderListDto = new TransactinOrderListDto();

                                    transactinOrderListDto.CoinAmount = userCoinTransactionOrderDto.CoinAmount;
                                    transactinOrderListDto.CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice;
                                    transactinOrderListDto.IdTransactionType = TransactionTypeEnum.CoinPurchasing;
                                    transactinOrderListDto.Total =
                                        transactinOrderListDto.CoinAmount * transactinOrderListDto.CoinUnitPrice;
                                    transactinOrderListDto.Price = salesRow.CoinUnitPrice;

                                    transactinOrderListDtoList.Add(transactinOrderListDto);

                                    //this requester is a maker
                                    ourCommissionFromRequester = salesRow.CoinAmount * commisionR.Data.MakerPercatange;
                                    requesterLog.CommissionCoinCount = ourCommissionFromRequester;

                                    ctx.UserCoinTransactionOrder.Remove(salesRow);
                                }
                            }

                        }// alış emrirlerini kontrol et
                        else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.CoinSales)
                        {
                            var coinBalanceR = GetCoinBalanceOfUser(userCoinTransactionOrderDto.IdUser, userCoinTransactionOrderDto.IdCoinType);
                            if (!coinBalanceR.IsSuccess)
                            {
                                return result;
                            }

                            if (userCoinTransactionOrderDto.CoinAmount > coinBalanceR.Data)
                            {
                                result.Status = ResultStatus.NotSufficentBalance;
                                return result;
                            }

                            var purchasingList = ctx.UserCoinTransactionOrder
                                .AsExpandable()
                                .Where(
                                    l => l.IdTransactionType == TransactionTypeEnum.CoinPurchasing
                                         && l.CoinUnitPrice >= userCoinTransactionOrderDto.CoinUnitPrice
                                         && l.IdCoinType == userCoinTransactionOrderDto.IdCoinType
                                         && (l.IdTransactionState == TransactionStateEnum.Processing
                                             || l.IdTransactionState == TransactionStateEnum.PartialyCompleted)
                                )
                                .OrderByDescending(l => l.CoinUnitPrice)
                                .ThenBy(l => l.CreatedAt)
                                .ToList();

                            //sales

                            foreach (var purchaseRow in purchasingList)
                            {
                                //responser
                                var txVolumeR = GetTransactionVolume(purchaseRow.IdUser);
                                if (!txVolumeR.IsSuccess)
                                {
                                    return txVolumeR;
                                }
                                var commisionManager = new CommisionManager();
                                var commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, txVolumeR.Data);

                                if (!commisionR.IsSuccess)
                                {
                                    return commisionR;
                                }

                                //this responser is taker

                                if (purchaseRow.CoinAmount >= userCoinTransactionOrderDto.CoinAmount)
                                {

                                    TransactinOrderListDto transactinOrderListDto = new TransactinOrderListDto();
                                    transactinOrderListDto.CoinAmount = -1;
                                    transactinOrderListDto.CoinUnitPrice = purchaseRow.CoinUnitPrice;
                                    transactinOrderListDtoList.Add(transactinOrderListDto);

                                    purchaseRow.CoinAmount = purchaseRow.CoinAmount - userCoinTransactionOrderDto.CoinAmount;

                                    UserTransactionLog userCoinTransactionLog =
                                        new UserTransactionLog
                                        {
                                            IdUser = purchaseRow.IdUser,
                                            CoinAmount = userCoinTransactionOrderDto.CoinAmount,
                                            IdCoinType = purchaseRow.IdCoinType,
                                            CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice,
                                            IsSucces = true,
                                            IdTransactionType = TransactionTypeEnum.CoinPurchasing
                                        };

                                    var responserLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                    //Commission
                                    //responser

                                    txVolumeR = GetTransactionVolume(purchaseRow.IdUser);
                                    if (!txVolumeR.IsSuccess)
                                    {
                                        return txVolumeR;
                                    }

                                    commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, txVolumeR.Data);

                                    if (!commisionR.IsSuccess)
                                    {
                                        return commisionR;
                                    }

                                    //this responser is a taker
                                    var ourCommissionFromResponser = userCoinTransactionOrderDto.CoinAmount * commisionR.Data.TakerPercatange;
                                    responserLog.CommissionCoinCount = ourCommissionFromResponser;

                                    userCoinTransactionLog = new UserTransactionLog
                                    {
                                        IdUser = userCoinTransactionOrderDto.IdUser,
                                        CoinAmount = userCoinTransactionOrderDto.CoinAmount,
                                        IdCoinType = purchaseRow.IdCoinType,
                                        CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice,
                                        IsSucces = true,
                                        IdTransactionType = TransactionTypeEnum.CoinSales
                                    };

                                    var requesterLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                    var ourCommissionFromRequester = userCoinTransactionOrderDto.CoinAmount
                                                                     * commisionR.Data.MakerPercatange
                                                                     * purchaseRow.CoinUnitPrice;

                                    requesterLog.CommissionMoney = ourCommissionFromRequester;

                                    if (purchaseRow.CoinAmount == 0)
                                    {
                                        ctx.UserCoinTransactionOrder.Remove(purchaseRow);
                                    }

                                    purchaseRow.IdTransactionState = TransactionStateEnum.PartialyCompleted;
                                    ctx.UserCoinTransactionOrder.Remove(persistent);

                                    break;
                                }
                                else
                                {
                                    persistent.CoinAmount = persistent.CoinAmount - purchaseRow.CoinAmount;
                                    persistent.IdTransactionState = TransactionStateEnum.PartialyCompleted;

                                    UserTransactionLog userCoinTransactionLog =
                                        new UserTransactionLog
                                        {
                                            IdUser = purchaseRow.IdUser,
                                            CoinAmount = purchaseRow.CoinAmount,
                                            IdCoinType = purchaseRow.IdCoinType,
                                            CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice,
                                            IsSucces = true,
                                            IdTransactionType = TransactionTypeEnum.CoinPurchasing
                                        };

                                    var responserLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                    //Commission
                                    //responser

                                    txVolumeR = GetTransactionVolume(purchaseRow.IdUser);
                                    if (!txVolumeR.IsSuccess)
                                    {
                                        return txVolumeR;
                                    }

                                    commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, txVolumeR.Data);

                                    if (!commisionR.IsSuccess)
                                    {
                                        return commisionR;
                                    }

                                    //this responser is a taker
                                    var ourCommissionFromResponser = purchaseRow.CoinAmount * commisionR.Data.TakerPercatange;
                                    responserLog.CommissionCoinCount = ourCommissionFromResponser;

                                    userCoinTransactionLog = new UserTransactionLog
                                    {
                                        IdUser = userCoinTransactionOrderDto.IdUser,
                                        CoinAmount = purchaseRow.CoinAmount,
                                        IdCoinType = purchaseRow.IdCoinType,
                                        CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice,
                                        IsSucces = true,
                                        IdTransactionType = TransactionTypeEnum.CoinSales
                                    };

                                    userCoinTransactionOrderDto.CoinAmount = userCoinTransactionOrderDto.CoinAmount - purchaseRow.CoinAmount;


                                    var requesterLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                    var ourCommissionFromRequester = purchaseRow.CoinAmount
                                                                     * commisionR.Data.MakerPercatange
                                                                     * purchaseRow.CoinUnitPrice;

                                    requesterLog.CommissionMoney = ourCommissionFromRequester;

                                    TransactinOrderListDto transactinOrderListDto = new TransactinOrderListDto();

                                    transactinOrderListDto.CoinAmount = userCoinTransactionOrderDto.CoinAmount;
                                    transactinOrderListDto.CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice;
                                    transactinOrderListDto.IdTransactionType = TransactionTypeEnum.CoinSales;
                                    transactinOrderListDto.Total =
                                        transactinOrderListDto.CoinAmount * transactinOrderListDto.CoinUnitPrice;
                                    transactinOrderListDto.Price = purchaseRow.CoinUnitPrice;

                                    transactinOrderListDtoList.Add(transactinOrderListDto);

                                    ctx.UserCoinTransactionOrder.Remove(purchaseRow);
                                }
                            }
                        }
                        // Bank
                        else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromBank)
                        {
                            Random rnd = new Random();
                            int pin = rnd.Next(1000, 9999);
                            persistent.UserCoinTransactionOrderGuid = pin + "";
                            persistent.IdTransactionState = TransactionStateEnum.Processing;
                            result.ObjectId = pin;
                            persistent.IdCoinType = null;
                        }
                        else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.ToBank)
                        {
                            persistent.IdTransactionState = TransactionStateEnum.Processing;
                        }
                        //wallet
                        else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.ToWallet)
                        {
                            //var coinBalanceR = GetCoinBalanceOfUser(userCoinTransactionOrderDto.IdUser,
                            //    userCoinTransactionOrderDto.IdCoinType);

                            //if (!coinBalanceR.IsSuccess)
                            //{
                            //    return coinBalanceR;
                            //}

                            //if (userCoinTransactionOrderDto.CoinAmount > coinBalanceR.Data)
                            //{
                            //    result.Status = ResultStatus.NotSufficentBalance;
                            //    return result;
                            //}
                            bool willBeWaitingApproval = false;
                            ////do it
                            //if (userCoinTransactionOrderDto.IdCoinType == CoinTypeEnum.DogeCoin)
                            //{

                            //    // pending approval
                            //    if (userCoinTransactionOrderDto.IdCoinType == CoinTypeEnum.DogeCoin)
                            //    {
                            //        if (userCoinTransactionOrderDto.CoinAmount > 200000)
                            //        {
                            //            persistent.IdTransactionState = TransactionStateEnum.PendingApproval;
                            //            willBeWaitingApproval = true;
                            //        }
                            //    }
                            //}

                            if (!willBeWaitingApproval)
                            {
                                var blockioManager = new BlockioWalletManager();

                                var commisionManager = new CommisionManager();
                                var commisionR = commisionManager.GetCommission(userCoinTransactionOrderDto.IdCoinType, 1);

                                if (!commisionR.IsSuccess)
                                {
                                    return commisionR;
                                }

                                //transaction log

                                var userCoinTransactionLog = new UserTransactionLog
                                {
                                    IdUser = userCoinTransactionOrderDto.IdUser,
                                    CoinAmount = userCoinTransactionOrderDto.CoinAmount,
                                    IdCoinType = userCoinTransactionOrderDto.IdCoinType,
                                    IsSucces = true,
                                    IdTransactionType = TransactionTypeEnum.ToWallet,
                                    FromHash = userCoinTransactionOrderDto.FromWalletAddress,
                                    ToHash = userCoinTransactionOrderDto.ToWalletAddress,
                                    IdTransactionState = TransactionStateEnum.Processing
                                };

                                var txLog = ctx.UserCoinTransactionLog.Add(userCoinTransactionLog);

                                var sendableCoinAmount = userCoinTransactionOrderDto.CoinAmount - commisionR.Data.TransferFeeCoinCount;

                                var withdrawR = blockioManager.Withdraw(userCoinTransactionOrderDto.IdCoinType,
                                    userCoinTransactionOrderDto.FromWalletAddress,
                                    userCoinTransactionOrderDto.ToWalletAddress,
                                    sendableCoinAmount,
                                    userCoinTransactionOrderDto.Pin);

                                if (withdrawR.IsSuccess)
                                {
                                    ctx.UserCoinTransactionOrder.Remove(persistent);
                                    txLog.TxId = withdrawR.Data;
                                    txLog.IdTransactionState = TransactionStateEnum.Completed;
                                }
                                else
                                {
                                    txLog.IdTransactionState = TransactionStateEnum.Failed;
                                }
                            }
                        }
                        ctx.SaveChanges();
                        tx.Commit();
                        result.Success();
                        //pin için kapatıldı
                        // result.ObjectId = persistent.Id;
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                    }
                }
            }
            return result;
        }

        private VarlikResult HasBalance(UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            VarlikResult result = new VarlikResult();
            if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.CoinPurchasing)
            {
                var bill = userCoinTransactionOrderDto.CoinAmount * userCoinTransactionOrderDto.CoinUnitPrice;
                var balanceR = GetMoneyBalanceOfUser(userCoinTransactionOrderDto.IdUser);
                if (!balanceR.IsSuccess)
                {
                    return result;
                }

                if (bill > balanceR.Data)
                {
                    result.Status = ResultStatus.NotSufficentBalance;
                    return result;
                }
                return result.Success();
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.CoinSales)
            {
                var coinBalanceR = GetCoinBalanceOfUser(userCoinTransactionOrderDto.IdUser, userCoinTransactionOrderDto.IdCoinType);
                if (!coinBalanceR.IsSuccess)
                {
                    return result;
                }
                //var diff = userCoinTransactionOrderDto.CoinAmount - coinBalanceR.Data;
                if (userCoinTransactionOrderDto.CoinAmount > coinBalanceR.Data)
                {
                    result.Status = ResultStatus.NotSufficentBalance;
                    return result;
                }
                return result.Success();
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromBank)
            {
                return result.Success();
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.ToBank)
            {
                var balanceR = GetMoneyBalanceOfUser(userCoinTransactionOrderDto.IdUser);
                if (!balanceR.IsSuccess)
                {
                    return result;
                }

                if (userCoinTransactionOrderDto.MoneyAmount > balanceR.Data)
                {
                    result.Status = ResultStatus.NotSufficentBalance;
                    return result;
                }
                return result.Success();
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromWallet)
            {
                return result.Success();
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.ToWallet)
            {
                var coinBalanceR = GetCoinBalanceOfUser(userCoinTransactionOrderDto.IdUser, userCoinTransactionOrderDto.IdCoinType);
                if (!coinBalanceR.IsSuccess)
                {
                    return result;
                }

                if (userCoinTransactionOrderDto.CoinAmount > coinBalanceR.Data)
                {
                    result.Status = ResultStatus.NotSufficentBalance;
                    return result;
                }
                return result.Success();
            }

            return result;
        }

        public VarlikResult Update(UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            var result = new VarlikResult();
            using (var ctx = new VarlikContext())
            {
                var entity = ctx.UserCoinTransactionOrder.FirstOrDefault(l => l.Id == userCoinTransactionOrderDto.Id);
                if (entity == null)
                {
                    result.Status = ResultStatus.NoSuchObject;
                    return result;
                }

                entity.CoinAmount = userCoinTransactionOrderDto.CoinAmount;

                try
                {
                    ctx.SaveChanges();
                    result.Success();
                }
                catch (Exception e)
                {
                    Log.Error("Update", e);
                }
            }
            return result;
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetAll(long idUser,
            int limit,
            int offset,
            string idCoinType,
            string idTransactionType)
        {
            var result = new VarlikResult<List<UserCoinTransactionOrderDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserCoinTransactionOrderDto().FromEntity().Expand();
                result.Data = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser && l.IdCoinType == idCoinType && l.IdTransactionType == idTransactionType)
                    .OrderBy(l => l.Id)
                    .Take(limit)
                    .Skip(offset)
                    .Select(fromEntity)
                    .ToList();

                result.Success();
            }
            return result;
        }

        public VarlikResult<decimal> GetMoneyBalanceOfUser(long idUser)
        {
            var result = new VarlikResult<decimal>();

            using (var ctx = new VarlikContext())
            {
                //Transaction

                var coinPurchasingExpanse = ctx.UserCoinTransactionLog
                      .AsExpandable()
                      .Where(l => l.IdUser == idUser &&
                                  l.IdTransactionType == TransactionTypeEnum.CoinPurchasing
                                  && l.IsSucces
                      ).Select(a =>

                            a.CoinAmount * a.CoinUnitPrice
                      )
                       .DefaultIfEmpty(0)
                       .Sum();

                var coinSalesIncome = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.CoinSales
                                && l.IsSucces
                    )
                    .Select(a =>

                        a.CoinAmount * a.CoinUnitPrice
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var coinSalesCommission = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.CoinSales
                                && l.IsSucces
                    )
                    .Select(a =>

                        a.CommissionMoney
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                // Bankaya Giden withdraw
                var toBank = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.ToBank
                                && l.IsSucces
                    )
                    .Select(a =>
                        a.MoneyAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var fromBank = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.FromBank
                                && l.IsSucces
                    )
                    .Select(a =>
                        a.MoneyAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                // Order

                var coinPurchasingOrderExpanse = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.CoinPurchasing &&
                                l.IdTransactionState != TransactionStateEnum.CancelledByUser
                    )
                    .Select(a =>

                        a.CoinAmount * a.CoinUnitPrice
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var toBankOrder = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.ToBank &&
                                l.IdTransactionState != TransactionStateEnum.CancelledByUser
                    )
                    .Select(a =>

                        a.MoneyAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();


                var balance = (coinSalesIncome + fromBank)
                    - (coinPurchasingExpanse + toBank)
                    - (coinPurchasingOrderExpanse + toBankOrder)
                    - (coinSalesCommission);

                result.Data = balance;
                result.Success();
            }
            return result;
        }

        public VarlikResult<decimal> GetCoinBalanceOfUser(long idUser, string idCoinType)
        {
            var result = new VarlikResult<decimal>();

            using (var ctx = new VarlikContext())
            {
                //Transaction

                var coinPurchasingCount = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.CoinPurchasing
                                && l.IdCoinType == idCoinType
                                && l.IsSucces
                    ).Select(a =>

                        a.CoinAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var coinPurchasingCommission = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.CoinPurchasing
                                && l.IdCoinType == idCoinType
                                && l.IsSucces
                    ).Select(a =>

                        a.CommissionCoinCount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var coinFromWalletCount = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.FromWallet
                                && l.IdCoinType == idCoinType
                                && l.IsSucces
                    ).Select(a =>
                        a.CoinAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var coinSalesCount = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.CoinSales
                                && l.IdCoinType == idCoinType
                                && l.IsSucces
                    ).Select(a =>

                        a.CoinAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var coinToWalletCount = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.ToWallet
                                && l.IdCoinType == idCoinType
                                && l.IsSucces
                    ).Select(a =>
                        a.CoinAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                // Order
                var coinSalesOrderExpanse = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.CoinSales
                                && l.IdCoinType == idCoinType &&
                                l.IdTransactionState != TransactionStateEnum.CancelledByUser
                    )
                    .Select(a =>

                        a.CoinAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var toWalletOrder = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                l.IdTransactionType == TransactionTypeEnum.ToWallet
                                && l.IdCoinType == idCoinType &&
                                l.IdTransactionState != TransactionStateEnum.CancelledByUser
                    )
                    .Select(a =>

                        a.CoinAmount
                    )
                    .DefaultIfEmpty(0)
                    .Sum();

                var balance = (coinPurchasingCount + coinFromWalletCount)
                    - (coinSalesCount + coinToWalletCount)
                    - (coinSalesOrderExpanse + toWalletOrder)
                    - coinPurchasingCommission;

                result.Data = balance;
                result.Success();
            }
            return result;
        }

        public VarlikResult<decimal> GetTransactionVolume(long idUser)
        {
            var result = new VarlikResult<decimal>();

            using (var ctx = new VarlikContext())
            {
                DateTime minDate = DateTime.Now.AddDays(-30);

                result.Data = ctx.UserCoinTransactionLog
                        .AsExpandable()
                        .Where(l => l.IdUser == idUser
                        && ((l.IdTransactionType == TransactionTypeEnum.CoinSales) || (l.IdTransactionType == TransactionTypeEnum.CoinPurchasing))
                        && l.IsSucces
                        && l.RequestedDate >= minDate
                        ).Select(a =>

                            a.CoinAmount * a.CoinUnitPrice
                        )
                        .DefaultIfEmpty(0)
                        .Sum();

                result.Success();
            }

            return result;
        }

        public VarlikResult<List<TransactinOrderListDto>> GetTransactionOrderList(string transactionType, string coinType, int limit)
        {
            var result = new VarlikResult<List<TransactinOrderListDto>>();

            using (var ctx = new VarlikContext())
            {
                var query = ctx.UserCoinTransactionOrder
                    .AsExpandable()

                    .Where(l => l.IdCoinType == coinType
                                && l.IdTransactionType == transactionType
                                && (l.IdTransactionState == TransactionStateEnum.Processing ||
                                l.IdTransactionState == TransactionStateEnum.PartialyCompleted))
                    .GroupBy(l => new
                    {
                        l.CoinUnitPrice,
                    }).Select(l => new TransactinOrderListDto()
                    {
                        CoinAmount = l.Sum(c => c.CoinAmount),
                        CoinUnitPrice = l.Key.CoinUnitPrice,
                        Total = l.Sum(c => c.CoinAmount) * l.Key.CoinUnitPrice

                    });

                if (transactionType == TransactionTypeEnum.CoinSales)
                {
                    query = query.OrderBy(l => l.CoinUnitPrice);
                }
                else
                {
                    query = query.OrderByDescending(l => l.CoinUnitPrice);
                }

                result.Data = query.Take(limit)
                    .Skip(0)
                    .ToList();

                result.Success();
            }

            return result;
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetProcessingBankOrders()
        {
            var result = new VarlikResult<List<UserCoinTransactionOrderDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserCoinTransactionOrderDto().FromEntity().Expand();
                result.Data = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdTransactionState == TransactionStateEnum.Processing
                                && (l.IdTransactionType == TransactionTypeEnum.FromBank ||
                                    l.IdTransactionType == TransactionTypeEnum.ToBank))
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }
            return result;
        }

        public VarlikResult<List<TransactinOrderListDto>> GetRealTransactionOrderList(string transactionType, string coinType)
        {
            var result = new VarlikResult<List<TransactinOrderListDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new TransactinOrderListDto().FromEntity().Expand();
                var query = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdCoinType == coinType
                                && l.IdTransactionType == transactionType
                                && l.IdUser != 1);

                if (transactionType == TransactionTypeEnum.CoinSales)
                {
                    query = query.OrderBy(l => l.CoinUnitPrice);
                }
                else
                {
                    query = query.OrderByDescending(l => l.CoinUnitPrice);
                }

                result.Data = query
                    .Select(fromEntity)
                    .ToList();

                result.Success();
            }

            return result;
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetProcessingWalletOrders()
        {
            var result = new VarlikResult<List<UserCoinTransactionOrderDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserCoinTransactionOrderDto().FromEntity().Expand();
                result.Data = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdTransactionState == TransactionStateEnum.Processing
                                && (l.IdTransactionType == TransactionTypeEnum.ToWallet ||
                                    l.IdTransactionType == TransactionTypeEnum.FromWallet))
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }
            return result;
        }

        public VarlikResult<List<BalanceDto>> GetAllCoinBalanceOfUser(long idUser)
        {
            var result = new VarlikResult<List<BalanceDto>>();
            result.Data = new List<BalanceDto>();

            var coinBalanceR = GetCoinBalanceOfUser(idUser, CoinTypeEnum.DogeCoin);
            if (coinBalanceR.IsSuccess)
            {
                BalanceDto balanceDto = new BalanceDto();
                balanceDto.IdCoinType = CoinTypeEnum.DogeCoin;
                balanceDto.Balance = coinBalanceR.Data;
                result.Data.Add(balanceDto);
            }

            coinBalanceR = GetCoinBalanceOfUser(idUser, CoinTypeEnum.Btc);
            if (coinBalanceR.IsSuccess)
            {
                BalanceDto balanceDto = new BalanceDto();
                balanceDto.IdCoinType = CoinTypeEnum.Btc;
                balanceDto.Balance = coinBalanceR.Data;
                result.Data.Add(balanceDto);
            }

            coinBalanceR = GetCoinBalanceOfUser(idUser, CoinTypeEnum.Iota);
            if (coinBalanceR.IsSuccess)
            {
                BalanceDto balanceDto = new BalanceDto();
                balanceDto.IdCoinType = CoinTypeEnum.Iota;
                balanceDto.Balance = coinBalanceR.Data;
                result.Data.Add(balanceDto);
            }

            coinBalanceR = GetCoinBalanceOfUser(idUser, CoinTypeEnum.Ripple);
            if (coinBalanceR.IsSuccess)
            {
                BalanceDto balanceDto = new BalanceDto();
                balanceDto.IdCoinType = CoinTypeEnum.Ripple;
                balanceDto.Balance = coinBalanceR.Data;
                result.Data.Add(balanceDto);
            }

            coinBalanceR = GetCoinBalanceOfUser(idUser, CoinTypeEnum.Eth);
            if (coinBalanceR.IsSuccess)
            {
                BalanceDto balanceDto = new BalanceDto();
                balanceDto.IdCoinType = CoinTypeEnum.Eth;
                balanceDto.Balance = coinBalanceR.Data;
                result.Data.Add(balanceDto);
            }

            coinBalanceR = GetCoinBalanceOfUser(idUser, CoinTypeEnum.LiteCoin);
            if (coinBalanceR.IsSuccess)
            {
                BalanceDto balanceDto = new BalanceDto();
                balanceDto.IdCoinType = CoinTypeEnum.LiteCoin;
                balanceDto.Balance = coinBalanceR.Data;
                result.Data.Add(balanceDto);
            }

            result.Success();

            return result;
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllMainOrder(long idUser,
            string idTransactionType,
            string idCoinType,
            string idTransactionState)
        {
            var result = new VarlikResult<List<MainOrderLogDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new MainOrderLogDto().FromEntity().Expand();
                result.Data = ctx.MainOrderLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser && l.IdCoinType == idCoinType &&
                                l.IdTransactionType == idTransactionType && l.IdTransactionState == idTransactionState)
                    .Select(fromEntity)
                    .ToList();

                result.Success();
            }

            return result;
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetAllTransactionAndOrderList(long idUser, string idCoinType)
        {
            var result = new VarlikResult<List<UserCoinTransactionOrderDto>>();

            using (var ctx = new VarlikContext())
            {
                var transactionList = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser && l.IdCoinType == idCoinType)
                    .ToList();

                result.Data = new List<UserCoinTransactionOrderDto>();

                foreach (var item in transactionList)
                {
                    UserCoinTransactionOrderDto userCoinTransactionOrderDto = new UserCoinTransactionOrderDto();
                    userCoinTransactionOrderDto.IdTransactionType = item.IdTransactionType;
                    userCoinTransactionOrderDto.IdTransactionState = item.IdTransactionState;
                    userCoinTransactionOrderDto.CreatedAt = item.TransactionDate;
                    userCoinTransactionOrderDto.CoinAmount = item.CoinAmount;
                    userCoinTransactionOrderDto.CoinUnitPrice = item.CoinUnitPrice;
                    result.Data.Add(userCoinTransactionOrderDto);
                }

                var fromEntity = new UserCoinTransactionOrderDto().FromEntity().Expand();

                var orderList = ctx.UserCoinTransactionOrder
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser && l.IdCoinType == idCoinType)
                    .Select(fromEntity)
                    .ToList();

                result.Data.AddRange(orderList);

                result.Success();
            }

            return result;
        }

    }
}