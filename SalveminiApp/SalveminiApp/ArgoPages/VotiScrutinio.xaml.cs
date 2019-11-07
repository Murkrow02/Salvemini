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
            if (iOS.AppDelegate.HasNotch)
                fullLayout.Padding = new Thickness(20, 35, 20, 25);
#endif
            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            votiList.HeightRequest = App.ScreenHeight / 1.7;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Get cache
            var cachedVoti = CacheHelper.GetCache<RestApi.Models.ScrutinioGrouped>("VotiScrutinio");
            if (cachedVoti != null)
            {
                Votis = cachedVoti;
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

            //Show loading
            votiList.IsRefreshing = true;

            //Check connection status
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Download voti from api call
                    var response = await App.Argo.GetVotiScrutinio();

                    //Detect if call returned a message of error
                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        //Error occourred, notify the user
                        Costants.showToast(response.Message);
                        //Stop loading list
                        votiList.IsRefreshing = false;
                        return;
                    }

                    //Deserialize object
                    Votis = response.Data as RestApi.Models.ScrutinioGrouped;

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
                catch
                {
                    Costants.showToast("Non è stato possibile aggiornare i voti");
                }
            }
            else //No connection
            {
                Costants.showToast("connection");
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
            try
            {
                //Set layout
                quad1.TextColor = Color.White;
                quad1.BackgroundColor = Color.FromHex("7F80FF");
                quad2.TextColor = Color.FromHex("7F80FF");
                quad2.BackgroundColor = Color.White;

                //Change itemsource
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
            catch
            {
                Costants.showToast("Non è stato possibile caricare i voti del primo quadrimestre");
            }

        }

        void secondQuad_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                //Set layout
                quad2.TextColor = Color.White;
                quad2.BackgroundColor = Color.FromHex("7F80FF");
                quad1.TextColor = Color.FromHex("7F80FF");
                quad1.BackgroundColor = Color.White;

                //Change itemsource
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
            catch
            {
                Costants.showToast("Non è stato possibile caricare i voti del secondo quadrimestre");
            }

        }
    }
}
