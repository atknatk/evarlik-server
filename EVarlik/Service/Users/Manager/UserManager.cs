using EVarlik.Authorization;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Dto.Users;
using EVarlik.Dto.Wallets;
using EVarlik.Identity;
using EVarlik.Service.Users.BusinessLayer;
using EVarlik.Service.Wallets.Manager;
using Google.Cloud.Storage.V1;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace EVarlik.Service.Users.Manager
{
    public class UserManager
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserOperation _userOperation;

        public UserManager()
        {
            _userOperation = new UserOperation();
        }

        public VarlikResult<TokenUser> Login(string mail, string password)
        {
            var loginResult = _userOperation.Login(mail, password);
            if (loginResult.IsSuccess)
            {
                var jwtManager = new JwtManager();
                loginResult.Data.Token = jwtManager.Encode(loginResult.Data).Data;
            }
            return loginResult;
        }

        public VarlikResult Register(UserDto userDto)
        {
            if (userDto == null)
            {
                var preResult = new VarlikResult();
                preResult.Status = ResultStatus.MissingRequiredParamater;
                return preResult;
            }

            var name = userDto.Name.Trim();
            var firstCharOfName = name.Substring(0, 1);
            var remainingOfName = name.Substring(1);
            userDto.Name = firstCharOfName.ToUpper() + remainingOfName.ToLower();

            var surname = userDto.Surname.Trim();
            var firstCharOfSurname = surname.Substring(0, 1);
            var remaingOfSurname = surname.Substring(1);
            userDto.Surname = firstCharOfSurname.ToUpper() + remaingOfSurname.ToLower();

            var result = _userOperation.Register(userDto);
            if (result.IsSuccess)
            {
                MailManager mailManager = new MailManager();
                mailManager.SendWelcome(userDto);
            }
            return result;
        }

        public VarlikResult<string> ForgotPassword(string mail)
        {
            var result = _userOperation.ForgotPassword(mail);

            if (result.IsSuccess)
            {
                //mail
                var userR = GetUserByMail(mail);
                if (userR.IsSuccess)
                {
                    userR.Data.Password = result.Data;
                    MailManager mailManager = new MailManager();
                    mailManager.SendForgotPassword(userR.Data);
                }
            }
            result.Data = null;
            return result;
        }

        public VarlikResult<UserDto> GetUserByMail(string mail)
        {
            return _userOperation.GetUserByMail(mail);
        }

        public VarlikResult<UserWalletDto> GetWalletAddress(string idCoinType)
        {
            var idUser = IdentityHelper.Instance.CurrentUserId;
            return _userOperation.GetWalletAddresByIdCoinType(idUser, idCoinType);
        }

        public VarlikResult<bool> DoesUserHaveThisAddress(long idUser, string idCoinType, string address = null)
        {
            return _userOperation.DoesUserHaveThisAddress(idUser, idCoinType, address);
        }

        public VarlikResult<WalletResultDto> AddWalletAddress(string idCoinType)
        {
            var result = new VarlikResult<WalletResultDto>();
            result.Data = new WalletResultDto();
            var userId = IdentityHelper.Instance.CurrentUserId;
            // has this wallet or not 
            var alreadyAdded = DoesUserHaveThisAddress(userId, idCoinType);
            if (!alreadyAdded.IsSuccess)
            {
                result.Status = alreadyAdded.Status;
                return result;
            }
            if (alreadyAdded.Data)
            {
                result.Status = ResultStatus.AlreadyAdded;
                return result;
            }

            if (idCoinType == CoinTypeEnum.DogeCoin
                || idCoinType == CoinTypeEnum.LiteCoin
                || idCoinType == CoinTypeEnum.Btc)
            {
                // block io
                var walletManager = new BlockioWalletManager();
                var walletApiRes = walletManager.CreateNewAddress(idCoinType);
                if (walletApiRes.IsSuccess)
                {
                    UserWalletDto userWalletDto = new UserWalletDto();
                    userWalletDto.Address = walletApiRes.Data;
                    userWalletDto.IdUser = userId;
                    userWalletDto.IdCoinType = idCoinType;
                    _userOperation.AddWalletAddress(userWalletDto);
                    result.Data.Address = walletApiRes.Data;
                    result.Success();
                    return result;
                }
            }
            else if (idCoinType == CoinTypeEnum.Iota)
            {
                var walletManager = new IotaWalletManager();
                var walletApiRes = walletManager.CreateNewAddress();
                UserWalletDto userWalletDto = new UserWalletDto();
                userWalletDto.Address = walletApiRes.Data.Address;
                userWalletDto.Secret = walletApiRes.Data.Secret;
                userWalletDto.IdUser = userId;
                userWalletDto.IdCoinType = idCoinType;
                _userOperation.AddWalletAddress(userWalletDto);
                result.Data = walletApiRes.Data;
                result.Success();
                return result;
            }
            else if (idCoinType == CoinTypeEnum.Ripple)
            {
                var walletManager = new RippleWalletManager();
                var walletApiRes = walletManager.CreateNewAddress();
                UserWalletDto userWalletDto = new UserWalletDto();
                userWalletDto.Address = walletApiRes.Data.Address;
                userWalletDto.Secret = walletApiRes.Data.Secret;
                userWalletDto.IdUser = userId;
                userWalletDto.IdCoinType = idCoinType;
                _userOperation.AddWalletAddress(userWalletDto);
                result.Data = walletApiRes.Data;
                result.Success();
                return result;
            }
            return new VarlikResult<WalletResultDto>();
        }

        public async Task<VarlikResult> SaveAttachment(AttachmentDto attachmentDto)
        {
            return await _userOperation.SaveAttachment(attachmentDto);
        }

        public async Task<VarlikResult<AttachmentDto>> UploadFile(HttpPostedFile file)
        {
            var result = new VarlikResult<AttachmentDto>();

            StorageClient storageClient = StorageClient.Create();

            var createBucketResult = CreateBucket();
            if (!createBucketResult.IsSuccess)
            {
                result.Status = createBucketResult.Status;
                return result;
            }

            var bucketName = createBucketResult.Data;
            var fileName = IdentityHelper.Instance.TokenUser.Mail + "_" + Guid.NewGuid().ToString();
            var fileAcl = PredefinedObjectAcl.BucketOwnerFullControl;
            try
            {

                var fileObject = await storageClient.UploadObjectAsync(
                    bucket: bucketName,
                    objectName: fileName,
                    contentType: file.ContentType,
                    source: file.InputStream,
                    options: new UploadObjectOptions { PredefinedAcl = fileAcl }
                );

                AttachmentDto attachmentDto = new AttachmentDto();
                attachmentDto.BucketName = bucketName;
                attachmentDto.IdFileType = file.ContentType;
                attachmentDto.Path = fileObject.MediaLink;
                attachmentDto.FileName = file.FileName;


                var attachmentSaveResult = await SaveAttachment(attachmentDto);

                attachmentDto.Id = attachmentSaveResult.ObjectId;

                result.Data = attachmentDto;
                result.Data.Path = null;
                result.Status = attachmentSaveResult.Status;
                result.ObjectId = attachmentSaveResult.ObjectId;
                return result;
            }
            catch (Exception e)
            {
            }
            return result;
        }

        private VarlikResult<string> CreateBucket()
        {
            var bucketProjectId = WebConfigurationManager.AppSettings["bucketProjectId"];
            if (string.IsNullOrEmpty(bucketProjectId))
            {
                Log.Error("CreateBucket : Cannot Get Bucket Project Id");
                return new VarlikResult<string>();
            }

            var result = new VarlikResult<string>();
            string projectId = bucketProjectId;
            try
            {
                StorageClient storageClient = StorageClient.Create();
                string bucketName = projectId + "_files";
                result.Data = bucketName;
                result.Success();
                storageClient.CreateBucket(projectId, bucketName);
            }
            catch (Google.GoogleApiException e)
            {
                if (e.Error.Code != 409)
                {
                    result.Status = ResultStatus.UnknownError;
                }
            }
            return result;
        }

        public VarlikResult UpdatePassword(UserUpdatePasswordDto userUpdatePasswordDto)
        {
            if (userUpdatePasswordDto == null)
            {
                var preResult = new VarlikResult();
                preResult.Status = ResultStatus.MissingRequiredParamater;
                return preResult;
            }

            var idUser = IdentityHelper.Instance.CurrentUserId;
            return _userOperation.UpdatePassword(idUser, userUpdatePasswordDto);
        }

        public VarlikResult<UserDto> GetUserById()
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _userOperation.GetUserById(idUser);
        }

        public VarlikResult Update(UserDto userDto)
        {
            if (userDto == null)
            {
                var preResult = new VarlikResult();
                preResult.Status = ResultStatus.MissingRequiredParamater;
                return preResult;
            }

            userDto.Id = IdentityHelper.Instance.CurrentUserId;
            return _userOperation.Update(userDto);
        }

        public VarlikResult<List<UserDto>> GetAllUserAdmin()
        {
            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId > 0 && userId < 1001)
            {
                return _userOperation.GetAllUserAdmin();
            }
            return new VarlikResult<List<UserDto>>();
        }
    }
}