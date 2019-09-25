using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVarlik.Database.Entity.Lookup;

namespace EVarlik.Database.Entity.Wallets
{
    [Table("ApiKey", Schema = "varlik")]
    public class ApiKey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string IdCoinType { get; set; }
        [ForeignKey("IdCoinType")]
        public virtual CoinTypeEnum CoinTypeEnum { get; set; }

        public string Key { get; set; }
        public string Url { get; set; }

    }
}