using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Transactions;

namespace EVarlik.Dto.Transactions
{
    public class UserTransactionLogDto
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string IdTransactionType { get; set; }
        public string IdTransactionState { get; set; }
        public bool IsSucces { get; set; }
        public string IdCoinType { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal CoinAmount { get; set; }
        public decimal CoinUnitPrice { get; set; }
        public decimal MoneyAmount { get; set; }

        public string FromHash { get; set; }
        public string ToHash { get; set; }

        public int ConfirmationCount { get; set; }
        public decimal CommissionCoinCount { get; set; }
        public decimal CommissionMoney { get; set; }

        public string TxId { get; set; }
        public string UserTransactionLogGuid { get; set; }

        public Expression<Func<UserTransactionLog, UserTransactionLogDto>> FromEntity()
        {
            return l => new UserTransactionLogDto()
            {
                Id = l.Id,
                IdUser = l.IdUser,
                IdTransactionType = l.IdTransactionType,
                IsSucces = l.IsSucces,
                IdCoinType = l.IdCoinType,
                RequestedDate = l.RequestedDate,
                TransactionDate = l.TransactionDate,
                CoinAmount = l.CoinAmount,
                CoinUnitPrice = l.CoinUnitPrice,
                MoneyAmount = l.MoneyAmount,
                FromHash = l.FromHash,
                ToHash = l.ToHash,
                ConfirmationCount = l.ConfirmationCount,
                UserTransactionLogGuid = l.UserTransactionLogGuid,
                CommissionCoinCount = l.CommissionCoinCount,
                CommissionMoney = l.CommissionMoney,
                TxId = l.TxId,
                IdTransactionState = l.IdTransactionState,
            };
        }

        public UserTransactionLog ToEntity(UserTransactionLogDto userCoinTransactionLogDto)
        {
            return new UserTransactionLog()
            {
                Id = userCoinTransactionLogDto.Id,
                IdUser = userCoinTransactionLogDto.IdUser,
                IdTransactionType = userCoinTransactionLogDto.IdTransactionType,
                IsSucces = userCoinTransactionLogDto.IsSucces,
                IdCoinType = userCoinTransactionLogDto.IdCoinType,
                RequestedDate = userCoinTransactionLogDto.RequestedDate,
                TransactionDate = userCoinTransactionLogDto.TransactionDate,
                CoinAmount = userCoinTransactionLogDto.CoinAmount,
                CoinUnitPrice = userCoinTransactionLogDto.CoinUnitPrice,
                MoneyAmount = userCoinTransactionLogDto.MoneyAmount,
                FromHash = userCoinTransactionLogDto.FromHash,
                ToHash = userCoinTransactionLogDto.ToHash,
                ConfirmationCount = userCoinTransactionLogDto.ConfirmationCount,
                UserTransactionLogGuid = userCoinTransactionLogDto.UserTransactionLogGuid,
                CommissionCoinCount = userCoinTransactionLogDto.CommissionCoinCount,
                CommissionMoney = userCoinTransactionLogDto.CommissionMoney,
                TxId = userCoinTransactionLogDto.TxId,
                IdTransactionState = userCoinTransactionLogDto.IdTransactionState,
            };
        }

    }
}