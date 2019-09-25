using System.Collections.Generic;
using EVarlik.Common.Model;
using EVarlik.Dto.Lookup;
using EVarlik.Service.Lookup.BusinessLayer;

namespace EVarlik.Service.Lookup.Manager
{
    public class TransactionTypeManager
    {
        private readonly TransactionTypeOperation _transactionTypeOperation;

        public TransactionTypeManager()
        {
            _transactionTypeOperation = new TransactionTypeOperation();
        }

        public VarlikResult<List<TransactionTypeDto>> GetAll()
        {
            return _transactionTypeOperation.GetAll();
        }
    }
}