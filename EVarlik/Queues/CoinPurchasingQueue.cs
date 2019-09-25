using System.Collections.Generic;
using EVarlik.Dto.Transactions;

namespace EVarlik.Queues
{
    public class CoinPurchasingQueue
    {
        private static CoinPurchasingQueue _instance = null;
        private static readonly object Lock = new object();

        private Dictionary<long, UserCoinTransactionOrderDto> PurchasingQueue = new Dictionary<long, UserCoinTransactionOrderDto>();

        public static CoinPurchasingQueue Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CoinPurchasingQueue();
                        }
                    }
                }
                return _instance;
            }
        }

        public void AddToQueue(UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            lock (Lock)
            {
                if (!PurchasingQueue.ContainsKey(userCoinTransactionOrderDto.Id))
                {
                    PurchasingQueue.Add(userCoinTransactionOrderDto.Id,userCoinTransactionOrderDto);
                }
            }
        }

        public UserCoinTransactionOrderDto DeleteFromQueue(long key)
        {
            lock (Lock)
            {
                if (PurchasingQueue.ContainsKey(key))
                {
                    var result = PurchasingQueue[key];
                    PurchasingQueue.Remove(key);
                    return result;
                }
            }
            return null;
        }

        public void UpdateFromQueue(UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            lock (Lock)
            {
                if (PurchasingQueue.ContainsKey(userCoinTransactionOrderDto.Id))
                {
                    PurchasingQueue[userCoinTransactionOrderDto.Id] = userCoinTransactionOrderDto;
                }
            }
        }
    }
}