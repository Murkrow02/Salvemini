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
    public partial class Bacheca : ContentPage
    {
        public List<RestApi.Models.Bacheca> Bacheche = new List<RestApi.Models.Bacheca>();
        IToastNotificator notificator = DependencyService.Get<IToastNotificator>();

        public Bacheca()
        {
            InitializeComponent();

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            bachecaList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            if (Barrel.Current.Exists("Bacheca"))
            {
                Bacheche = Barrel.Current.Get<List<RestApi.Models.Bacheca>>("Bacheca");
                bachecaList.ItemsSource = Bacheche;
                emptyLayout.IsVisible = Bacheche.Count <= 0;
            }
        }

        private async void Bacheca_Selected(object sender, SelectedItemChangedEventArgs e)
        {
            var data = e.SelectedItem as RestApi.Models.Bacheca;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (!data.presaVisione)
                {
                    var success = await App.Argo.VisualizzaBacheca(new RestApi.Models.VisualizzaBacheca { presaVisione = true, prgMessaggio = data.prgMessaggio });
                }
            }
            else
            {
                var options = new NotificationOptions
                {
                    Description = "Nessuna connessione ad internet 🚀",
                };

                var result = await notificator.Notify(options);
            }

            //Create page with webview
            var content = new ContentPage { Title = (e.SelectedItem as RestApi.Models.Bacheca).formattedTitle, Content = new WebView { Source = (e.SelectedItem as RestApi.Models.Bacheca).Allegati[0].fullUrl } };
            bool haftaClose = true;
            //Add toolbaritems to the page
            var barItem = new ToolbarItem { Text = "Chiudi" , };
            barItem.Clicked += (object mandatore, EventArgs f) =>
            {
                haftaClose = false;
                Navigation.PopModalAsync();
            };
            content.ToolbarItems.Add(barItem);

            //Make it navigable
            var webPage = new Xamarin.Forms.NavigationPage(content) { BarTextColor = Styles.TextColor };

            //iOS 13 modal bug
            webPage.Disappearing += (object disappearer, EventArgs g) =>
            {
                try
                {
                    if (haftaClose)
                    {
                        Navigation.PopModalAsync();
                    }
                    if (!data.presaVisione)
                    {
                        OnAppearing();
                    }
                }
                catch { }
            };

            //Set the presentation to formsheet
#if __IOS__
            webPage.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
            //Push there
            await Navigation.PushModalAsync(webPage);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();


            //Start loading
            bachecaList.IsRefreshing = true;

            //Api Call
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var datas = await App.Argo.GetBacheca();

                if (string.IsNullOrEmpty(datas.Message))
                {
                    Bacheche = datas.Data as List<RestApi.Models.Bacheca>;
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
                bachecaList.ItemsSource = Bacheche;
                emptyLayout.IsVisible = Bacheche.Count <= 0;
                bachecaList.IsRefreshing = false;

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
