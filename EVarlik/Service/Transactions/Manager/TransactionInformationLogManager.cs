using System.Collections.Generic;
using EVarlik.Common.Model;
using EVarlik.Dto.Transactions;
using EVarlik.Service.Transactions.BusinessLayer;

namespace EVarlik.Service.Transactions.Manager
{
    public class TransactionInformationLogManager
    {
        private readonly TransactionInformationLogOperation _transactionInformationLogOperation;

        public TransactionInformationLogManager()
        {
            _transactionInformationLogOperation = new TransactionInformationLogOperation();
        }

        public VarlikResult Save(TransactionInformationLogDto transactionInformationLogDto)
        {
            return _transactionInformationLogOperation.Save(transactionInformationLogDto);
        }

        public VarlikResult<List<TransactionInformationLogDto>> GetAll( int limit, int offset)
        {
            var idUser = 0;
            return _transactionInformationLogOperation.GetAll(idUser, limit, offset);
        }
    }
}