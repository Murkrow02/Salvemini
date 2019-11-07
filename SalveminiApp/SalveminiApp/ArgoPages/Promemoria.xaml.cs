using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
#if __IOS__
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.Toasts;

namespace SalveminiApp.ArgoPages
{
    public partial class Promemoria : ContentPage
    {
        public List<RestApi.Models.Promemoria> Promemorias = new List<RestApi.Models.Promemoria>();

        public Promemoria()
        {
            InitializeComponent();

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
            if (iOS.AppDelegate.HasNotch)
                fullLayout.Padding = new Thickness(20, 35, 20, 25);
#endif

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            promemoriaList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            var cachedPromemoria = CacheHelper.GetCache<List<RestApi.Models.Promemoria>>("Promemoria");
            if (cachedPromemoria != null)
            {
                Promemorias = cachedPromemoria;
                promemoriaList.ItemsSource = Promemorias;
                emptyLayout.IsVisible = Promemorias.Count <= 0;
            }

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Start loading
            promemoriaList.IsRefreshing = true;

            //Check connection status
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Download promemoria from api
                    var response = await App.Argo.GetPromemoria();

                    //Detect if call returned a message of error
                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        //Error occourred, notify the user
                        Costants.showToast(response.Message);
                        //Stop loading list
                        promemoriaList.IsRefreshing = false;
                        return;
                    }

                    //Deserialize object
                    Promemorias = response.Data as List<RestApi.Models.Promemoria>;

                    //Fill promemoria list
                    promemoriaList.ItemsSource = Promemorias;
                    emptyLayout.IsVisible = Promemorias.Count <= 0;
                }
                catch //Random error
                {
                    Costants.showToast("Non è stato possibile aggiornare i promemoria");
                }
            }
            else //No connection
            {
                Costants.showToast("connection");
            }

            //Stop loading list
            promemoriaList.IsRefreshing = false;
        }




        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
