using System;
using System.Web;
using log4net;

namespace EVarlik
{
    public class ExceptionHandling
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Execute(HttpServerUtility server, HttpResponse response)
        {
            Exception exc = server.GetLastError();
            if (exc.GetType() == typeof(HttpException))
            {

                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;

                server.Transfer("HttpErrorPage.aspx");
            }
            Log.Error(exc);
            server.ClearError();
        }
    }
}