using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace SalveminiApp.FlappyMimmo
{
    public partial class FlappyHome : ContentPage
    {
        public RestApi.Models.FlappyMoneteReturn Potenziamento = new RestApi.Models.FlappyMoneteReturn();

        public FlappyHome()
        {
            InitializeComponent();

            //Set Sizes
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
            singleImage.WidthRequest = App.ScreenWidth * 0.35;
            singleImage.HeightRequest = App.ScreenWidth * 0.35;
            boardImage.WidthRequest = App.ScreenWidth * 0.58;
            boardImage.HeightRequest = App.ScreenWidth * 0.35;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Check connection
            if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            //Get current multiplier
            Potenziamento = await App.Flappy.GetUpgrade();
            if (Potenziamento != null)
            {
                Preferences.Set("multiplier", Potenziamento.Valore - 1);
            }


            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!File.Exists(Path.Combine(documentsPath, "classicMimmo1.png")) || !File.Exists(Path.Combine(documentsPath, "classicMimmo2.png")) || !File.Exists(Path.Combine(documentsPath, "classicMimmo3.png")))
            {
                canPlay = false;
                var skin = await App.Flappy.BuySkin(1);
                //Download skin if is not downloaded
                for (int i = 1; i < 4; i++)
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(Costants.Uri("images/flappyskin/classicMimmo" + i), Path.Combine(documentsPath, "classicMimmo" + i + ".png"));
                    }
                }
                canPlay = true;
            }
        }

        bool canPlay = true;
        void Play_Tapped(object sender, EventArgs e)
        {
            if (canPlay)
            {
                Navigation.PushModalAsync(new GamePage());
            }
        }

        void Score_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Classifica());
        }

        void Shop_Tapped(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new Shop());
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
