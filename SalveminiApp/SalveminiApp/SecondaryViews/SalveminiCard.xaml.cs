using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using System.Diagnostics;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class SalveminiCard : ContentPage
    {
        public List<RestApi.Models.Offerte> offerte = new List<RestApi.Models.Offerte>();
        public double cardInitialHeight;

        public SalveminiCard()
        {
            InitializeComponent();

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif

            if (Barrel.Current.Exists("cardofferte"))
            {
                offerte = Barrel.Current.Get<List<RestApi.Models.Offerte>>("cardofferte");
                offersList.ItemsSource = offerte;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Animation
           await card.RotateYTo(0,600,Easing.BounceOut);

            //Download offerte
            if(Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var offerte_ = await App.Card.GetOfferte();
                if(offerte_.Count < 1)
                {
                    //Non possibile caricare nuove offerte
                }
                else
                {
                    offersList.ItemsSource = offerte_;
                }
            }
            else
            {
                //Notify no internet
            }

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Ios 13 bug
            try
            {
                Navigation.PopModalAsync();
            }
            catch
            {
                //fa nient
            }
        }

        async void offerSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            offersList.SelectedItem = null;

            var data = (sender as Xamarin.Forms.ListView).SelectedItem as RestApi.Models.Offerte;
            await DisplayAlert(data.Nome, data.Descrizione, "Ok");
        }


            void closePage (object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }


        public void listScroll(object sender, ScrolledEventArgs e)
        {
            Debug.WriteLine(e.ScrollY);

            card.RotationX = 0 + e.ScrollY;
            if (card.RotationX >= 90)
                card.RotationX = 90;

            offersList.HeightRequest = offersList.Height + e.ScrollY;

        }
    }
}
