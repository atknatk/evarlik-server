using System.Collections.Generic;
using System.Linq;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Dto.Lookup;
using LinqKit;

namespace EVarlik.Service.Lookup.BusinessLayer
{
    public class TransactionTypeOperation
    {
        public VarlikResult<List<TransactionTypeDto>> GetAll()
        {
            var result = new VarlikResult<List<TransactionTypeDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new TransactionTypeDto().FromEntity().Expand();
                result.Data = ctx.TransactionTypeEnum
                    .AsExpandable()
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }

            return result;
        }
    }
}