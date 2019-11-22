using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class Analytics : ContentPage
    {
        public Analytics()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int accessiCount = 0;
            int avvisiCount = 0;

            //Show loading
            loading.IsRunning = true;

            //Download data
            var analytics = await App.Analytics.GetAnalytics();

            //Take accessi count
            var accessi = analytics.Where(x => x.Tipo == "Accessi").ToList();

            //Take last avviso visual count
            var avvisi = analytics.Where(x => x.Tipo == "UltimoAvviso").ToList();

            //Show in label
            if (accessi.Count > 0)
                accessiLbl.Text = "Accessi totali: " + accessi[0].Valore;

            if (avvisi.Count > 0)
                avvisiLbl.Text = "Visualizzazioni ultimo avviso: " + avvisi[0].Valore;

            loading.IsRunning = false;

        }
    }
}
