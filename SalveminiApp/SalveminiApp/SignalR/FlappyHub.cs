//using System;
//using Microsoft.AspNet.SignalR.Client;
//using Xamarin.Forms;

//namespace SalveminiApp.SignalR
//{
//    public class FlappyHub
//    {
//        public HubConnection hubConnection = new HubConnection(Costants.Uri("", false));
//        public IHubProxy hubProxy;

//        public FlappyHub()
//        {
//            hubProxy = hubConnection.CreateHubProxy("FlappyHub");
//        }

        

//        public async void InitilizeHub()
//        {
//            try
//            {
//                hubConnection.Start().Wait();
//                hubProxy.On<string>("SendSuccess", (message) =>
//                {
//                    MessagingCenter.Send<App>((App)Application.Current, "AlertGroup");
//                });

//                hubProxy.On<float>("updateY", (y) =>
//                {
//                    MessagingCenter.Send<App>((App)Application.Current, "AlertGroup");

//                  //  MessagingCenter.Send<App, float>((App)Application.Current, "UpdateSlider", y);
//                });

//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public async void creaGruppo(string id)
//        {

//            hubProxy.Invoke("CreateGroup", id).Wait();
//        }

//        public async void joinGruppo(string id)
//        {
//            hubProxy.Invoke("JoinGroup", id, 2).Wait();
//        }

//        public async void sendY(float y, string code)
//        {
//            hubProxy.Invoke("sendY", y, code).Wait();
//        }



//    }
//}
