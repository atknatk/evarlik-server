using System.Collections.Generic;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Lookup;
using EVarlik.Service.Lookup.Manager;

namespace EVarlik.Service.Lookup.Controller
{
    public class TransactionStateController : VarlikController
    {
        private readonly TransactionStateManager _transactionStateManager;

        public TransactionStateController()
        {
            _transactionStateManager = new TransactionStateManager();
        }

        [HttpGet]
        public VarlikResult<List<TransactionStateDto>> GetAll()
        {
            return _transactionStateManager.GetAll();
        }
    }
}