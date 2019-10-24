using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
using Xamarin.Forms;

namespace SalveminiApp
{
    public partial class SecondHome : ContentPage
    {

        public SecondHome()
        {
            InitializeComponent();

            


        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            

        }

        void Card_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new SecondaryViews.SalveminiCard());
        }
        }
}
