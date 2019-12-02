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
    public partial class Assenze : ContentPage
    {
        public List<RestApi.Models.Assenza> Assenzes = new List<RestApi.Models.Assenza>();

        public Assenze()
        {
            InitializeComponent();

#if __IOS__
           
            if (iOS.AppDelegate.HasNotch)
                fullLayout.Padding = new Thickness(20, 35, 20, 25);
#endif
            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            assenzeList.HeightRequest = App.ScreenHeight / 1.8;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            var cachedAssenze = CacheHelper.GetCache<List<RestApi.Models.Assenza>>("Assenze");
            if (cachedAssenze != null)
            {
                Assenzes = cachedAssenze;
                assenzeList.ItemsSource = Assenzes;
                emptyLayout.IsVisible = Assenzes.Count <= 0;
                loadInfo();
            }

            //Messaging Centers
            MessagingCenter.Subscribe<App>(this, "ReloadAssenze", (sender) =>
           {
               OnAppearing();
           });
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Show loading
            assenzeList.IsRefreshing = true;

            //Check connection status
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Get assenze from api
                    var response = await App.Argo.GetAssenze();

                    //Detect if call returned a message of error
                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        //Error occourred, notify the user
                        Costants.showToast(response.Message);
                        //Stop loading list
                        assenzeList.IsRefreshing = false;
                        return;
                    }

                    Assenzes = response.Data as List<RestApi.Models.Assenza>;

                    //Fill assnze list
                    assenzeList.ItemsSource = Assenzes;
                    emptyLayout.IsVisible = Assenzes.Count <= 0;
                    loadInfo();
                }
                catch //Random error
                {
                    Costants.showToast("Non è stato possibile aggiornare le assenze");
                }
            }
            else //No connection
            {
                Costants.showToast("connection");
            }

            //Stop loading list
            assenzeList.IsRefreshing = false;
        }

        //Show bottom label with assenze ritardi e uscite count
        void loadInfo()
        {
            //Load Counters
            int assenzeCount = Assenzes.Count(x => x.codEvento == "A");
            assenzeLabel.Text = assenzeCount == 1 ? assenzeCount.ToString() + " Assenza" : assenzeCount.ToString() + " Assenze";
            int ritardiCount = Assenzes.Count(x => x.codEvento == "I");
            ritardiLabel.Text = ritardiCount == 1 ? ritardiCount.ToString() + " Ritardo" : ritardiCount.ToString() + " Ritardi";
            int usciteCount = Assenzes.Count(x => x.codEvento == "U");
            usciteLabel.Text = usciteCount == 1 ? usciteCount.ToString() + " Uscita" : usciteCount.ToString() + " Uscite";
        }


        void Assenza_Selected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect Animation
            if (e.SelectedItem == null)
                return;

            assenzeList.SelectedItem = null;

            //Push to giustifica if possible
            if (!(e.SelectedItem as RestApi.Models.Assenza).Giustificata)
            {
                Navigation.PushPopupAsync(new Helpers.Popups.GiustificaAssenza(e.SelectedItem as RestApi.Models.Assenza));
            }
        }

        //Show popup with legenda
        void Info_Tapped(object sender, System.EventArgs e)
        {
            var infoLayout = new StackLayout { HorizontalOptions = LayoutOptions.Center, Orientation = StackOrientation.Vertical, Spacing = 10 };
            var giustificaLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            giustificaLayout.Children.Add(new Frame { HasShadow = false, CornerRadius = (float)(App.ScreenWidth / 17) / 2, HeightRequest = App.ScreenWidth / 17, WidthRequest = App.ScreenWidth / 17, Padding = 0, BackgroundColor = Color.FromHex("#A9FA63"), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center });
            giustificaLayout.Children.Add(new Label { Text = "Indica un’assenza giustificata", FontSize = 15, VerticalOptions = LayoutOptions.Center });
            infoLayout.Children.Add(giustificaLayout);
            var nonGiustificaLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            nonGiustificaLayout.Children.Add(new Frame { HasShadow = false, CornerRadius = (float)(App.ScreenWidth / 17) / 2, HeightRequest = App.ScreenWidth / 17, WidthRequest = App.ScreenWidth / 17, Padding = 0, BackgroundColor = Color.FromHex("#FA6363"), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center });
            nonGiustificaLayout.Children.Add(new Label { Text = "Indica un’assenza non giustificata", FontSize = 15, VerticalOptions = LayoutOptions.Center });
            infoLayout.Children.Add(nonGiustificaLayout);
            Navigation.PushPopupAsync(new Helpers.Popups.BindablePopup("Info Assenze", "Assenze.svg", infoLayout));
        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            //Close Page
            Navigation.PopModalAsync();
        }
    }
}
