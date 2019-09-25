using System.Collections.Generic;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Transactions;
using EVarlik.Service.Transactions.Manager;

namespace EVarlik.Service.Transactions.Controller
{
    public class UserCoinTransactionOrderController : VarlikController
    {
        private readonly UserCoinTransactionOrderManager _userCoinTransactionOrderManager;

        public UserCoinTransactionOrderController()
        {
            _userCoinTransactionOrderManager = new UserCoinTransactionOrderManager();
        }

        [HttpPost]
        public VarlikResult Save([FromBody]UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            return _userCoinTransactionOrderManager.Save(userCoinTransactionOrderDto);
        }

        [HttpGet]
        public VarlikResult<List<UserCoinTransactionOrderDto>> GetAll(int limit, int offset, string idCoinType,
        string idTransactionType)
        {
            return _userCoinTransactionOrderManager.GetAll(limit, offset, idCoinType, idTransactionType);
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/AllTransactionAndOrder")]
        public VarlikResult<List<UserCoinTransactionOrderDto>> GetAllTransactionAndOrderList(string idCoinType)
        {
            return _userCoinTransactionOrderManager.GetAllTransactionAndOrderList(idCoinType);
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/List")]
        public VarlikResult<List<TransactinOrderListDto>> GetTransactionOrderList(string transactionType,
            string coinType,
            int limit)
        {
            return _userCoinTransactionOrderManager.GetTransactionOrderList(transactionType, coinType, limit);
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/CoinBalance")]
        public VarlikResult<decimal> GetCoinBalanceOfUser(string idCoinType)
        {
            return _userCoinTransactionOrderManager.GetCoinBalanceOfUser(idCoinType);
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/AllCoinBalance")]
        public VarlikResult<List<BalanceDto>> GetAllCoinBalanceOfUser()
        {
            return _userCoinTransactionOrderManager.GetAllCoinBalanceOfUser();
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/MoneyBalance")]
        public VarlikResult<decimal> GetMoneyBalanceOfUser()
        {
            return _userCoinTransactionOrderManager.GetMoneyBalanceOfUser();
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/ProcessingBankOrder")]
        public VarlikResult<List<UserCoinTransactionOrderDto>> GetProcessingBankOrders()
        {
            return _userCoinTransactionOrderManager.GetProcessingBankOrders();
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/RealList")]
        public VarlikResult<List<TransactinOrderListDto>> GetRealTransactionOrderList(string transactionType,
            string coinType)
        {
            return _userCoinTransactionOrderManager.GetRealTransactionOrderList(transactionType, coinType);
        }

        [HttpGet]
        [Route("api/UserCoinTransactionOrder/ProcessingWalletOrder")]
        public VarlikResult<List<UserCoinTransactionOrderDto>> GetProcessingWalletOrders()
        {
            return _userCoinTransactionOrderManager.GetProcessingWalletOrders();
        }
    }
}