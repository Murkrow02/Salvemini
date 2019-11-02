using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using MonkeyCache.SQLite;
using System.IO;
using System.Linq;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
using Foundation;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class SettingsPanel : PopupPage
    {


        public SettingsPanel()
        {
            InitializeComponent();
            //Navigation Animation

            //Set Interface
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 25);
            }
#endif
            SystemPaddingSides = 0;
            frameBackground.TranslationX = App.ScreenWidth * 2;
            frameBackground.HeightRequest = App.ScreenHeight * 2;
            frameBackground.WidthRequest = App.ScreenWidth / 1.7;
            if (Preferences.Get("Token", "Guest") == "Guest")
            {
                logoutLbl.Text = "Login";
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            frameBackground.TranslateTo(0, 0, 400, Easing.CubicOut);
        }

        void Account_Tapped(object sender, System.EventArgs e)
        {
            SecondaryViews.Profile UserPage = new Profile();
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif
            Navigation.PushModalAsync(UserPage);
            closePopUp();
        }

        async void Logout_Tapped(object sender, System.EventArgs e)
        {
            //Perform logout actions
            await this.FadeTo(0, 300);

            //Delete Cache
            var storage = new Helpers.GetStorageInfo();
            await storage.storageInfo(true);

            //Set token to guest
            Preferences.Set("Token", "Guest");

            //Push to Login
            Navigation.PopPopupAsync();
            Navigation.PushModalAsync(new FirstAccess.Login());
        }

        void Handle_BackgroundClicked(object sender, System.EventArgs e)
        {
            closePopUp();
        }
        void Contact_Clicked(object sender, System.EventArgs e)
        {
            //Navigation.PopPopupAsync();
            //Navigation.PushModalAsync(new SecondaryViews.ContactPage());
        }
        void Notifications_Clicked(object sender, System.EventArgs e)
        {
            //Navigation.PopPopupAsync();
            //Navigation.PushModalAsync(new SecondaryViews.NotificationPage());
        }

        void Archivio_Tapped(object sender, System.EventArgs e)
        {
            //Navigation.PopPopupAsync();
            //Navigation.PushModalAsync(new SecondaryViews.ArchivioPage());
        }

        void Info_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PopPopupAsync();
            try
            {
                Device.OpenUri(new Uri("http://codexdevelopment.net"));
            }
            catch { }
        }

        async void Share_Tapped(object sender, System.EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = "https://www.google.com",
                Title = "Condividi sito web"
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
#if __IOS__
            UIApplication.SharedApplication.StatusBarHidden = false;
#endif
        }

        async void closePopUp()
        {
            await frameBackground.TranslateTo(App.ScreenWidth * 2, 0, 400, Easing.CubicIn);
            await Navigation.RemovePopupPageAsync(this);
        }
    }
}
