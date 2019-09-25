using System.Collections.Generic;
using System.Linq;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Dto.Lookup;
using LinqKit;

namespace EVarlik.Service.Lookup.BusinessLayer
{
    public class CoinTypeOperation
    {
        public VarlikResult<List<CoinTypeDto>> GetAll()
        {
            var result = new VarlikResult<List<CoinTypeDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new CoinTypeDto().FromEntity().Expand();
                result.Data = ctx.CoinTypeEnum
                    .AsExpandable()
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }

            return result;
        }
    }
}