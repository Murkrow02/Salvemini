using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.ArgoPages
{
    public partial class Promemoria : ContentPage
    {
        public List<RestApi.Models.Promemoria> Promemorias = new List<RestApi.Models.Promemoria>();

        public Promemoria()
        {
            InitializeComponent();

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            promemoriaList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            if (Barrel.Current.Exists("Promemoria"))
            {
                Promemorias = Barrel.Current.Get<List<RestApi.Models.Promemoria>>("Promemoria");
                promemoriaList.ItemsSource = Promemorias;
            }

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //Api Call
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Promemorias = await App.Argo.GetPromemoria();

                //Fill List
                promemoriaList.ItemsSource = Promemorias;
            }

        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
