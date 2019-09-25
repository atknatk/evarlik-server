using System;
using Microsoft.AspNet.SignalR.Hubs;

namespace EVarlik.Hubs
{
    [HubName("orderHub")]
    public class OrderHub : Microsoft.AspNet.SignalR.Hub
    {
        public void Subscribe(string idCoinType)
        {
            try
            {
                Groups.Add(Context.ConnectionId, idCoinType);
            }
            catch (Exception e)
            {
            }
        }

        public void Unsubscribe(string idCoinType)
        {
            try
            {
                Groups.Remove(Context.ConnectionId, idCoinType);
            }
            catch (Exception e)
            {
            }
        }
    }
}