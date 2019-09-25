using System;
using System.Linq;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Transactions;

namespace EVarlik.Dto.Transactions
{
    public class MainOrderLogAdminDto
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
        public long UserId { get; set; }
        public string IdentityNo { get; set; }
        public string EvarlikBanka { get; set; }
        public string EvarlikBankaIban { get; set; }
        public string UserWalletAddress { get; set; }


        public Expression<Func<MainOrderLog, MainOrderLogAdminDto>> FromEntity()
        {
            return l => new MainOrderLogAdminDto()
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
                TransactionDate = l.TransactionDate,
                Mail = l.User.Mail,
                Name = l.User.Name,
                Surname = l.User.Surname,
                UserId = l.IdUser,
                IdentityNo = l.User.IdentityNo,
                EvarlikBanka = "Kuveyt Turk",
                EvarlikBankaIban = "IBAN",
                UserWalletAddress = l.User.UserWalletList.Where(a => a.IdCoinType == l.IdCoinType).Select(a => a.Address).FirstOrDefault()
            };
        }
    }
}