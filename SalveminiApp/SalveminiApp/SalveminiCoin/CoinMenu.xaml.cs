using System;
using System.Collections.Generic;
using Xamarin.Forms;

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

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
        }

        void Events_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new RedeemCode());
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
