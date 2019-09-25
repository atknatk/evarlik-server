using System.Collections.Generic;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Lookup;
using EVarlik.Service.Lookup.Manager;

namespace EVarlik.Service.Lookup.Controller
{
    public class TransactionTypeController : VarlikController
    {
        private readonly TransactionTypeManager _transactionTypeManager;

        public TransactionTypeController()
        {
            _transactionTypeManager = new TransactionTypeManager();
        }

        [HttpGet]
        public VarlikResult<List<TransactionTypeDto>> GetAll()
        {
            return _transactionTypeManager.GetAll();
        }
    }
}