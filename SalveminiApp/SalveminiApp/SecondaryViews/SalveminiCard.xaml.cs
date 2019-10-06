using System;
using System.Collections.Generic;
using Xamarin.Forms;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class SalveminiCard : ContentPage
    {
        public SalveminiCard()
        {
            InitializeComponent();

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            UIApplication.SharedApplication.StatusBarHidden = true;

#endif
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           await card.RotateYTo(0,600,Easing.BounceOut);
        }

        void closePage (object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
