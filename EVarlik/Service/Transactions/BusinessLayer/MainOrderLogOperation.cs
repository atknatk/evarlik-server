using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Database.Entity.Transactions;
using EVarlik.Dto.Transactions;
using EVarlik.Hubs;
using log4net;
using LinqKit;

namespace EVarlik.Service.Transactions.BusinessLayer
{
    public class MainOrderLogOperation
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VarlikResult Save(MainOrderLogDto mainOrderLogDto)
        {
            var result = new VarlikResult();
            using (var ctx = new VarlikContext())
            {
                var entity = mainOrderLogDto.ToEntity(mainOrderLogDto);
                ctx.MainOrderLog.Add(entity);
                try
                {
                    ctx.SaveChanges();
                    result.Success();
                    result.ObjectId = entity.Id;
                }
                catch (Exception e)
                {
                    Log.Error("Save", e);
                }
            }
            return result;
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllMainOrderByIdCoinType(long idUser, string idCoinType)
        {
            var result = new VarlikResult<List<MainOrderLogDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new MainOrderLogDto().FromEntity().Expand();
                try
                {
                    result.Data = ctx.MainOrderLog
                        .AsExpandable()
                        .Where(l => l.IdUser == idUser && l.IdCoinType == idCoinType &&
                                    (l.IdTransactionType == TransactionTypeEnum.CoinPurchasing ||
                                     l.IdTransactionType == TransactionTypeEnum.CoinSales))
                        .Select(fromEntity)
                        .ToList();
                }
                catch (Exception e)
                {
                }
                result.Success();
            }
            return result;
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllBankOrder(long idUser)
        {
            var result = new VarlikResult<List<MainOrderLogDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new MainOrderLogDto().FromEntity().Expand();
                result.Data = ctx.MainOrderLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                (l.IdTransactionType == TransactionTypeEnum.FromBank ||
                                 l.IdTransactionType == TransactionTypeEnum.ToBank))
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }
            return result;
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllWalletOrder(long idUser, string idCoinType)
        {
            var result = new VarlikResult<List<MainOrderLogDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new MainOrderLogDto().FromEntity().Expand();
                result.Data = ctx.MainOrderLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser &&
                                (l.IdTransactionType == TransactionTypeEnum.FromWallet ||
                                 l.IdTransactionType == TransactionTypeEnum.ToWallet) &&
                                l.IdCoinType == idCoinType)
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }
            return result;
        }

        public VarlikResult CancelTheOrder(long idMainOrder, long idUser)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var mainOrder = ctx.MainOrderLog
                    .FirstOrDefault(l => l.Id == idMainOrder && l.IdUser == idUser);
                if (mainOrder == null)
                {
                    result.Status = ResultStatus.NoSuchObject;
                    return result;
                }

                if (mainOrder.IdTransactionState != TransactionStateEnum.Processing)
                {
                    result.Status = ResultStatus.CannotBeCancelled;
                    return result;
                }

                //sub orders
                var subOrders = ctx.UserCoinTransactionOrder.Where(l => l.IdMainOrderLog == mainOrder.Id).ToList();
                foreach (var item in subOrders)
                {
                    item.IdTransactionState = TransactionStateEnum.CancelledByUser;
                }

                mainOrder.IdTransactionState = TransactionStateEnum.CancelledByUser;

                try
                {
                    ctx.SaveChanges();
                    result.Success();

                    //push 
                    OrderPublisher orderPublisher = new OrderPublisher();
                    foreach (var item in subOrders)
                    {
                        orderPublisher.PublishOrder(item.IdCoinType, new TransactinOrderListDto()
                        {
                            CoinAmount = -1 * item.CoinAmount,
                            CoinUnitPrice = item.CoinUnitPrice,
                            IdTransactionType = item.IdTransactionType,
                            Total = -1 * item.CoinAmount * item.CoinUnitPrice
                        });
                    }
                }
                catch (Exception e)
                {
                }
            }

            return result;
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllOrder(long idUser)
        {
            var result = new VarlikResult<List<MainOrderLogDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new MainOrderLogDto().FromEntity().Expand();
                result.Data = ctx.MainOrderLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser)
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }
            return result;
        }

        public VarlikResult<List<MainOrderLogAdminDto>> GetAllOrderAdmin(Expression<Func<MainOrderLog, bool>> predicate,
        int limit, int offset)
        {
            var result = new VarlikResult<List<MainOrderLogAdminDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new MainOrderLogAdminDto().FromEntity().Expand();
                result.Data = ctx.MainOrderLog
                    .AsExpandable()
                    .Where(predicate)
                    .OrderByDescending(l => l.CreatedAt)
                    .Take(limit)
                    .Skip(offset)
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }
            return result;
        }

        public VarlikResult ApproveFromBankAdmin(long idMainOrder, string idTransactionState)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var data = ctx.MainOrderLog
                    .FirstOrDefault(l => l.Id == idMainOrder
                                && l.IdTransactionState != TransactionStateEnum.Completed
                                && l.IdTransactionType == TransactionTypeEnum.FromBank);

                if (idTransactionState == TransactionStateEnum.Completed)
                {
                    if (data != null)
                    {
                        data.IdTransactionState = TransactionStateEnum.Completed;
                        data.TransactionDate = DateTime.Now;

                        var order = ctx.UserCoinTransactionOrder
                            .FirstOrDefault(l => l.IdMainOrderLog == idMainOrder
                                                 && l.IdTransactionState != TransactionStateEnum.Completed
                                                 && l.IdTransactionType == TransactionTypeEnum.FromBank);

                        if (order != null)
                        {
                            ctx.UserCoinTransactionOrder.Remove(order);
                        }
                        //write tx log

                        var tx = new UserTransactionLog()
                        {
                            MoneyAmount = data.MoneyAmount,
                            IdTransactionState = TransactionStateEnum.Completed,
                            IdTransactionType = TransactionTypeEnum.FromBank,
                            TransactionDate = data.TransactionDate.Value,
                            IsSucces = true,
                            IdUser = data.IdUser,
                            IdMainOrderLog = idMainOrder
                        };

                        ctx.UserCoinTransactionLog.Add(tx);
                        try
                        {
                            ctx.SaveChanges();
                            result.Success();
                        }
                        catch (Exception e)
                        {
                            Log.Error("ApproveFromBankAdmin", e);
                        }
                    }
                }
            }

            return result;
        }

        //COMMISSION ????
        public VarlikResult ApproveToBankAdmin(long idMainOrder)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var data = ctx.MainOrderLog
                    .FirstOrDefault(l => l.Id == idMainOrder
                                && l.IdTransactionState != TransactionStateEnum.Completed
                                && l.IdTransactionType == TransactionTypeEnum.ToBank);

                if (data != null)
                {
                    data.IdTransactionState = TransactionStateEnum.Completed;
                    data.TransactionDate = DateTime.Now;

                    var order = ctx.UserCoinTransactionOrder
                        .FirstOrDefault(l => l.IdMainOrderLog == idMainOrder
                                             && l.IdTransactionState != TransactionStateEnum.Completed
                                             && l.IdTransactionType == TransactionTypeEnum.ToBank);

                    if (order != null)
                    {
                        ctx.UserCoinTransactionOrder.Remove(order);
                    }
                    //write tx log

                    var tx = new UserTransactionLog()
                    {
                        MoneyAmount = data.MoneyAmount,
                        IdTransactionState = TransactionStateEnum.Completed,
                        IdTransactionType = TransactionTypeEnum.ToBank,
                        TransactionDate = data.TransactionDate.Value,
                        IsSucces = true,
                        IdUser = data.IdUser
                    };

                    ctx.UserCoinTransactionLog.Add(tx);
                    try
                    {
                        ctx.SaveChanges();
                        result.Success();
                    }
                    catch (Exception e)
                    {
                        Log.Error("ApproveToBankAdmin", e);
                    }
                }
            }

            return result;
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllRealCoinOrderAdmin(int limit, int offset)
        {
            var result = new VarlikResult<List<MainOrderLogDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new MainOrderLogDto().FromEntityForAdmin().Expand();
                result.Data = ctx.MainOrderLog
                    .AsExpandable()
                    .Where(l => l.IdUser > 1000
                    && (l.IdTransactionType == TransactionTypeEnum.CoinSales || l.IdTransactionType == TransactionTypeEnum.CoinPurchasing)
                    && l.IdTransactionState != TransactionStateEnum.Completed)
                    .OrderByDescending(l => l.CreatedAt)
                    .Take(limit)
                    .Skip(offset)
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }
            return result;
        }

    }
}