using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.SalveminiCoin
{
    public partial class Transazioni : ContentPage
    {
        public Transazioni()
        {
            InitializeComponent();

            //Get cache
            eventsList.ItemsSource = CacheHelper.GetCache<List<RestApi.Models.Transaction>>("transazioni");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection"); return;
            }

            //Download eventi
            eventsList.IsRefreshing = true;
            var eventi = new List<RestApi.Models.Transaction>();

            //Download events
            eventi = await App.Coins.Transazioni();


            //Show result
            eventsList.IsRefreshing = false;
            if (eventi == null)
            {
                Costants.showToast("Non è stato possibile scaricare i tuoi movimenti");
                return;
            }
            eventsList.ItemsSource = eventi;
        }
    }
}
