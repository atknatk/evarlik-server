using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVarlik.Database.Entity.Users
{
    [Table("VarlikLog", Schema = "varlik")]
    public class VarlikLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime LogDate { get; set; }
        public string Thread { get; set; }
        public string LogLevel { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Ip { get; set; }
    }
}