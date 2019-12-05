using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;

#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif

namespace SalveminiApp.SalveminiCoin
{
    public partial class CoinMenu : ContentPage
    {
        public CoinMenu()
        {
            InitializeComponent();

            //Modal presentation style
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif

            //Set sizes
            coinImage.WidthRequest = App.ScreenWidth * 0.45;
            coinImage.HeightRequest = App.ScreenWidth * 0.45;

            //Create tapped gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            //Set children
            //Guadagna
            var codeButton = new Helpers.PushCell { Title = "Riscatta codice", IsEnabled = true, Separator = "si", Push = new RedeemCode() };
            codeButton.GestureRecognizers.Add(tapGestureRecognizer);
            gainLayout.Children.Add(codeButton);
            var gainButton = new Helpers.PushCell { Title = "Ottieni sCoins gratis", IsEnabled = true, Separator = "no" };
            gainButton.GestureRecognizers.Add(tapGestureRecognizer);
            gainLayout.Children.Add(gainButton);

            //Cronologia
            var activeButton = new Helpers.PushCell { Title = "Codici attivati", IsEnabled = true, Separator = "si", Push = new AreaVip.ListaEventi(false) };
            activeButton.GestureRecognizers.Add(tapGestureRecognizer);
            historyLayout.Children.Add(activeButton);
            var prizeButton = new Helpers.PushCell { Title = "Premi riscattati", IsEnabled = true, Separator = "no" };
            prizeButton.GestureRecognizers.Add(tapGestureRecognizer);
            historyLayout.Children.Add(prizeButton);
        }

        //Handle cell tapped
        async private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                //Get push page from model
                var cell = sender as Helpers.PushCell;

                //Push to selected page
                if (cell.Push != null)
                {
                    //Push
                    await Navigation.PushModalAsync(cell.Push); 
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //Ios 13 bug
            try
            {
#if __IOS__
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    Navigation.PopModalAsync();
                }
#endif
            }
            catch
            {
                //fa nient
            }
        }
    }
}
