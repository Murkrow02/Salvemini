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
#endif

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            promemoriaList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            if (Barrel.Current.Exists("Promemoria"))
            {
                Promemorias = Barrel.Current.Get<List<RestApi.Models.Promemoria>>("Promemoria");
                promemoriaList.ItemsSource = Promemorias;
                emptyLayout.IsVisible = Promemorias.Count <= 0;
            }

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var notificator = DependencyService.Get<IToastNotificator>();

            //Start loading
            promemoriaList.IsRefreshing = true;

            //Api Call
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var datas = await App.Argo.GetPromemoria();

                if (string.IsNullOrEmpty(datas.Message))
                {
                    Promemorias = datas.Data as List<RestApi.Models.Promemoria>;
                }
                else
                {
                    var options = new NotificationOptions()
                    {
                        Description = datas.Message
                    };

                    var result = await notificator.Notify(options);
                }

                //Fill List
                promemoriaList.ItemsSource = Promemorias;
                emptyLayout.IsVisible = Promemorias.Count <= 0;
                promemoriaList.IsRefreshing = false;

            }
            else
            {
                var options = new NotificationOptions()
                {
                    Description = "Nessuna connessione ad internet 🚀",
                };

                var result = await notificator.Notify(options);
            }

        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
