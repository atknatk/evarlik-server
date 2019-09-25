using EVarlik.Dto.Transactions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EVarlik.Hubs
{
    public class OrderPublisher
    {
        public void PublishOrder(string idCoinType,TransactinOrderListDto transactinOrderListDto)
        {
            var jsonValue = JsonConvert.SerializeObject(transactinOrderListDto, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var hub = GlobalHost.ConnectionManager.GetHubContext<OrderHub>();
            ((IClientProxy)hub.Clients.Group(idCoinType)).Invoke("getOrder", jsonValue).Wait();
        }
    }
}