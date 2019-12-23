using System;
using System.Collections.Generic;

using Xamarin.Forms;
#if __IOS__
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class ProfileView : ContentPage
    {
        public RestApi.Models.Utenti utente;

        public ProfileView(RestApi.Models.Utenti utente_)
        {
            InitializeComponent();

            //Load initial interface
            userInfo.User = utente_;
            utente = utente_;

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && !iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(25, 20, 25, 0);
            }
            //UIApplication.SharedApplication.StatusBarHidden = true;
#endif

            compleannoLbl.Text = utente.Compleanno.ToString("dd MMMM");
            comuneLbl.Text = utente.Residenza;
            if(utente.Stato > 0)
            {
                roleLayout.IsVisible = true;
                switch (utente.Stato)
                {
                    case 1:roleLbl.Text = "Rappresentante di classe";break;
                    case 2:roleLbl.Text = "VIP";break;
                    case 3:roleLbl.Text = "Creatore SalveminiApp"; break;
                }
            }
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Download orario
            var data = await App.Orari.GetOrario(utente.classeCorso);

            //Failed to get
            if (data.Message != null)
            {
                return;
            }

            //Set null if error occourred
            if (data.Data == null) { orario.Lezioni = new List<RestApi.Models.Lezione>(); return; }

            //Update lezioni
            orario.Lezioni = data.Data as List<RestApi.Models.Lezione>;

        }

        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
#if __IOS__
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
#endif
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
    }
}
