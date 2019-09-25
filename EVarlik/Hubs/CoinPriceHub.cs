using System;
using System.Threading;
using EVarlik.Service.Transactions.Manager;
using Microsoft.AspNet.SignalR.Hubs;

namespace EVarlik.Hubs
{
    [HubName("coinPriceHub")]
    public class CoinPriceHub : Microsoft.AspNet.SignalR.Hub
    {
        public void Subscribe()
        {
            try
            {
                Groups.Add(Context.ConnectionId, "price");

                Thread.Sleep(3000);

                var priceManager = new UserCoinTransactionLogManager();
                var result = priceManager.GetLastPrices();
                if (result.IsSuccess && result.Data != null)
                {
                    CoinPricePublisher coinPricePublisher = new CoinPricePublisher();
                    foreach (var item in result.Data)
                    {
                        coinPricePublisher.PublishPrice(item);
                    }
                }

            }
            catch (Exception e)
            {
            }
        }

        public void Unsubscribe()
        {
            try
            {
                Groups.Remove(Context.ConnectionId, "price");
            }
            catch (Exception e)
            {
            }
        }
    }
}