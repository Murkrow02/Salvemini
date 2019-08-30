using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Rg.Plugins.Popup.Extensions;

namespace SalveminiApp.ArgoPages
{
    public partial class Assenze : ContentPage
    {
        public List<RestApi.Models.Assenza> Assenzes = new List<RestApi.Models.Assenza>();

        public Assenze()
        {
            InitializeComponent();

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            assenzeList.HeightRequest = App.ScreenHeight / 1.8;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
            infoImage.HeightRequest = App.ScreenWidth / 13;
            infoImage.WidthRequest = App.ScreenWidth / 13;

            //Load Cache
            if (Barrel.Current.Exists("Assenze"))
            {
                Assenzes = Barrel.Current.Get<List<RestApi.Models.Assenza>>("Assenze");
                assenzeList.ItemsSource = Assenzes;
                loadInfo();
            }

            //Messaging Centers
            MessagingCenter.Subscribe<App>(this, "ReloadAssenze", (sender) =>
           {
               OnAppearing();
           });
        }

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

        protected async override void OnAppearing()
        {
            base.OnAppearing();
           
            //Api Call
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var response = await App.Argo.GetAssenze();

                if (!string.IsNullOrEmpty(response.Message))
                {

                }
                else
                {
                    Assenzes = response.Data as List<RestApi.Models.Assenza>;
                }
                //Fill List
                assenzeList.ItemsSource = Assenzes;
            }

            //Fill Infos
            loadInfo();
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
