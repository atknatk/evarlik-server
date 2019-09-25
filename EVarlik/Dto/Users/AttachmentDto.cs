using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Users;
using EVarlik.Identity;

namespace EVarlik.Dto.Users
{
    public class AttachmentDto
    {
        public long Id { get; set; }
        public long? IdUser { get; set; }
        public string Path { get; set; } 
        public string BucketName { get; set; }
        public string IdFileType { get; set; }
        public string FileName { get; set; }

        public Expression<Func<Attachment, AttachmentDto>> FromEntity()
        {
            return l => new AttachmentDto()
            {
                Id = l.Id, 
                Path = l.Path,
                BucketName = l.BucketName,
                IdFileType = l.IdFileType,
                FileName = l.FileName
            };
        }

        public Attachment ToEntity(AttachmentDto attachmentDto)
        {
            var idUser = IdentityHelper.Instance.CurrentUserId;

            return new Attachment()
            {
                Id = attachmentDto.Id,
                IdUser = idUser,
                Path = attachmentDto.Path,
                BucketName = attachmentDto.BucketName,
                IdFileType = attachmentDto.IdFileType,
                FileName = attachmentDto.FileName
            };
        }
    }
}