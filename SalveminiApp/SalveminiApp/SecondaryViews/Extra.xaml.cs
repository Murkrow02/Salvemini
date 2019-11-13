﻿using System;
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

        }

        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
