using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVarlik.Database.Entity.Lookup
{
    // Alış Satış Para Yatırma Para Cekme Coin Yatırma/Cekme
    [Table("ETransactionType", Schema = "varlik")]
    public class TransactionTypeEnum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }

        public string Name { get; set; }
      
    }
}