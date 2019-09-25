using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVarlik.Common.Attributes;
using EVarlik.Database.Entity.Lookup;

namespace EVarlik.Database.Entity.Commissions
{
    [Table("Commission", Schema = "varlik")]
    public class Commission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string IdCoinType { get; set; }
        [ForeignKey("IdCoinType")]
        public virtual CoinTypeEnum CoinTypeEnum { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal TransactionVolume { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal MakerPercatange { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal TakerPercatange { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal TransferFeeCoinCount { get; set; }

        [DecimalPrecision(22, 10)]
        public decimal TransferFee { get; set; }
        
    }
}