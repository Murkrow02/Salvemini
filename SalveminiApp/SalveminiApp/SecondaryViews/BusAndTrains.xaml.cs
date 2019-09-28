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

        void Trains_Tapped (object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new BusETreni.Treni());
        }
    }
}
