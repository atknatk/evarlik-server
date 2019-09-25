using System.Collections.Generic;
using System.Linq;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Dto.Commisions;
using LinqKit;

namespace EVarlik.Service.Commissions.BusinessLayer
{
    public class CommisionOperation
    {
        public VarlikResult<CommisionDto> GetCommission(string idCoinType, decimal transactionVolume)
        {
            var result = new VarlikResult<CommisionDto>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new CommisionDto().FromEntity().Expand();
                result.Data = ctx.Commission
                    .AsExpandable()
                    .Where(l => l.IdCoinType == idCoinType && l.TransactionVolume <= (decimal) transactionVolume)
                    .OrderByDescending(l => l.TransactionVolume)
                    .Select(fromEntity)
                    .OrderByDescending(l => l.TransactionVolume)
                    .FirstOrDefault();
                result.Success();
            }
            return result;
        }

        public VarlikResult<decimal> GetTransferCommissionByIdCoinType(string idCoinType)
        {
            var result = new VarlikResult<decimal>();

            using (var ctx = new VarlikContext())
            {
                result.Data = ctx.Commission
                    .Where(l => l.IdCoinType == idCoinType)
                    .Select(l => l.TransferFee)
                    .FirstOrDefault();
                result.Success();
            }

            return result;
        }

        public VarlikResult<List<CommisionDto>> GetAllCommission()
        {
            var result = new VarlikResult<List<CommisionDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new CommisionDto().FromEntity().Expand();
                result.Data = ctx.Commission
                    .AsExpandable()
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }

            return result;
        }
    }
}