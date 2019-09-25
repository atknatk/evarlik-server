using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Users;
using EVarlik.Dto.Wallets;
using EVarlik.Service.Users.Manager;
using log4net;

namespace EVarlik.Service.Users.Controller
{
    public class UserController : VarlikController
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UserManager _userManager;

        public UserController()
        {
            _userManager = new UserManager();
        }

        [HttpPost]
        [Route("api/User/Login")]
        public VarlikResult<TokenUser> Login([FromBody] BaseTokenUser baseTokenUser)
        {
            //trying to login log
            string message = "Trying to Login : ";
            var userAgent = HttpContext.Current.Request.UserAgent;
            var ip = HttpContext.Current.Request.UserHostAddress;
            message += ip + " ";
            message += userAgent + " ";

            Log.Info(message);

            var result = _userManager.Login(baseTokenUser.Mail, baseTokenUser.Password);

            if (result.IsSuccess)
            {
                message = "Logged In : ";
                message += ip + " ";
                message += userAgent + " ";

                Log.Info(message);
            }
            else
            {
                message = "Cannot Be Logged In : ";
                message += ip + " ";
                message += userAgent + " ";

                Log.Warn(message);
            }

            return result;
        }

        [HttpPost]
        [Route("api/User/Register")]
        public VarlikResult Register(UserDto userDto)
        {
            return _userManager.Register(userDto);
        }

        [HttpPost]
        [Route("api/User/ForgotPassword")]
        public VarlikResult<string> ForgotPassword([FromBody]MailUpdateDto mailUpdateDto)
        {
            return _userManager.ForgotPassword(mailUpdateDto.Mail);
        }

        [HttpPost]
        [Route("api/User/Wallet")]
        public VarlikResult<WalletResultDto> AddWalletAddress(string idCoinType)
        {
            return _userManager.AddWalletAddress(idCoinType);
        }

        [HttpGet]
        [Route("api/User/Wallet")]
        public VarlikResult<UserWalletDto> GetWalletAddress(string idCoinType)
        {
            return _userManager.GetWalletAddress(idCoinType);
        }

        [HttpPost]
        [Route("api/Attachment/UploadFile")]
        public async Task<VarlikResult<AttachmentDto>> UploadFile()
        {
            var file = HttpContext.Current.Request.Files[0];
            return await _userManager.UploadFile(file);
        }

        [HttpPost]
        [Route("api/User/UpdatePassword")]
        public VarlikResult UpdatePassword(UserUpdatePasswordDto userUpdatePasswordDto)
        {
            return _userManager.UpdatePassword(userUpdatePasswordDto);
        }

        [HttpGet]
        public VarlikResult<UserDto> GetUserById()
        {
            return _userManager.GetUserById();
        }

        [HttpPost]
        public VarlikResult Update([FromBody]UserDto userDto)
        {
            return _userManager.Update(userDto);
        }

        [HttpGet]
        [Route("api/User/AllUserAdmin")]
        public VarlikResult<List<UserDto>> GetAllUserAdmin()
        {
            return _userManager.GetAllUserAdmin();
        }
    }

    public class MailUpdateDto
    {
        public string Mail { get; set; }
    }
}