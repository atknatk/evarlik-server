using System.Linq;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Dto.Wallets;
using LinqKit;

namespace EVarlik.Service.Wallets.BusinessLayer
{
    public class ApiKeyOperation
    {
        public VarlikResult<ApiKeyDto> GetApiKeyByIdCoinType(string idCoinType)
        {
            var result = new VarlikResult<ApiKeyDto>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new ApiKeyDto().FromEntity().Expand();

                result.Data = ctx.ApiKey
                    .AsExpandable()
                    .Where(l => l.IdCoinType == idCoinType)
                    .Select(fromEntity)
                    .FirstOrDefault();

                result.Success();
            }
            return result;
        }
    }
}