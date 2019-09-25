using System.IO;
using System.Net; 
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Dto.Wallets; 
using Newtonsoft.Json.Linq;

namespace EVarlik.Service.Wallets.Manager
{
    public class IotaWalletManager
    {
        public VarlikResult<WalletResultDto> CreateNewAddress()
        {
            var result = new VarlikResult<WalletResultDto>();

            var apiKeyManager = new ApiKeyManager();
            var apiKeyR = apiKeyManager.GetApiKeyByIdCoinType(CoinTypeEnum.Iota);
            if (!apiKeyR.IsSuccess)
            {
                return result;
            }

            var url = apiKeyR.Data.Url + "/createNewAddress/";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            dynamic parsed = JObject.Parse(responseString);
            result.Data = new WalletResultDto();
            result.Data.Secret = parsed.wallet.secret;
            result.Data.Address = parsed.wallet.address;
            result.Success();

            return result;
        }

    }
}