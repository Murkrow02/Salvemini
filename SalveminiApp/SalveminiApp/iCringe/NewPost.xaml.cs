using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarcTron.Plugin;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.iCringe
{
    public partial class NewPost : PopupPage
    {

        public NewPost()
        {
            InitializeComponent();
            CrossMTAdmob.Current.LoadInterstitial(AdsHelper.InterstitialId());
        }

        public async void send_Clicked(object sender, EventArgs e)
        {
            //Check internet
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            if (string.IsNullOrEmpty(domanda.Text))
            {
                Costants.showToast("Scrivi qualcosa!");
                return;
            }

            //Show loading
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;
            sendBtn.IsEnabled = false;

            //Post question
            var response = await App.Cringe.PostDomanda(domanda.Text, switchAnonimo.IsToggled);
            if (response[0] == "Successo")
            {
                await DisplayAlert("Successo", response[1], "Ok");
                await Navigation.PopPopupAsync();

                //Show ad
                await showAd();
            }
            else
            {
                Costants.showToast(response[1]);
            }

            //Hide loading
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
            sendBtn.IsEnabled = true;
        }

        async Task showAd()
        {
            CrossMTAdmob.Current.ShowInterstitial();
        }

    }
}
