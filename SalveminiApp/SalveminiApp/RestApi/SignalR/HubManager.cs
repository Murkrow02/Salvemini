using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Forms;

namespace SalveminiApp.SignalR
{
    public class HubManager
    {
        public HubConnection hubConnection;
        public HubManager()
        {
            // localhost for UWP/iOS or special IP for Android
            hubConnection = new HubConnectionBuilder()
        .WithUrl($"http://localhost:5001/signalr")
        .Build();


            hubConnection.On<string>("acknowledgeMessage", (message) =>
            {
                MessagingCenter.Send<App>((App)Application.Current, "updateResults");
            });
        }

        public async Task Connect()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                // Something has gone wrong
            }
        }
    }
}
