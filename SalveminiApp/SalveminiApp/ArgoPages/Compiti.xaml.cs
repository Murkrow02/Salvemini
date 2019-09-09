using System;
using System.Collections.Generic;
using Forms9Patch;
using Rg.Plugins.Popup;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;


namespace SalveminiApp.ArgoPages
{
    public partial class Compiti : ContentPage
    {

        public Compiti()
        {
            InitializeComponent();

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            //compitiList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Tips (falli vedere solo la prima volta)
            var firstPopUp = new Helpers.PopOvers().compitiPopOver;
            firstPopUp.Content = new Xamarin.Forms.Label { Text = "Clicca per visualizzare" + Environment.NewLine + "tutti i voti", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            firstPopUp.IsVisible = true;
            firstPopUp.Target = clockButton;
            firstPopUp.BackgroundColor = Color.FromHex("#00D10D");
        }

       
    }
}
