using System.Collections.Generic;
using EVarlik.Common.Model;
using EVarlik.Dto.Commisions;
using EVarlik.Service.Commissions.BusinessLayer;

namespace EVarlik.Service.Commissions.Manager
{
    public class CommisionManager
    {
        private readonly CommisionOperation _commisionOperation;

        public CommisionManager()
        {
            _commisionOperation = new CommisionOperation();
        }

        public VarlikResult<CommisionDto> GetCommission(string idCoinType, decimal transactionVolume)
        {
            return _commisionOperation.GetCommission(idCoinType, transactionVolume);
        }

        public VarlikResult<List<CommisionDto>> GetAllCommission()
        {
            return _commisionOperation.GetAllCommission();
        }
    }
}