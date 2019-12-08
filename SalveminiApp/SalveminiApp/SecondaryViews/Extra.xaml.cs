using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
#if __IOS__
using UIKit;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class Extra : ContentPage
    {
        int i = 5;

        public Extra()
        {
            InitializeComponent();
#if __IOS__
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && !iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 20, 0, 0);
            }
#endif



        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            secondLayout.Children.RemoveAt(1);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Status bar color
#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            }
#endif
            secondLayout.Children.Insert(1, new Helpers.CountDown());

            string etaText = "A";
            switch (Preferences.Get("DateToPoint", new DateTime(2020, 6, 6, 13, 40, 0)).ToString("dd-MM-yyyy"))
            {
                case "25-12-2019":
                    //Natale
                    etaText += " Natale";
                    break;
                case "12-04-2020":
                    //Pasqua
                    etaText += " Pasqua";
                    break;
                default:
                    etaText += "lla fine della scuola";
                    break;
            }

            //Update EtaText
            wenEta.Text = etaText;

        }

        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void random_Clicked(object sender, EventArgs e)
        {
            //Navigation.PopModalAsync();
            Navigation.PushAsync(new RandomExtract());
        }
    }
}
