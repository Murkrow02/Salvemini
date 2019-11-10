using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SalveminiApi.Startup))]

namespace SalveminiApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR("/signalr", new Microsoft.AspNet.SignalR.HubConfiguration());
        }

    }
}
