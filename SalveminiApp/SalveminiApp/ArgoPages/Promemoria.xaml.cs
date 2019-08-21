using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.ArgoPages
{
    public partial class Promemoria : ContentPage
    {
        public Promemoria()
        {
            InitializeComponent();

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            promemoriaList.HeightRequest = App.ScreenHeight / 2;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
