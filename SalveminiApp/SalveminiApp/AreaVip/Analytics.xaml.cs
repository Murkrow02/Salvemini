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
            int accessi = 0;
            var analytics = await App.Analytics.GetAnalytics();
            var questoMese = analytics.Where(x => x.Giorno.Month == DateTime.Today.Month && x.Tipo == "Accessi").ToList();

            foreach(RestApi.Models.Analytics accesso in analytics)
            {
                accessi += accesso.Valore;
            }

            accessiLbl.Text = "Questo mese ci sono stati " + accessi.ToString() + " accessi";

        }
    }
}
