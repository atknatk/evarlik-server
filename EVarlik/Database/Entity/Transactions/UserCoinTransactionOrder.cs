﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVarlik.Common.Attributes;
using EVarlik.Database.Entity.Lookup;
using EVarlik.Database.Entity.Users;

namespace EVarlik.Database.Entity.Transactions
{
    [Table("UserCoinTransactionOrder", Schema = "varlik")]
    public class UserCoinTransactionOrder
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

        public string IdCoinType { get; set; }
        [ForeignKey("IdCoinType")]
        public virtual CoinTypeEnum CoinTypeEnum { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DecimalPrecision(22, 10)]
        public decimal CoinAmount { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal CoinUnitPrice { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal MoneyAmount { get; set; }

        public string FromWalletAddress { get; set; }
        public string ToWalletAddress { get; set; }

        public string UserCoinTransactionOrderGuid { get; set; } = Guid.NewGuid().ToString().Split('-')[0];

        // Source User Ekle

    }
}