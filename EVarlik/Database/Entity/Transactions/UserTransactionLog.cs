using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVarlik.Common.Attributes;
using EVarlik.Database.Entity.Lookup;
using EVarlik.Database.Entity.Users;

namespace EVarlik.Database.Entity.Transactions
{
    [Table("UserTransactionLog", Schema = "varlik")]
    public class UserTransactionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long IdMainOrderLog { get; set; }
        [ForeignKey("IdMainOrderLog")]
        public virtual MainOrderLog MainOrderLog { get; set; }

        public long IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual User User { get; set; }

        public string IdTransactionType { get; set; }
        [ForeignKey("IdTransactionType")]
        public virtual TransactionTypeEnum TransactionTypeEnum { get; set; }

        public string IdTransactionState { get; set; }
        [ForeignKey("IdTransactionState")]
        public virtual TransactionStateEnum TransactionStateEnum { get; set; }

        public bool IsSucces { get; set; }

        public string IdCoinType { get; set; }
        [ForeignKey("IdCoinType")]
        public virtual CoinTypeEnum CoinTypeEnum { get; set; }

        // adamın satma / alma işlemini baslattıgı tarih
        public DateTime RequestedDate { get; set; } = DateTime.Now;
        //bizim dayıya parayı yatırdıgımız tarih
        public DateTime TransactionDate { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal CoinAmount { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal CoinUnitPrice { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal MoneyAmount { get; set; }

        public string FromHash { get; set; }
        public string ToHash { get; set; }

        public int ConfirmationCount { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal CommissionCoinCount { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal CommissionMoney { get; set; }

        public string TxId { get; set; }

        public string UserTransactionLogGuid { get; set; } = Guid.NewGuid().ToString();

    }
}