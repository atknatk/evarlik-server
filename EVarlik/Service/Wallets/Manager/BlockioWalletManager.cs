using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Identity;
using EVarlik.Service.Users.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EVarlik.Service.Wallets.Manager
{
    public class BlockioWalletManager  
    {
        public VarlikResult<string> CreateNewAddress(string idCoinType)
        {
            var result = new VarlikResult<string>();

            var apiKeyManager = new ApiKeyManager();
            var apiKeyR = apiKeyManager.GetApiKeyByIdCoinType(idCoinType);
            if (!apiKeyR.IsSuccess)
            {
                return result;
            } 
            var url = apiKeyR.Data.Url + "/get_new_address/";
            url += "?api_key=" + apiKeyR.Data.Key;
           
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            dynamic parsed = JObject.Parse(responseString);
            if (parsed.status == "success")
            {
                result.Data = parsed.data.address;
                result.Success();
            }
            else
            {
                result.Message = parsed.data.error_message;
            } 
            return result;
        }

        public async Task<VarlikResult<decimal>> GetAddressBalance(string idCoinType, string address)
        {
            var result = new VarlikResult<decimal>();

            var apiKeyManager = new ApiKeyManager();
            var apiKeyR = apiKeyManager.GetApiKeyByIdCoinType(idCoinType);
            if (!apiKeyR.IsSuccess)
            {
                return result;
            }

            var url = apiKeyR.Data.Url + "/get_address_balance/";

            var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("api_key",apiKeyR.Data.Key),
                new KeyValuePair<string, string>("addresses",address)
            });

            HttpResponseMessage response = await client.PostAsync(url, requestContent);
            HttpContent responseContent = response.Content;

            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var json = await reader.ReadToEndAsync();
                dynamic parsed = JObject.Parse(json);
                if (parsed.status == "success")
                {
                    result.Data = decimal.Parse(parsed.data.available_balance.ToString());
                    result.Success();
                }
                else
                {
                    result.Message = parsed.data.error_message;
                }
            }
            return result;
        }

        public VarlikResult<string> Withdraw(string idCoinType,
                                                 string fromAddress,
                                                 string toAddress,
                                                 decimal amount,
                                                 string pin)
        {

            var result = new VarlikResult<string>();

            try
            {
                var apiKeyManager = new ApiKeyManager();
                var apiKeyR = apiKeyManager.GetApiKeyByIdCoinType(idCoinType);
                if (!apiKeyR.IsSuccess)
                {
                    return result;
                }

                var url = apiKeyR.Data.Url + "/withdraw_from_addresses/";

                url += "?api_key=" + apiKeyR.Data.Key;
                url += "&amounts=" + amount;
                url += "&from_addresses=" + fromAddress;
                url += "&to_addresses=" + toAddress;
                url += "&pin=" + pin;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                dynamic parsed = JObject.Parse(responseString);
                if (parsed.status == "success")
                {
                    result.Data = parsed.data.txid;
                    result.Success();
                }
                else
                {
                    result.Message = parsed.data.error_message;
                }

            }
            catch (Exception e)
            {
            }
            return result;
        }
    }
}