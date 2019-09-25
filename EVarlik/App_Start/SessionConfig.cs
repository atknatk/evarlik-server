using System.Globalization;
using System.Web;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Identity;

namespace EVarlik
{
    public class SessionConfig
    {
        public static void SessionExecute()
        {
            string authorizationUrl = "/User/Login";
            string registerUrl = "/User/Register";
            string forgotPassUrl = "/User/ForgotPassword";
            string commisionUrl = "/Commission/All";
            string adminUrl = "/evarlik-admin";
           
            var request = HttpContext.Current.Request;

            // authorization url control

            var url = request.Path.ToLower(new CultureInfo("en-US", false));
            url = url[url.Length - 1] == '/' ? url.Remove(url.Length - 1) : url; 

            if (!string.IsNullOrEmpty(authorizationUrl))
            {
                if (url.EndsWith(authorizationUrl.ToLower()))
                { 
                    return;
                }
            }

            if (url.EndsWith(registerUrl.ToLower()) || 
                url.EndsWith(forgotPassUrl.ToLower()) ||
                url.EndsWith(commisionUrl.ToLower())
                || url.StartsWith(adminUrl.ToLower()))
            {
                return;
            }


            if (url.Contains("signalr"))
            {
                return;
            }
 

            var authorizationHeader = request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {

                RedirectVarlikResult.RedirectWithData(new VarlikResult()
                {
                    Status = ResultStatus.Unauthorized,
                    Message = "Jwt Token Does Not Exist in Authorization Header"
                });
                return;
            }

            var authorizationHeaderSplitted = authorizationHeader.Split(' ');
            if (authorizationHeaderSplitted.Length < 2)
            {
                RedirectVarlikResult.RedirectWithData(new VarlikResult()
                {
                    Status = ResultStatus.Unauthorized,
                    Message = "Most Probably Authentication Type Is Not Set"
                });
                return;
            }

            var token = authorizationHeaderSplitted[1];

            DinazorPrincipal.AuthenticateUser(token);

            var dinazorPrincipal = (DinazorPrincipal)HttpContext.Current.User;

            if (dinazorPrincipal == null || !dinazorPrincipal.Identity.IsAuthenticated)
            {
                RedirectVarlikResult.RedirectWithData(new VarlikResult()
                {
                    Status = ResultStatus.SessionNotValid,
                    Message = $"Incorrect Token : {token}"
                });
            }
        }
    }
}