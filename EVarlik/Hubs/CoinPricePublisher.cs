using System.Collections.Generic;
using EVarlik.Dto.Transactions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EVarlik.Hubs
{
    public class CoinPricePublisher
    {
        public void PublishPrice(PriceDto priceDto)
        {
            var jsonValue = JsonConvert.SerializeObject(priceDto, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var hub = GlobalHost.ConnectionManager.GetHubContext<CoinPriceHub>();
            ((IClientProxy)hub.Clients.Group("price")).Invoke("getPrice", jsonValue).Wait();
        } 
    }
}