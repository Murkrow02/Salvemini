using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Forms9Patch;
using Syncfusion.SfChart.XForms;
using System.Linq;
using System.Reflection;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif
using Xamarin.Forms;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Plugin.Toasts;

namespace SalveminiApp.ArgoPages
{
    public partial class VotiScrutinio : ContentPage
    {
        public RestApi.Models.ScrutinioGrouped Votis = new RestApi.Models.ScrutinioGrouped();

        public VotiScrutinio()
        {
            InitializeComponent();

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif
            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            votiList.HeightRequest = App.ScreenHeight / 1.7;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Cache
            if (Barrel.Current.Exists("VotiScrutinio"))
            {
                Votis = Barrel.Current.Get<RestApi.Models.ScrutinioGrouped>("VotiScrutinio");
                votiList.ItemsSource = Votis.Primo;
                if (Votis.Primo.Count > 0)
                {
                    emptyLayout.IsVisible = false;
                }
                else
                {
                    emptyLayout.IsVisible = true;
                    placeholderLabel.Text = "I voti del primo quadrimestre non sono ancora stati pubblicati";
                }
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var notificator = DependencyService.Get<IToastNotificator>();

            votiList.IsRefreshing = true;

            //Api Call
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var datas = await App.Argo.GetVotiScrutinio();
                if (string.IsNullOrEmpty(datas.Message))
                {
                    Votis = datas.Data as RestApi.Models.ScrutinioGrouped;
                }
                else
                {
                    var options = new NotificationOptions()
                    {
                        Description = datas.Message
                    };

                    var result = await notificator.Notify(options);
                    votiList.IsRefreshing = false;
                    return;
                }

                //Fill List
                if (Votis != null)
                {
                    votiList.ItemsSource = Votis.Primo;
                    if (Votis.Primo.Count > 0)
                    {
                        emptyLayout.IsVisible = false;
                    }
                    else
                    {
                        emptyLayout.IsVisible = true;
                        placeholderLabel.Text = "I voti del primo quadrimestre non sono ancora stati pubblicati";
                    }
                    votiList.IsRefreshing = false;
                }
            }
            else
            {
                var options = new NotificationOptions()
                {
                    Description = "Nessuna connessione ad internet 🚀",
                };

                var result = await notificator.Notify(options);
            }

            votiList.IsRefreshing = false;
        }

        //Close modal
        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void firstQuad_Clicked(object sender, System.EventArgs e)
        {
            quad1.TextColor = Color.White;
            quad1.BackgroundColor = Color.FromHex("7F80FF");
            quad2.TextColor = Color.FromHex("7F80FF");
            quad2.BackgroundColor = Color.White;
            votiList.ItemsSource = Votis.Primo;
            if (Votis.Primo.Count > 0)
            {
                emptyLayout.IsVisible = false;
            }
            else
            {
                emptyLayout.IsVisible = true;
                placeholderLabel.Text = "I voti del primo quadrimestre non sono ancora stati pubblicati";
            }
        }

        void secondQuad_Clicked(object sender, System.EventArgs e)
        {
            quad2.TextColor = Color.White;
            quad2.BackgroundColor = Color.FromHex("7F80FF");
            quad1.TextColor = Color.FromHex("7F80FF");
            quad1.BackgroundColor = Color.White;
            votiList.ItemsSource = Votis.Secondo;
            if (Votis.Secondo.Count > 0)
            {
                emptyLayout.IsVisible = false;
            }
            else
            {
                emptyLayout.IsVisible = true;
                placeholderLabel.Text = "I voti del secondo quadrimestre non sono ancora stati pubblicati";
            }
        }
    }
}
