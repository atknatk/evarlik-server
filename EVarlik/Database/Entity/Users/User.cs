using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EVarlik.Database.Entity.Users
{
    [Table("User", Schema = "varlik")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public bool IsDeleted { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdentityNo { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Adres { get; set; }
        public bool IsContractApproved { get; set; } = true;
        public bool IsUserApproved { get; set; } = false;

        public string Iban { get; set; }
        public string BankName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ApprovalDate { get; set; }
        public string DenialReason { get; set; }

        public long? IdIdentityFilePath { get; set; }
        public long? IdIdentityFilePath2 { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserWallet> UserWalletList { get; set; }

    }
}