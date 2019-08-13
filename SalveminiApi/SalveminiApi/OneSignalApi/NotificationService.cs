using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SalveminiApi.OneSignalApi
{
    public class NotificationService
    {
        public static void sendNotification(NotificationModel notifica)
        {
            var client = new RestClient("https://onesignal.com/api/v1/notifications");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("", Method.POST);
          
            // easily add HTTP Headers
            request.AddHeader("Authorization", "Basic MGFlMzY3NWEtMDA5OC00NjQ2LTg3N2EtYzU0ZmVkMzAwZjVk");

            request.AddJsonBody(notifica);

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

        }
    }
}