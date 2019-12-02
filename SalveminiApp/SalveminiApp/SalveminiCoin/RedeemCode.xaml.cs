using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.SalveminiCoin
{
    public partial class RedeemCode : ContentPage
    {
        public RedeemCode()
        {
            InitializeComponent();

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
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

        private void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void Confirm_Clicked(object sender, EventArgs e)
        {
            await Task.WhenAll(arrowLabel.FadeTo(0, 300), bgArrowFrame.ColorTo(Color.FromHex("#E3E3E3"), Color.FromHex("#529FFF"), x => bgArrowFrame.BackgroundColor = x, 500), exitButton.ColorTo(exitButton.BackgroundColor, Color.FromHex("#529FFF"), x => exitButton.BackgroundColor = x, 500));
            arrowLabel.Text = "check";
            exitButton.Text = "Fatto";
            arrowLabel.TextColor = Color.White;
            await arrowLabel.FadeTo(1, 300);
        }

        private void Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Check if lenght of code is 6 numbers
            if (e.NewTextValue.Length == 6)
            {
                confirmButton.IsEnabled = true;
            }
            else
            {
                confirmButton.IsEnabled = false;
            }
        }
    }
}
