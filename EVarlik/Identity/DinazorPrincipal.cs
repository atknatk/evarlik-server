using System; 
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace EVarlik.Identity
{
    public class DinazorPrincipal : BaseDinazorPrincipal
    {
        static DinazorPrincipal _objPrincipal = null;

        public static IPrincipal User
        {
            get
            {
                if (HttpContext.Current == null)
                    return Thread.CurrentPrincipal = _objPrincipal;
                return HttpContext.Current.User = _objPrincipal;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.User = value;
                Thread.CurrentPrincipal = value;
            }
        }
        private DinazorPrincipal(IIdentity identity) : base(identity)
        {

        }


        public static bool AuthenticateUser(string token)
        {
            var objIdentity = DinazorIdentity.GetIdentity(token);

            if (!objIdentity.IsAuthenticated) return objIdentity.IsAuthenticated;
            _objPrincipal = new DinazorPrincipal(objIdentity);
            User = _objPrincipal;
            return objIdentity.IsAuthenticated;

        }

        public static void SignOut()
        {
            var objIdentity = DinazorIdentity.UnauthenticatedIdentity();
            _objPrincipal = new DinazorPrincipal(objIdentity);
            User = _objPrincipal;
        }
    }
}