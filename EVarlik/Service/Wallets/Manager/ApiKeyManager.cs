using EVarlik.Common.Model;
using EVarlik.Dto.Wallets;
using EVarlik.Service.Wallets.BusinessLayer;

namespace EVarlik.Service.Wallets.Manager
{
    public class ApiKeyManager
    {
        private readonly ApiKeyOperation _apiKeyOperation;

        public ApiKeyManager()
        {
            _apiKeyOperation = new ApiKeyOperation();
        }

        public VarlikResult<ApiKeyDto> GetApiKeyByIdCoinType(string idCoinType)
        {
            return _apiKeyOperation.GetApiKeyByIdCoinType(idCoinType);
        }
    }
}