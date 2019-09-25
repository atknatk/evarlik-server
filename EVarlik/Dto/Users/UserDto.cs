using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Users;

namespace EVarlik.Dto.Users
{
    public class UserDto : BaseDto
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string IdentityNo { get; set; }
        [Required]
        public string Mail { get; set; }
        [Required]
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Adres { get; set; }
        public bool IsContractApproved { get; set; } 
        public bool IsUserApproved { get; set; }

        public string Iban { get; set; }
        public string BankName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ApprovalDate { get; set; }
        public string DenialReason { get; set; }

        public long? IdIdentityFilePath { get; set; }
        public long? IdIdentityFilePath2 { get; set; }

        public Expression<Func<User, UserDto>> FromEntity()
        {
            return l => new UserDto()
            {
                Id = l.Id,
                Name = l.Name,
                Surname = l.Surname,
                IdentityNo = l.IdentityNo,
                Mail = l.Mail,
                Password = l.Password,
                Phone = l.Phone,
                Adres = l.Adres,
                IsContractApproved = l.IsContractApproved,
                IsUserApproved = l.IsUserApproved,
                Iban = l.Iban,
                BankName = l.BankName,
                CreatedAt = l.CreatedAt,
                ApprovalDate = l.ApprovalDate,
                DenialReason = l.DenialReason,
                IdIdentityFilePath = l.IdIdentityFilePath,
                IdIdentityFilePath2 = l.IdIdentityFilePath2, 
            };
        }

        public User ToEntity(UserDto userDto)
        {
            return new User()
            {
                Id = userDto.Id,
                Name = userDto.Name,
                Surname = userDto.Surname,
                Password = userDto.Password,
                IdentityNo = userDto.IdentityNo,
                Mail = userDto.Mail,
                Phone = userDto.Phone,
                Adres = userDto.Adres,
                IsContractApproved = userDto.IsContractApproved,
                IsUserApproved = userDto.IsUserApproved,
                Iban = userDto.Iban,
                BankName = userDto.BankName,
                CreatedAt = userDto.CreatedAt,
                ApprovalDate = userDto.ApprovalDate,
                DenialReason = userDto.DenialReason,
                IdIdentityFilePath = userDto.IdIdentityFilePath,
                IdIdentityFilePath2 = userDto.IdIdentityFilePath2, 
            };
        }
    }
}