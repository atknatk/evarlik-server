using EVarlik.Common.Model;
using System; 
using EVarlik.Authorization;
using EVarlik.Service.Users.Manager;

namespace EVarlik.Identity
{
    [Serializable]
    public class DinazorIdentity : System.Security.Principal.IIdentity
    {
        private static DinazorIdentity _dinazorIdentity;
        public string AuthenticationType => "Dinazor";
        public bool IsAuthenticated { get; private set; }

        public string Name { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public string ClientId { get; private set; } = string.Empty;
        public TokenUser TokenUser { get; private set; }


        internal static DinazorIdentity UnauthenticatedIdentity()
        {
            return new DinazorIdentity();
        }

        internal static DinazorIdentity GetIdentity(string userName, string password)
        {
            return AuthenticateandAuthorizeMe(userName, password);
        }

        internal static DinazorIdentity GetIdentity(string token)
        {
            return AuthenticateMe(token);
        }

        private DinazorIdentity()
        {

        }

        private static DinazorIdentity AuthenticateandAuthorizeMe(string mail, string password)
        {
            _dinazorIdentity = new DinazorIdentity();
            try
            { 
                var userManager = new UserManager();
                var tokenData = userManager.Login(mail,password);
                
                if (tokenData.IsSuccess)
                {
                    return AuthenticateMe(tokenData.Data);
                }
                if (_dinazorIdentity.TokenUser == null)
                {
                    _dinazorIdentity.TokenUser = new TokenUser();
                }
                _dinazorIdentity.TokenUser.Token = string.Empty;
                _dinazorIdentity.TokenUser.Mail = string.Empty;
                _dinazorIdentity.TokenUser.IsAuthenticated = false;
                return (_dinazorIdentity);
            }
            catch (Exception exce)
            {
                throw new Exception("Error while retrieving the authorization details. Please contact administrator.", exce);
            }
        }

        private static DinazorIdentity AuthenticateMe(TokenUser tokenUser)
        {
            _dinazorIdentity = new DinazorIdentity();
            _dinazorIdentity.TokenUser = new TokenUser();

            try
            {
                if (tokenUser != null)
                {
                    _dinazorIdentity.TokenUser = tokenUser;
                    _dinazorIdentity.IsAuthenticated = true;
                }
                else
                {
                    _dinazorIdentity.TokenUser.Token = string.Empty;
                    _dinazorIdentity.TokenUser.Mail = string.Empty;
                    _dinazorIdentity.TokenUser.IsAuthenticated = false;
                }
                return (_dinazorIdentity);
            }
            catch (Exception exce)
            {
                throw new Exception("Error while retrieving the authorization details. Please contact administrator.", exce);
            }
        }

        private static DinazorIdentity AuthenticateMe(string token)
        {
            _dinazorIdentity = new DinazorIdentity();
            try
            {
                var jwtManager = new JwtManager();
                var tokenUserResult = jwtManager.Decode(token);
                if (tokenUserResult.IsSuccess)
                {
                    return AuthenticateMe(tokenUserResult.Data);
                }
                TokenUser tokenUser = null;
                return AuthenticateMe(tokenUser);
            }

            catch (Exception ex)
            {
                throw new Exception("Error while retrieving the authorization details. Please contact administrator.", ex);
            }
        }
    }
}