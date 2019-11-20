using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;
//using MarcTron.Plugin;
namespace SalveminiApp.SecondaryViews
{
    public partial class SalveminiCoin : ContentPage
    {


        public SalveminiCoin()
        {
            InitializeComponent();

            //Initialize ads
            //setAdEvents();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Load ad video
            //CrossMTAdmob.Current.LoadRewardedVideo(Costants.RewardId());
        }

        //public async void adShow(object sender, EventArgs e)
        //{
        //    showRewardBtn.IsEnabled = false;

        //    //Reload reward video
        //    if (!CrossMTAdmob.Current.IsRewardedVideoLoaded())
        //        CrossMTAdmob.Current.LoadRewardedVideo(Costants.RewardId());

        //    //Check if he can watch a reward video
        //    var canWatch = await App.Ads.canWatchAd();
        //    bool success = Convert.ToBoolean(canWatch.Data);

        //    //Error in the API call
        //    if (!string.IsNullOrEmpty(canWatch.Message) || !success)
        //    {
        //        await DisplayAlert("Attenzione", canWatch.Message, "Ok");
        //        showRewardBtn.IsEnabled = true;
        //        return;
        //    }

        //    //Success

        //    //Show video
        //    CrossMTAdmob.Current.ShowRewardedVideo();

        //    showRewardBtn.IsEnabled = true;
        //}

        //private async void Current_OnRewarded(object sender, MTEventArgs e)
        //{
        //    //Try adding coin to user account
        //    var canWatch = await App.Ads.watchedAd();
        //    bool success = Convert.ToBoolean(canWatch.Data);

        //    //Error in the API call
        //    if (!string.IsNullOrEmpty(canWatch.Message) || !success)
        //    {
        //        await DisplayAlert("Attenzione", canWatch.Message, "Ok");
        //        return;
        //    }

        //    //Show success
        //    await DisplayAlert("Grande", "La sCoin è stata aggiunta al tuo account", "Chiudi");
        //}










        //#region Events from Ads

        //public void setAdEvents()
        //{
        //    CrossMTAdmob.Current.OnRewardedVideoStarted += Current_OnRewardedVideoStarted;
        //    CrossMTAdmob.Current.OnRewarded += Current_OnRewarded;
        //    CrossMTAdmob.Current.OnRewardedVideoAdClosed += Current_OnRewardedVideoAdClosed;
        //    CrossMTAdmob.Current.OnRewardedVideoAdFailedToLoad += Current_OnRewardedVideoAdFailedToLoad;
        //    CrossMTAdmob.Current.OnRewardedVideoAdLeftApplication += Current_OnRewardedVideoAdLeftApplication;
        //    CrossMTAdmob.Current.OnRewardedVideoAdLoaded += Current_OnRewardedVideoAdLoaded;
        //    CrossMTAdmob.Current.OnRewardedVideoAdOpened += Current_OnRewardedVideoAdOpened;
        //    CrossMTAdmob.Current.OnRewardedVideoAdCompleted += Current_OnRewardedVideoAdCompleted;
        //}


        //private void Current_OnRewardedVideoAdOpened(object sender, EventArgs e)
        //{
        //    Debug.WriteLine("OnRewardedVideoAdOpened");
        //}

        //private void Current_OnRewardedVideoAdLoaded(object sender, EventArgs e)
        //{
        //    Debug.WriteLine("OnRewardedVideoAdLoaded");

        //}

        //private void Current_OnRewardedVideoAdLeftApplication(object sender, EventArgs e)
        //{
        //    Debug.WriteLine("OnRewardedVideoAdLeftApplication");
        //}

        //private void Current_OnRewardedVideoAdFailedToLoad(object sender, MTEventArgs e)
        //{
        //    Debug.WriteLine("OnRewardedVideoAdFailedToLoad");
        //}

        //private void Current_OnRewardedVideoAdClosed(object sender, EventArgs e)
        //{
        //    Debug.WriteLine("OnRewardedVideoAdClosed");
        //}


        //private void Current_OnRewardedVideoStarted(object sender, EventArgs e)
        //{
        //    Debug.WriteLine("OnRewardedVideoStarted");
        //}

        //private void Current_OnRewardedVideoAdCompleted(object sender, EventArgs e)
        //{
        //    Debug.WriteLine("OnRewardedVideoAdCompleted");
        //}


        //#endregion
    }
}
