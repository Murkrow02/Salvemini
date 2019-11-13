using System;
using System.Diagnostics;
using Microsoft.AspNet.SignalR.Client;

namespace SalveminiApp.SignalR
{
    public class SondaggiHub
    {
        public HubConnection hubConnection = new HubConnection(Costants.Uri("", false));
        public IHubProxy hubProxy;

        public SondaggiHub()
        {
            hubProxy = hubConnection.CreateHubProxy("SondaggiHub");
        }

        public async void InitilizeHub()
        {
            try
            {
                //Start connection with the hub
                hubConnection.Start().Wait();

                if(hubConnection.State != ConnectionState.Connected)
                {
                    Debug.WriteLine("Impossibile connettersi all hub dei sondaggi");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Impossibile connettersi all hub dei sondaggi" + ex);
            }
        }

        public async void Disconnect()
        {
            
            try { if (this.hubConnection.State != ConnectionState.Disconnected) hubConnection.Stop(); } catch { }
        }



    }
}
