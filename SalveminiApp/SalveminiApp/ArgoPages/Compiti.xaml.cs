using System;
using System.Collections.Generic;
using Forms9Patch;
using Rg.Plugins.Popup;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif

namespace SalveminiApp.ArgoPages
{
    public partial class Compiti : ContentPage
    {

        public Compiti()
        {
            InitializeComponent();
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif
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
            var firstPopUp = new Helpers.PopOvers().defaultPopOver;
            firstPopUp.Content = new Xamarin.Forms.Label { Text = "Clicca per visualizzare" + Environment.NewLine + "tutti i voti", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            firstPopUp.IsVisible = true;
            firstPopUp.PointerDirection = PointerDirection.Up;
            firstPopUp.PreferredPointerDirection = PointerDirection.Up;
            firstPopUp.Target = clockButton;
            firstPopUp.BackgroundColor = Color.FromHex("#00D10D");
            firstPopUp.Disappearing += Second_PoUp;
        }

        private void Second_PoUp(object sender, EventArgs e)
        {
           
        }



    }
}
