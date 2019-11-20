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

        public Bacheca()
        {
            InitializeComponent();

            //Hide large titles (useless)
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
            if (iOS.AppDelegate.HasNotch)
                fullLayout.Padding = new Thickness(20, 35, 20, 25);
#endif

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            bachecaList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            var cachedBacheca = CacheHelper.GetCache<List<RestApi.Models.Bacheca>>("Bacheca");
            if (cachedBacheca != null)
            {
                Bacheche = cachedBacheca;
                bachecaList.ItemsSource = Bacheche;
                emptyLayout.IsVisible = Bacheche.Count <= 0;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif
        }

        //Show avviso selected
        private async void Bacheca_Selected(object sender, SelectedItemChangedEventArgs e)
        {
            //Get selected item
            var data = e.SelectedItem as RestApi.Models.Bacheca;

            //Check internet connection
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (!data.presaVisione)
                {
                    var success = await App.Argo.VisualizzaBacheca(new RestApi.Models.VisualizzaBacheca { presaVisione = true, prgMessaggio = data.prgMessaggio });
                }
            }
            else //No connection
            {
                Costants.showToast("Non è stato possibile scaricare l'allegato, controlla la tua connessione e riprova");
                return;
            }

            try
            {
                //Android does not load pdf in web view, open view in default browser
#if __ANDROID__
                await Launcher.OpenAsync(new Uri("http://drive.google.com/viewer?url=" + (e.SelectedItem as RestApi.Models.Bacheca).Allegati[0].fullUrl));
                return;
#endif

                //Create page with webview
                var content = new ContentPage { Title = (e.SelectedItem as RestApi.Models.Bacheca).formattedTitle, Content = new WebView { Source = (e.SelectedItem as RestApi.Models.Bacheca).Allegati[0].fullUrl } };
                bool haftaClose = true;

                //Add toolbaritems to the page
                var barItem = new ToolbarItem { Text = "Chiudi", };
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
            catch
            {
                Costants.showToast("Non è stato possibile aprire l'allegato");
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();


            //Start loading
            bachecaList.IsRefreshing = true;

            //Check connection
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Download bacheca from api
                    var datas = await App.Argo.GetBacheca();

                    //Detect if call returned a message of error
                    if (!string.IsNullOrEmpty(datas.Message))
                    {
                        //Error occourred, notify the user
                        Costants.showToast(datas.Message);
                        //Stop loading list
                        bachecaList.IsRefreshing = false;
                        return;
                    }

                    //Deserialize new object
                    Bacheche = datas.Data as List<RestApi.Models.Bacheca>;

                    //Fill List
                    bachecaList.ItemsSource = Bacheche;
                    emptyLayout.IsVisible = Bacheche.Count <= 0;
                    bachecaList.IsRefreshing = false;
                }
                catch //Random error
                {
                    Costants.showToast("Non è stato possibile aggiornare la bacheca");
                }
            }
            else //No connection
            {
                Costants.showToast("connection");
            }
            bachecaList.IsRefreshing = false;
        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
