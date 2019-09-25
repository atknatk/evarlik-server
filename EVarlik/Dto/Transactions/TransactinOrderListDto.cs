using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Transactions;

namespace EVarlik.Dto.Transactions
{
    public class TransactinOrderListDto
    {
        public decimal CoinUnitPrice { get; set; }
        public decimal CoinAmount { get; set; }
        public decimal Total { get; set; }
        public string IdTransactionType { get; set; }

        public decimal Price { get; set; }

        public Expression<Func<UserCoinTransactionOrder, TransactinOrderListDto>> FromEntity()
        {
            return l => new TransactinOrderListDto()
            {
                CoinUnitPrice = l.CoinUnitPrice,
                CoinAmount = l.CoinAmount,
                Total = l.CoinUnitPrice * l.CoinAmount,
                IdTransactionType = l.IdTransactionType
            };
        }

    }
}