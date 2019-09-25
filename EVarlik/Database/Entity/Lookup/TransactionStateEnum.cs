using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVarlik.Database.Entity.Lookup
{
    // Başarılı İptal Timeout , Açık Emir , EVarlik İptal
    [Table("ETransactionState", Schema = "varlik")]
    public class TransactionStateEnum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }

        public string Name { get; set; }
        
    }
}