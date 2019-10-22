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
            Navigation.PushModalAsync(new BusETreni.Treni());
        }

        void bus_tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new BusETreni.Treni());
        }

        void ali_tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new BusETreni.Treni());
        }

        void changePref_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FirstAccess.OrariTrasporti());
        }
    }
}
