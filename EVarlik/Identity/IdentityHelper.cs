using System; 
using System.Web;
using EVarlik.Common.Model;

namespace EVarlik.Identity
{
    public class IdentityHelper
    {
        private static readonly object LockObj = new Object();
        private static IdentityHelper _instance;

        public static IdentityHelper Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (LockObj)
                {
                    {
                        _instance = new IdentityHelper();
                    }
                }
                return _instance;
            }
        }

        public DinazorIdentity DinazorIdentity
        {
            get
            {
                if (HttpContext.Current?.User?.Identity != null)
                {
                    try
                    {
                        return ((DinazorIdentity)HttpContext.Current?.User?.Identity);
                    }
                    catch (Exception e)
                    {

                    }
                }
                return null;
            }
        }
        public long CurrentUserId => DinazorIdentity?.TokenUser?.Id ?? -1;
        public TokenUser TokenUser => DinazorIdentity?.TokenUser;

        public string GetToken()
        {
            return DinazorIdentity?.TokenUser?.Token;
        }
    }
}