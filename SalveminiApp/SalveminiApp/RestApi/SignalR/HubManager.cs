using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms;

namespace SalveminiApp.SignalR
{
    public class HubManager
    {
        public string Url = Costants.Uri("signalr",false);
        public HubConnection Connection { get; set; }
        public IHubProxy Hub { get; set; }

        public HubManager()
        {
            Connection = new HubConnection(Url, useDefaultUrl: false);
            Hub = Connection.CreateHubProxy("SignalRHub");
            Connection.Start().Wait();

            //Connect to signalr
            Hub.On<string>("acknowledgeMessage", (message) =>
            {
                MessagingCenter.Send<App>((App)Application.Current, "updateResults");
            });
        }

        public void SayHello(string message)
        {
            Hub.Invoke("hello", message);
            Console.WriteLine("hello method is called!");
        }

        public void Stop()
        {
            Connection.Stop();
        }
    }
}
