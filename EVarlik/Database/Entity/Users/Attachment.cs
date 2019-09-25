using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVarlik.Database.Entity.Users
{
    [Table("Attachment", Schema = "varlik")]
    public class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Path { get; set; }
        public string BucketName { get; set; }
        public string IdFileType { get; set; }
        public string FileName { get; set; }

        public long IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual User User { get; set; }
    }
}