using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EVarlik.Database.Entity.Lookup;

namespace EVarlik.Database.Entity.Users
{
    [Table("UserWallet", Schema = "varlik")]
    public class UserWallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public bool IsDeleted { get; set; }

        public long IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual User User { get; set; }

        public string IdCoinType { get; set; }
        [ForeignKey("IdCoinType")]
        public virtual CoinTypeEnum CoinTypeEnum { get; set; }

        public string Address { get; set; }
        public string Secret { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}