using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp
{
    public partial class ArgoPage : ContentPage
    {
        public ArgoPage()
        {
            InitializeComponent();

            //Set Sizes
            assenzeIcon.WidthRequest = App.ScreenWidth / 3.8;
            promemoriaIcon.WidthRequest = App.ScreenWidth / 3.8;
        }

        void Assenze_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.Assenze());
        }

        void Promemoria_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.Promemoria());
        }
    }
}
