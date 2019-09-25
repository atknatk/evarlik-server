using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVarlik.Common.Enum;
using EVarlik.Common.Hash;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Database.Entity.Users;
using EVarlik.Dto.Users;
using log4net;
using LinqKit;

namespace EVarlik.Service.Users.BusinessLayer
{
    public class UserOperation
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VarlikResult<TokenUser> Login(string mail, string password)
        {
            var result = new VarlikResult<TokenUser>();

            using (var ctx = new VarlikContext())
            {
                password = VarlikHasher.Hash(password);

                var entity = ctx.User
                    .FirstOrDefault(l => l.Mail == mail.ToLower()
                                && l.Password == password
                                && !l.IsDeleted);

                if (entity == null)
                {
                    result.Status = ResultStatus.LoginFailed;
                    return result;
                }

                result.Data = new TokenUser()
                {
                    Mail = mail,
                    Id = entity.Id,
                    CreatedAt = DateTime.Now,
                    IsApproved = entity.IsUserApproved,
                    Name = entity.Name,
                    Surname = entity.Surname
                };
                result.Success();
            }
            return result;
        }

        public VarlikResult Register(UserDto userDto)
        {
            var result = new VarlikResult();
            using (var ctx = new VarlikContext())
            {
                result = userDto.IsValid();
                if (!result.IsSuccess)
                {
                    return result;
                }

                var alreadyAdded = ctx.User.Any(l => l.Mail == userDto.Mail && !l.IsDeleted);
                if (alreadyAdded)
                {
                    result.Status = ResultStatus.AlreadyAdded;
                    return result;
                }

                userDto.Password = VarlikHasher.Hash(userDto.Password);

                User user = new User()
                {
                    Mail = userDto.Mail.ToLower(),
                    Password = userDto.Password,
                    Name = userDto.Name,
                    Surname = userDto.Surname,
                    Phone = userDto.Phone,
                    IsContractApproved = true
                };
                ctx.User.Add(user);
                try
                {
                    ctx.SaveChanges();
                    result.Success();
                }
                catch (Exception e)
                {
                    Log.Error("Register", e);
                }
            }
            return result;
        }

        public VarlikResult<string> ForgotPassword(string mail)
        {
            var result = new VarlikResult<string>();
            using (var ctx = new VarlikContext())
            {
                var user =ctx.User.FirstOrDefault(l => l.Mail == mail && !l.IsDeleted);
                if (user!=null)
                {
                    user.Password = Guid.NewGuid().ToString();
                    user.Password = VarlikHasher.Hash(user.Password);
                    try
                    {
                        ctx.SaveChanges();
                        result.Success(); 
                        result.Data = user.Password;
                    }
                    catch (Exception e)
                    {
                        Log.Error("ForgotPassword", e);
                    }
                }
            }
            return result;
        }

        public VarlikResult<UserDto> GetUserByMail(string mail)
        {
            var result = new VarlikResult<UserDto>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserDto().FromEntity().Expand();
                result.Data = ctx.User.AsExpandable().Where(l => l.Mail == mail &&!l.IsDeleted).Select(fromEntity).FirstOrDefault();
                result.Success();
            }

            return result;
        }

        public VarlikResult Save(UserDto userDto)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var isAdded = ctx.User.Any(l => !l.IsDeleted && l.Mail == userDto.Mail);
                if (isAdded)
                {
                    result.Status = ResultStatus.AlreadyAdded;
                    return result;
                }

                var entity = userDto.ToEntity(userDto);

                var persistent = ctx.User.Add(entity);

                try
                {
                    ctx.SaveChanges();
                    result.Success();
                    result.ObjectId = persistent.Id;
                }
                catch (Exception e)
                {
                    Log.Error("Save", e);
                }
            }
            return result;
        }

        public VarlikResult Update(UserDto userDto)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var old = ctx.User.FirstOrDefault(l => !l.IsDeleted && l.Id == userDto.Id);
                if (old==null)
                {
                    result.Status = ResultStatus.NoSuchObject;
                    return result;
                }

                old.IdentityNo = userDto.IdentityNo;
                old.Iban = userDto.Iban;
                old.BankName = userDto.BankName; 
                old.Adres = userDto.Adres;

                if (!old.IsUserApproved)
                {
                    old.IdIdentityFilePath = userDto.IdIdentityFilePath;
                    old.IdIdentityFilePath2 = userDto.IdIdentityFilePath2;
                }
                
                try
                {
                    ctx.SaveChanges();
                    result.Success();
                }
                catch (Exception e)
                {
                    Log.Error("Update", e);
                }
            }
            return result;
        }

        public VarlikResult<bool> DoesUserHaveThisAddress(long idUser, string idCoinType,string address =null)
        {
            var result = new VarlikResult<bool>();

            using (var ctx = new VarlikContext())
            {
                if (address!=null)
                {
                    result.Data = ctx.UserWallet
                        .Any(l => l.IdUser == idUser && l.IdCoinType == idCoinType && l.Address == address);
                    result.Success();
                }
                else
                {
                    result.Data = ctx.UserWallet
                        .Any(l => l.IdUser == idUser && l.IdCoinType == idCoinType);
                    result.Success();
                }
                
            }

            return result;
        }

        public VarlikResult AddWalletAddress(UserWalletDto userWalletDto)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var isAdded = ctx.UserWallet.Any(l =>
                    l.IdUser == userWalletDto.IdUser && l.IdCoinType == userWalletDto.IdCoinType);
                if (isAdded)
                {
                    result.Status = ResultStatus.AlreadyAdded;
                    return result;
                }
                var entity = userWalletDto.ToEntity(userWalletDto);
                var persistent =ctx.UserWallet.Add(entity);
                try
                {
                    ctx.SaveChanges();
                    result.Success();
                    result.ObjectId = persistent.Id;
                }
                catch (Exception e)
                {
                    Log.Error("AddWalletAddress", e);
                }
            }
            return result;
        }

        public VarlikResult<UserWalletDto> GetWalletAddresByIdCoinType(long idUser, string idCoinType)
        {
            var result = new VarlikResult<UserWalletDto>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserWalletDto().FromEntity().Expand();
                result.Data = ctx.UserWallet
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser && l.IdCoinType == idCoinType)
                    .Select(fromEntity)
                    .FirstOrDefault();

                if (result.Data ==null)
                {
                    result.Status = ResultStatus.UserDoesNotHaveThisAddress;
                }
                else
                {
                    result.Success();
                }
            }
            return result;
        }

        public async Task<VarlikResult> SaveAttachment(AttachmentDto attachmentDto)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var entity = attachmentDto.ToEntity(attachmentDto);
                var persistent =ctx.Attachment.Add(entity);
                try
                {
                    ctx.SaveChanges();
                    result.Success();
                    result.ObjectId = persistent.Id;
                }
                catch (Exception e)
                {
                    Log.Error("SaveAttachment", e);
                }
            }

            return result;
        }

        public VarlikResult UpdatePassword(long idUser,UserUpdatePasswordDto userUpdatePasswordDto)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var user = ctx.User.FirstOrDefault(l => !l.IsDeleted && l.Id == idUser && l.Password==userUpdatePasswordDto.OldPassword);
                if (user==null)
                {
                    result.Status = ResultStatus.NoSuchObject;
                    return result;
                }
                user.Password = VarlikHasher.Hash(user.Password);
                user.Password = userUpdatePasswordDto.Password;
                try
                {
                    ctx.SaveChanges();
                    result.Success();
                }
                catch (Exception e)
                {
                    Log.Error("UpdatePassword", e);
                }
            }
            return result;
        }

        public VarlikResult<UserDto> GetUserById(long idUser)
        {
            var result = new VarlikResult<UserDto>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserDto().FromEntity().Expand();
                result.Data = ctx.User
                    .AsExpandable()
                    .Where(l => l.Id == idUser && !l.IsDeleted)
                    .Select(fromEntity)
                    .FirstOrDefault();
                result.Success();
            }
            return result;
        }

        public VarlikResult<List<UserDto>> GetAllUserAdmin()
        {
            var result = new VarlikResult<List<UserDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserDto().FromEntity().Expand();
                result.Data = ctx.User
                    .AsExpandable()
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(fromEntity)
                    .ToList();
                result.Success();
            }

            return result;
        }
    }
}