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
            var accessi = analytics.FirstOrDefault(x => x.Tipo == "Accessi");

            //Take last avviso visual count
            var avvisi = analytics.FirstOrDefault(x => x.Tipo == "UltimoAvviso");

            //Take user count
            var utenti = analytics.FirstOrDefault(x => x.Tipo == "UtentiCount");

            //Show in label
            if (accessi != null)
                accessiLbl.Text = "Accessi totali: " + accessi.Valore;

            if (accessi != null)
                avvisiLbl.Text = "Visualizzazioni ultimo avviso: " + avvisi.Valore;

            if (accessi != null)
                utentiLbl.Text = "Utenti: " + utenti.Valore;

            loading.IsRunning = false;

        }
    }
}
