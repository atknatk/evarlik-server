using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVarlik.Common.Attributes;
using EVarlik.Database.Entity.Lookup;
using EVarlik.Database.Entity.Users;

namespace EVarlik.Database.Entity.Transactions
{
    [Table("TransactionInformationLog", Schema = "varlik")]
    public class TransactionInformationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual User User { get; set; }

        public string TxHash { get; set; }
        public string TxReceiptStatus { get; set; }
        public string BlockHeight { get; set; }
        public DateTime TimeStamp { get; set; }

        public string From { get; set; }
        public string To { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal Amount{ get; set; }

        public string IdCoinType { get; set; }
        [ForeignKey("IdCoinType")]
        public virtual CoinTypeEnum CoinTypeEnum { get; set; }
    }
}