//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Microsoft.AspNet.SignalR;

//namespace SalveminiApi.Hubs
//{
//    public class SignalRHub : Hub
//    {
//        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();

//        public static void SondaggioUpdate(string message)
//        {
//            hubContext.Clients.All.acknowledgeMessage(message);
//        }

     
//    }
//}