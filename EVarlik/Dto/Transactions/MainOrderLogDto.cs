using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Transactions;

namespace EVarlik.Dto.Transactions
{
    public class MainOrderLogDto
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string IdTransactionType { get; set; }
        public string IdTransactionState { get; set; }
        public string IdCoinType { get; set; }
        public string UserCoinTransactionOrderGuid { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal CoinAmount { get; set; }
        public decimal CoinUnitPrice { get; set; }
        public decimal MoneyAmount { get; set; }
        public string Iban { get; set; }
        public DateTime? TransactionDate { get; set; }

        //For Admin Panel
        public string Mail { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public Expression<Func<MainOrderLog, MainOrderLogDto>> FromEntity()
        {
            return l => new MainOrderLogDto()
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
                UserCoinTransactionOrderGuid = l.UserCoinTransactionOrderGuid,
                Iban = l.User.Iban,
                TransactionDate = l.TransactionDate
            };
        }

        public Expression<Func<MainOrderLog, MainOrderLogDto>> FromEntityForAdmin()
        {
            return l => new MainOrderLogDto()
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
                UserCoinTransactionOrderGuid = l.UserCoinTransactionOrderGuid,
                Iban = l.User.Iban,
                Mail = l.User.Mail,
                Name = l.User.Name,
                Surname = l.User.Surname,
                TransactionDate = l.TransactionDate
            };
        }

        public MainOrderLog ToEntity(MainOrderLogDto userCoinTransactionLogDto)
        {
            return new MainOrderLog()
            {
                Id = userCoinTransactionLogDto.Id,
                IdUser = userCoinTransactionLogDto.IdUser,
                IdTransactionType = userCoinTransactionLogDto.IdTransactionType,
                IdTransactionState = userCoinTransactionLogDto.IdTransactionState,
                IdCoinType = userCoinTransactionLogDto.IdCoinType,
                CreatedAt = userCoinTransactionLogDto.CreatedAt,
                CoinAmount = userCoinTransactionLogDto.CoinAmount,
                CoinUnitPrice = userCoinTransactionLogDto.CoinUnitPrice,
                MoneyAmount = userCoinTransactionLogDto.MoneyAmount,
                UserCoinTransactionOrderGuid = userCoinTransactionLogDto.UserCoinTransactionOrderGuid
            };
        }

    }
}