using System.Collections.Generic;
using System.Linq;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Dto.Lookup;
using LinqKit;

namespace EVarlik.Service.Lookup.BusinessLayer
{
    public class TransactionStateOperation
    {
        public VarlikResult<List<TransactionStateDto>> GetAll()
        {
            var result = new VarlikResult<List<TransactionStateDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new TransactionStateDto().FromEntity().Expand();
                result.Data = ctx.TransactionStateEnum
                    .AsExpandable()
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }

            return result;
        }
    }
}