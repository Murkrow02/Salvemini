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
        public Extra()
        {
            InitializeComponent();
#if __IOS__
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && !iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 20, 0, 0);
            }
#endif
            //Random
            widgetsLayout.Children.Add(new Controls.ExtraWidget("Estrai", "Sorteggia casualmente tra i tuoi compagni di classe, utile per programmare interrogazioni o assegnare dei posti per un compito", "dices", Color.FromHex("E91D27"), Color.FromHex("BF1113"), new RandomExtract()));

            //Flappy mimmo
            widgetsLayout.Children.Add(new Controls.ExtraWidget("Flappy Mimmo", "Remake di flappy bird ma con un panino di Mimmo", "flappy", Color.FromHex("FEB100"), Color.FromHex("F98D00"), new RandomExtract()));

        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            countDown.StartCountDown = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Show countdown animation
            countDown.StartCountDown = true;

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

            if(Preferences.Get("CountHolidays", false))
                noFestivi.IsVisible = false;


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
