using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Transactions;

namespace EVarlik.Dto.Transactions
{
    public class UserCoinTransactionOrderDto
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string IdTransactionType { get; set; }
        public string IdTransactionState { get; set; }
        public string IdCoinType { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal CoinAmount { get; set; }
        public decimal CoinUnitPrice { get; set; }
        public decimal MoneyAmount { get; set; }
        public string FromWalletAddress { get; set; }
        public string ToWalletAddress { get; set; }
        public string UserCoinTransactionOrderGuid { get; set; }
        public string Pin { get; set; }

        public Expression<Func<UserCoinTransactionOrder, UserCoinTransactionOrderDto>> FromEntity()
        {
            return l => new UserCoinTransactionOrderDto()
            {
                Id = l.Id,
                IdUser = l.IdUser,
                IdTransactionType = l.IdTransactionType,
                IdTransactionState = l.IdTransactionState,
                IdCoinType = l.IdCoinType,
                CreatedAt = l.CreatedAt,
                CoinAmount = l.CoinAmount,
                CoinUnitPrice = l.CoinUnitPrice,
                MoneyAmount = l.MoneyAmount,
                FromWalletAddress = l.FromWalletAddress,
                ToWalletAddress = l.ToWalletAddress,
                UserCoinTransactionOrderGuid = l.UserCoinTransactionOrderGuid
            };
        }

        public UserCoinTransactionOrder ToEntity(UserCoinTransactionOrderDto userCoinTransactionOrderDto)
        {
            return new UserCoinTransactionOrder()
            {
                Id = userCoinTransactionOrderDto.Id,
                IdUser = userCoinTransactionOrderDto.IdUser,
                IdTransactionType = userCoinTransactionOrderDto.IdTransactionType,
                IdTransactionState = userCoinTransactionOrderDto.IdTransactionState,
                IdCoinType = userCoinTransactionOrderDto.IdCoinType,
                CreatedAt = DateTime.Now,
                CoinAmount = userCoinTransactionOrderDto.CoinAmount,
                CoinUnitPrice = userCoinTransactionOrderDto.CoinUnitPrice,
                UserCoinTransactionOrderGuid = userCoinTransactionOrderDto.UserCoinTransactionOrderGuid,
                MoneyAmount = userCoinTransactionOrderDto.MoneyAmount,
                FromWalletAddress = userCoinTransactionOrderDto.FromWalletAddress,
                ToWalletAddress = userCoinTransactionOrderDto.ToWalletAddress,
            };
        }
    }
}