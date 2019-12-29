using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class BusAndTrains : ContentPage
    {
        public BusAndTrains()
        {
            InitializeComponent();
        }

        void treni_tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Trasporti.Treni());
        }

        void bus_tapped(object sender, EventArgs e)
        {
            //DisplayAlert("Attenzione", "Gli orari dei bus non sono ancora pronti", "Ok");
            Navigation.PushModalAsync(new Trasporti.Bus());
        }

        void ali_tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Trasporti.Aliscafi());
        }

        void changePref_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FirstAccess.OrariTrasporti());
        }
    }
}
