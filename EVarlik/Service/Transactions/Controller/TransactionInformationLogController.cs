using System.Collections.Generic;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Transactions;
using EVarlik.Service.Transactions.Manager;

namespace EVarlik.Service.Transactions.Controller
{
    public class TransactionInformationLogController : VarlikController
    {
        private readonly TransactionInformationLogManager _transactionInformationLogManager;

        public TransactionInformationLogController()
        {
            _transactionInformationLogManager = new TransactionInformationLogManager();
        }

        /*    [HttpPost]
            public VarlikResult Save([FromBody]TransactionInformationLogDto transactionInformationLogDto)
            {
                return _transactionInformationLogManager.Save(transactionInformationLogDto);
            }*/

        [HttpGet]
        public VarlikResult<List<TransactionInformationLogDto>> GetAll(int limit, int offset)
        {
            return _transactionInformationLogManager.GetAll(limit, offset);
        }
    }
}