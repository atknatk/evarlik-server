using System.Collections.Generic;
using EVarlik.Common.Model;
using EVarlik.Dto.Lookup;
using EVarlik.Service.Lookup.BusinessLayer;

namespace EVarlik.Service.Lookup.Manager
{
    public class TransactionStateManager
    {
        private readonly TransactionStateOperation _transactionStateOperation;

        public TransactionStateManager()
        {
            _transactionStateOperation = new TransactionStateOperation();
        }

        public VarlikResult<List<TransactionStateDto>> GetAll()
        {
            return _transactionStateOperation.GetAll();
        }
    }
}