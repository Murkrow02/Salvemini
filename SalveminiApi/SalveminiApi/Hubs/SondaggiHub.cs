using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SalveminiApi.Hubs
{
    [HubName("SondaggiHub")]
    public class SondaggiHub : Hub
    {
        IHubContext context;

        public SondaggiHub()
        {
          context = GlobalHost.ConnectionManager.GetHubContext<SondaggiHub>();
        }

        //A vote was added in a poll
        public void NewVoto()
        {
            //Update results on client
            context.Clients.All.UpdateVoti();
        }




        public override Task OnDisconnected(bool stopCalled)
        {

            return base.OnDisconnected(stopCalled);

        }


    }

}