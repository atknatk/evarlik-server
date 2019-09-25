using System;
using System.Collections.Generic;
using System.Linq;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Dto.Transactions;
using EVarlik.Hubs;
using EVarlik.Identity;
using EVarlik.Queues;
using EVarlik.Service.Transactions.BusinessLayer;
using EVarlik.Service.Users.Manager;

namespace EVarlik.Service.Transactions.Manager
{
    public class UserCoinTransactionOrderManager
    {
        private readonly UserCoinTransactionOrderOperation _userCoinTransactionOrderOperation;

        public UserCoinTransactionOrderManager()
        {
            _userCoinTransactionOrderOperation = new UserCoinTransactionOrderOperation();
        }

        public VarlikResult Save(UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            if (userCoinTransactionOrderDto == null)
            {
                var result = new VarlikResult();
                result.Status = ResultStatus.MissingRequiredParamater;
                return result;
            }

            userCoinTransactionOrderDto.CoinUnitPrice = Math.Abs(userCoinTransactionOrderDto.CoinUnitPrice);
            userCoinTransactionOrderDto.CoinAmount = Math.Abs(userCoinTransactionOrderDto.CoinAmount);

            var transactionManager = new UserCoinTransactionLogManager();

            //get current max price of coin
            if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.CoinSales)
            {
                var maxPriceR = transactionManager.GetMaxPriceOfCoin(userCoinTransactionOrderDto.IdCoinType);
                if (maxPriceR.IsSuccess)
                {
                    if (maxPriceR.Data > 0)
                    {
                        var limit = maxPriceR.Data * 20;
                        if (userCoinTransactionOrderDto.CoinUnitPrice > limit)
                        {
                            var result = new VarlikResult();
                            result.Status = ResultStatus.AboveTheLimit;
                            return result;
                        }
                    }
                }
            }
            var idUser = IdentityHelper.Instance.CurrentUserId;
            userCoinTransactionOrderDto.IdUser = idUser;

            // does user have this address
            if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.FromWallet)
            {
                var userManager = new UserManager();
                var hasAddressR = userManager.DoesUserHaveThisAddress(idUser, userCoinTransactionOrderDto.IdCoinType, userCoinTransactionOrderDto.ToWalletAddress);
                if (!hasAddressR.IsSuccess)
                {
                    return hasAddressR;
                }
                if (!hasAddressR.Data)
                {
                    var result = new VarlikResult();
                    result.Status = ResultStatus.UserDoesNotHaveThisAddress;
                    return result;
                }
            }
            else if (userCoinTransactionOrderDto.IdTransactionType == TransactionTypeEnum.ToWallet)
            {
                var userManager = new UserManager();
                var hasAddressR = userManager.DoesUserHaveThisAddress(idUser, userCoinTransactionOrderDto.IdCoinType, userCoinTransactionOrderDto.FromWalletAddress);
                if (!hasAddressR.IsSuccess || !hasAddressR.Data)
                {
                    var result = new VarlikResult();
                    result.Status = ResultStatus.UserDoesNotHaveThisAddress;
                    return result;
                }
            }

            var push = new List<TransactinOrderListDto>();
            var saveResult = _userCoinTransactionOrderOperation.MakeOrder(userCoinTransactionOrderDto, ref push);

            // push to order list
            if (saveResult.IsSuccess)
            {
                OrderPublisher orderPublisher = new OrderPublisher();
                foreach (var transactinOrderListDto in push)
                {
                    orderPublisher.PublishOrder(userCoinTransactionOrderDto.IdCoinType, transactinOrderListDto);
                }
            }

            //push to price list
            transactionManager.PushPrice(userCoinTransactionOrderDto.IdCoinType);

            return saveResult;
        }

        public VarlikResult Update(UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            var updateResult = _userCoinTransactionOrderOperation.Update(userCoinTransactionOrderDto);
            return updateResult;
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetAll(int limit, int offset,
            string idCoinType,
            string idTransactionType)
        {
            var idUser = IdentityHelper.Instance.CurrentUserId;
            return _userCoinTransactionOrderOperation.GetAll(idUser, limit, offset, idCoinType, idTransactionType);
        }

        public VarlikResult<List<TransactinOrderListDto>> GetTransactionOrderList(string transactionType,
            string coinType,
             int limit)
        {
            if (limit > 50)
            {
                limit = 50;
            }

            return _userCoinTransactionOrderOperation.GetTransactionOrderList(transactionType, coinType, limit);
        }

        public VarlikResult<decimal> GetCoinBalanceOfUser(string idCoinType)
        {
            var idUser = IdentityHelper.Instance.CurrentUserId;
            return _userCoinTransactionOrderOperation.GetCoinBalanceOfUser(idUser, idCoinType);
        }

        public VarlikResult<List<BalanceDto>> GetAllCoinBalanceOfUser()
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _userCoinTransactionOrderOperation.GetAllCoinBalanceOfUser(idUser);
        }

        public VarlikResult<decimal> GetMoneyBalanceOfUser()
        {
            var idUser = IdentityHelper.Instance.CurrentUserId;
            return _userCoinTransactionOrderOperation.GetMoneyBalanceOfUser(idUser);
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllMainOrder(string idTransactionType,
            string idCoinType,
            string idTransactionState)
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _userCoinTransactionOrderOperation.GetAllMainOrder(idUser, idTransactionType, idCoinType, idTransactionState);
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetAllTransactionAndOrderList(string idCoinType)
        {
            var idUser = IdentityHelper.Instance.CurrentUserId;
            return _userCoinTransactionOrderOperation.GetAllTransactionAndOrderList(idUser, idCoinType);
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetProcessingBankOrders()
        {
            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId == 1 || userId == 2 || userId == 3)
            {
                return _userCoinTransactionOrderOperation.GetProcessingBankOrders();
            }
            return new VarlikResult<List<UserCoinTransactionOrderDto>>();
        }

        public VarlikResult<List<TransactinOrderListDto>> GetRealTransactionOrderList(string transactionType,
            string coinType)
        {
            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId != 1)
            {
                return new VarlikResult<List<TransactinOrderListDto>>();
            }
            return _userCoinTransactionOrderOperation.GetRealTransactionOrderList(transactionType, coinType);
        }

        public VarlikResult<List<UserCoinTransactionOrderDto>> GetProcessingWalletOrders()
        {
            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId == 1 || userId == 2 || userId == 3)
            {
                return _userCoinTransactionOrderOperation.GetProcessingWalletOrders();
            }
            return new VarlikResult<List<UserCoinTransactionOrderDto>>();
        }
    }
}