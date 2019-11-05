using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Rg.Plugins.Popup.Extensions;
using Plugin.Toasts;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif

namespace SalveminiApp.ArgoPages
{
    public partial class Note : ContentPage
    {
        public List<RestApi.Models.Note> Notes = new List<RestApi.Models.Note>();

        public Note()
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
            noteList.HeightRequest = App.ScreenHeight / 1.8;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            if (Barrel.Current.Exists("Note"))
            {
                Notes = Barrel.Current.Get<List<RestApi.Models.Note>>("Note");
                noteList.ItemsSource = Notes;
                emptyLayout.IsVisible = Notes.Count <= 0;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var notificator = DependencyService.Get<IToastNotificator>();
            //Start loading
            noteList.IsRefreshing = true;

            //Api Call
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var response = await App.Argo.GetNote();

                if (!string.IsNullOrEmpty(response.Message))
                {
                    var options = new NotificationOptions()
                    {
                        Description = response.Message
                    };

                    var result = await notificator.Notify(options);
                }
                else
                {
                    Notes = response.Data as List<RestApi.Models.Note>;
                }
                //Fill List
                noteList.ItemsSource = Notes;
                emptyLayout.IsVisible = Notes.Count <= 0;

            }
            else
            {
                var options = new NotificationOptions()
                {
                    Description = "Nessuna connessione ad internet 🚀",
                };

                var result = await notificator.Notify(options);
            }

            noteList.IsRefreshing = false;
        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            //Close Page
            Navigation.PopModalAsync();
        }
    }
}
