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
        SignalR.FlappyHub Hub = new SignalR.FlappyHub();

        public Extra()
        {
            InitializeComponent();
#if __IOS__
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && !iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 20, 0, 0);
            }
#endif
            MessagingCenter.Subscribe<App>(this, "AlertGroup", (sender) =>
            {
                i += 10;
                lbl.TranslateTo(i, i, 100);
            });

            MessagingCenter.Subscribe<App, float>(this, "UpdateSlider", (sender, y) =>
            {
                S2.Value = y;
            });
        }

        void Signal(object sender, EventArgs e)
        {
            Hub.creaGruppo("pile");
        }

        void SendValue(object sender, ValueChangedEventArgs e)
        {
            var fix1 = Math.Round(e.OldValue, 1);
            var fix2= Math.Round(e.NewValue, 1);
            if (fix1 != fix2)
            {
                Hub.sendY((float)S1.Value, "pile");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            secondLayout.Children.RemoveAt(1);
            //Ios 13 bug
            try
            {
                
                Navigation.PopModalAsync();
            }
            catch
            {
                //fa nient
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            secondLayout.Children.Insert(1, new Helpers.CountDown());

            Hub.InitilizeHub();
        }

        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
