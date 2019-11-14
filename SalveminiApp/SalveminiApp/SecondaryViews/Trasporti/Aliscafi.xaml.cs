using System;
using System.Collections.Generic;
using Plugin.Iconize;
using Xamarin.Forms;
using System.Linq;
#if __IOS__
using UIKit;
#endif

namespace SalveminiApp.SecondaryViews.Trasporti
{
    public partial class Aliscafi : ContentPage
    {
        public string staticLink = "https://www.naplesbayferry.com/it/t/";
        public Aliscafi()
        {
            InitializeComponent();
#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            }
#endif
            //Set ItemsSource
            fromPicker.ItemsSource = new List<string>(Costants.Rotte.Keys);

            //Set Padding for Notch Devices
#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(20, 50);
            }
#endif
        }


        private void fromPicker_Unfocused(object sender, FocusEventArgs e)
        {
            //Set ItemsSource to destination picker
            if (fromPicker.SelectedItem != null)
            {
                var Destinations = Costants.Rotte[fromPicker.SelectedItem.ToString()];
                toPicker.ItemsSource = Destinations;
                if (Destinations.ToList().Count() == 1)
                {
                    toPicker.SelectedItem = Destinations[0];
                }
                toFrame.FadeTo(1, 200);
                toPicker.IsEnabled = true;
            }

            if (fromPicker.SelectedItem != null && toPicker.SelectedItem != null)
            {
                //Create Link
                webView.Source = staticLink + fromPicker.SelectedItem.ToString() + "/" + toPicker.SelectedItem.ToString();

                //Hide Placeholder and show List
                emptyLayout.IsVisible = false;
                webView.IsVisible = true;

            }
            else
            {
                //Show Placeholder and hide WebView
                emptyLayout.IsVisible = true;
                webView.IsVisible = false;
                expandButton.IsVisible = false;
            }
        }

        private void toPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (fromPicker.SelectedItem != null && toPicker.SelectedItem != null)
            {
                //Create Link
                webView.Source = staticLink + fromPicker.SelectedItem.ToString().Replace(" ", "-") + "/" + toPicker.SelectedItem.ToString().Replace(" ", "-");

                //Hide Placeholder and show WebView
                emptyLayout.IsVisible = false;
                webView.IsVisible = true;
            }
            else
            {
                //Show Placeholder and hide WebView
                emptyLayout.IsVisible = true;
                webView.IsVisible = false;
                expandButton.IsVisible = false;
            }
        }


        private void WebView_Navigated(object sender, WebNavigatingEventArgs e)
        {
            indicator.IsRunning = false;
            expandButton.IsVisible = true;
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            indicator.IsRunning = true;
        }

        void expand_Clicked(object sender, EventArgs e)
        {
            var page = new IconNavigationPage(new ContentPage { Title = fromPicker.SelectedItem.ToString() + "-" + toPicker.SelectedItem.ToString(), Content = new WebView { Source = staticLink + fromPicker.SelectedItem.ToString().Replace(" ", "-") + "/" + toPicker.SelectedItem.ToString().Replace(" ", "-") } }) { BarBackgroundColor = Color.FromHex("766AFF"), BarTextColor = Color.White };
            page.ToolbarItems.Add(new IconToolbarItem { IconImageSource = "fas-times", IconColor = Styles.TextColor });
            page.ToolbarItems[0].Clicked += WebClose_Clicked;
            Navigation.PushModalAsync(page);
        }

        private void WebClose_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void Close_Page(object sender, EventArgs e)
        {
            //Close the page
            Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
#if __IOS__
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
#endif
        }
    }
}
