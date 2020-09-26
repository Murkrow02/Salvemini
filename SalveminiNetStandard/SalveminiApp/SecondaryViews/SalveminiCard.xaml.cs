using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using System.Diagnostics;
using System.Threading.Tasks;
namespace SalveminiApp.SecondaryViews
{
    public partial class SalveminiCard : ContentPage
    {
        public List<RestApi.Models.Offerte> offerte = new List<RestApi.Models.Offerte>();
        public double cardInitialHeight;

        public SalveminiCard()
        {
            InitializeComponent();
            lilCard.Opacity = 0;

            DependencyService.Get<IPlatformSpecific>().SetSafeArea(this);
            if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
                mainLayout.Padding = new Thickness(25, 20, 25, 0);



            //Get cached offerte
            offerte = CacheHelper.GetCache<List<RestApi.Models.Offerte>>("cardofferte");
            if (offerte != null)
                offersList.ItemsSource = offerte;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Start refereshing
            offersList.IsRefreshing = true;

            //Download offers in background
            await Task.Run((Action)DownloadOfferte);

            //Animation
            await card.RotateYTo(0, 800, Easing.BounceOut);

        }


        async void DownloadOfferte()
        {
            //Check connection
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Download offerte
                offerte = await App.Card.GetOfferte();

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (offerte.Count < 1)
                    {
                        //Non possibile caricare nuove offerte
                    }
                    else
                    {
                        offersList.ItemsSource = offerte;
                        offersList.IsRefreshing = false;
                    }
                });

            }
            else //No connection
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Costants.showToast("connection");
                    offersList.IsRefreshing = false;
                });
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }

        //Open alert on offer selcted
        async void offerSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            offersList.SelectedItem = null;

            var data = (sender as Xamarin.Forms.ListView).SelectedItem as RestApi.Models.Offerte;
            await DisplayAlert(data.Nome, data.Descrizione, "Ok");
        }


        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private double previousScrollPosition = 0;
        public bool animating = false;
        public async void listScroll(object sender, ScrolledEventArgs e)
        {
            Debug.WriteLine(e.ScrollY);

            if (previousScrollPosition < e.ScrollY)
            {
                //scrolled up
                if ((int)e.ScrollY > (int)card.Height && !animating)
                {
                    animating = true;
                    await lilCard.FadeTo(1, 300, Easing.CubicInOut);
                    animating = false;
                }
            }
            else
            {
                //scrolled down
                if ((int)e.ScrollY < (int)card.Height && !animating)
                {
                    animating = true;
                    await lilCard.FadeTo(0, 300, Easing.CubicInOut);
                    animating = false;
                }
            }
            previousScrollPosition = e.ScrollY;

        }
    }
}
