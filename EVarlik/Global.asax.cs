using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Http;
using EVarlik.Database.Context;
using EVarlik.Dto.Transactions;
using EVarlik.Hubs;
using log4net;
using log4net.Config; 
using EVarlik.Common.Attributes;
using EVarlik.ReadWrite;

namespace EVarlik
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            XmlConfigurator.Configure();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Filters.Add(new ExceptionHandlerAttributes());
            System.Data.Entity.Database.SetInitializer<VarlikContext>(null);

            Seed();
        }
 

        void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandling.Execute(Server, Response);
        }


        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            SessionConfig.SessionExecute();
        }

        private void Seed()
        {
            try
            {
                using (var ctx = new VarlikContext())
                {
                    var isSeeded = ctx.TransactionTypeEnum.Any();
                    if (!isSeeded)
                    {
                        GenerateDummyData.WriteCoinType();
                        GenerateDummyData.WriteTransactionState();
                        GenerateDummyData.WriteTransactionType();
                        GenerateDummyData.WriteUser(); 
                        GenerateDummyData.WriteCommission();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
