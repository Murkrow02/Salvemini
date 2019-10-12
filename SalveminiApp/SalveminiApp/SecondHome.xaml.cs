using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
using Xamarin.Forms;

namespace SalveminiApp
{
    public partial class SecondHome : ContentPage
    {
        public List<RestApi.Models.Pentagono> Medie = new List<RestApi.Models.Pentagono>();

        public SecondHome()
        {
            InitializeComponent();

            if (Barrel.Current.Exists("Medie"))
            {
                Medie = Barrel.Current.Get<List<RestApi.Models.Pentagono>>("Medie");
                radarChart.ItemsSource = Medie;
            }


            //Set Sizes
            chart.WidthRequest = App.ScreenWidth;
            chart.HeightRequest = App.ScreenHeight / 3;
            cardImage.WidthRequest = App.ScreenWidth / 7.5;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var dates = await App.Argo.GetPentagono();

            if (!string.IsNullOrEmpty(dates.Message))
            {

            }
            else
            {
                Medie = dates.Data as List<RestApi.Models.Pentagono>;
            }

            if (Medie.Count >= 3)
            {
                chart.IsVisible = true;
                noSubjectsLayout.IsVisible = false;
                radarChart.ItemsSource = Medie;
            }
            else
            {
                chart.IsVisible = false;
                noSubjectsLayout.IsVisible = true;
            }

        }

        void Card_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new SecondaryViews.SalveminiCard());
        }
        }
}
