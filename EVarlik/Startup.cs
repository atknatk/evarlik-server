using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EVarlik.Startup))]

namespace EVarlik
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            app.Map("/EnableDetailedErrors", map =>
            {
                var hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true
                };

                map.MapSignalR(hubConfiguration);
            });
        }
    }
}
