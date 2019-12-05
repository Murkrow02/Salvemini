using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif

namespace SalveminiApp.SalveminiCoin
{
    public partial class HandleEvent : ContentPage
    {
        public HandleEvent()
        {
            InitializeComponent();

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
            //Add radar
            var radarView = new Controls.RadarView(100, new Location { Latitude = 40.626893, Longitude = 14.374786 });
            mainLayout.Children.Add(radarView);

            //Add person
            radarView.addPerson(new Location { Latitude = 40.627182, Longitude = 14.375597 });
            radarView.addPerson(new Location { Latitude = 40.627542, Longitude = 14.374744 });
            radarView.addPerson(new Location { Latitude = 40.627000, Longitude = 14.373774 });

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

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
