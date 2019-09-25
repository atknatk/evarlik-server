using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Database.Entity.Transactions;
using EVarlik.Dto.Transactions;
using EVarlik.Service.Transactions.Manager;

namespace EVarlik.Service.Transactions.Controller
{
    public class MainOrderLogController : VarlikController
    {
        private readonly MainOrderLogManager _mainOrderLogManager;

        public MainOrderLogController()
        {
            _mainOrderLogManager = new MainOrderLogManager();
        }

        [HttpGet]
        [Route("api/MainOrderLog/CoinOrder")]
        public VarlikResult<List<MainOrderLogDto>> GetAllMainOrderByIdCoinType(string idCoinType)
        {
            return _mainOrderLogManager.GetAllMainOrderByIdCoinType(idCoinType);
        }

        [HttpGet]
        [Route("api/MainOrderLog/BankOrder")]
        public VarlikResult<List<MainOrderLogDto>> GetAllBankOrder()
        {
            return _mainOrderLogManager.GetAllBankOrder();
        }

        [HttpGet]
        [Route("api/MainOrderLog/WalletOrder")]
        public VarlikResult<List<MainOrderLogDto>> GetAllWalletOrder(string idCoinType)
        {
            return _mainOrderLogManager.GetAllWalletOrder(idCoinType);
        }

        [HttpDelete]
        [Route("api/MainOrderLog/{idMainOrder}")]
        public VarlikResult CancelTheOrder(long idMainOrder)
        {
            return _mainOrderLogManager.CancelTheOrder(idMainOrder);
        }

        [HttpGet]
        [Route("api/MainOrderLog/AllOrder")]
        public VarlikResult<List<MainOrderLogDto>> GetAllOrder()
        {
            return _mainOrderLogManager.GetAllOrder();
        }

        [HttpGet]
        [Route("api/MainOrderLog/AllOrderAdmin")]
        public VarlikResult<List<MainOrderLogAdminDto>> GetAllOrderAdmin([FromUri]MainOrderLogDto dto)
        {
            int limit = 1000;
            int offset = 0;
            return _mainOrderLogManager.GetAllOrderAdmin(dto, limit, offset);
        }

        [HttpPost]
        [Route("api/MainOrderLog/ApproveFromBankAdmin")]
        public VarlikResult ApproveFromBankAdmin(long idMainOrder,string idTransactionState)
        {
            return _mainOrderLogManager.ApproveFromBankAdmin(idMainOrder, idTransactionState);
        }

        [HttpPost]
        [Route("api/MainOrderLog/ApproveToBankAdmin")]
        public VarlikResult ApproveToBankAdmin(long idMainOrder)
        {
            return _mainOrderLogManager.ApproveToBankAdmin(idMainOrder);
        }

        [HttpGet]
        [Route("api/MainOrderLog/AllRealCoinOrderAdmin")]
        public VarlikResult<List<MainOrderLogDto>> GetAllRealCoinOrderAdmin()
        {
            int limit = 1000;
            int offset = 0;
            return _mainOrderLogManager.GetAllRealCoinOrderAdmin(limit, offset);
        }
    }
}