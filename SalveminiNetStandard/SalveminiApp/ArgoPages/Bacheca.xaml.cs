﻿using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
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

            //Custom padding for notch devices
            if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
                fullLayout.Padding = new Thickness(20, 35, 20, 25);

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            bachecaList.HeightRequest = App.ScreenHeight / 1.8;
            //buttonFrame.WidthRequest = App.ScreenWidth / 6;
            //buttonFrame.HeightRequest = App.ScreenWidth / 6;
            //buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

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

            if ((e.SelectedItem as RestApi.Models.Bacheca).Allegati.Count == 0)
            {
                return;
            }

            //Openpdf
            var title = (e.SelectedItem as RestApi.Models.Bacheca).formattedTitle;
            var link = (e.SelectedItem as RestApi.Models.Bacheca).Allegati[0].fullUrl;
            Costants.OpenPdf(link, title);
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

        public void updateList(object sender, EventArgs e)
        {
            OnAppearing();
        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
