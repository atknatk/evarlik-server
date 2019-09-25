using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVarlik.Database.Entity.Lookup
{
    [Table("ECoinType", Schema = "varlik")]
    public class CoinTypeEnum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }

        public string Name { get; set; }

    }
}