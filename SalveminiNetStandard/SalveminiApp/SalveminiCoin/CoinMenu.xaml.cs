using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;
using MarcTron.Plugin;
using MarcTron.Plugin.CustomEventArgs;
using Xamarin.Essentials;
namespace SalveminiApp.SalveminiCoin
{
    public partial class CoinMenu : ContentPage
    {
        public bool isShowingAd;
        public Helpers.PushCell gainButton;

        public CoinMenu()
        {
            InitializeComponent();

            //Initialize ads
            setAdEvents();

            ////Set sizes
            //coinImage.WidthRequest = App.ScreenWidth * 0.4;
            //coinImage.HeightRequest = App.ScreenWidth * 0.4;

            //Create tapped gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            //Set children
            //Guadagna
            var codeButton = new Helpers.PushCell { Title = "Riscatta codice", IsEnabled = true, Separator = "si", Push = new RedeemCode() };
            codeButton.GestureRecognizers.Add(tapGestureRecognizer);
            gainLayout.Children.Add(codeButton);
            gainButton = new Helpers.PushCell { Title = "Ottieni sCoins gratis", IsEnabled = true, Separator = "no" };
            gainButton.GestureRecognizers.Add(tapGestureRecognizer);
            gainLayout.Children.Add(gainButton);

            //Cronologia
            var activeButton = new Helpers.PushCell { Title = "Codici attivati", IsEnabled = true, Separator = "si", Push = new AreaVip.ListaEventi(false) };
            activeButton.GestureRecognizers.Add(tapGestureRecognizer);
            historyLayout.Children.Add(activeButton);
            var prizeButton = new Helpers.PushCell { Title = "Transazioni", IsEnabled = true, Separator = "no", Push = new Transazioni() };
            prizeButton.GestureRecognizers.Add(tapGestureRecognizer);
            historyLayout.Children.Add(prizeButton);
        }

        //Handle cell tapped
        async private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                //Get push page from model
                var cell = sender as Helpers.PushCell;

                //Push to selected page
                if (cell.Push != null)
                {
                    //Push
                    await Navigation.PushAsync(cell.Push);
                }
                if (cell.Title == "Ottieni sCoins gratis")
                {
                    MainPage.isSelectingImage = true;
                    adShow();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Prevent ios 13 modal bug
            MainPage.isSelectingImage = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                coinCount.Opacity = 0;
                return;
            }

            //Get user coins
            var coins = await App.Coins.UserCoins();
            if (coins == null)
            {
                coinCount.Opacity = 0;
                Costants.showToast("Non è stato possibile caricare le tue sCoin");
            }
            else
            {
                //Display new value
                coinCount.Text = coins.ToString();
                coinCount.FadeTo(1, 400);

                //Update home value
                MessagingCenter.Send<App,int?>((App)Xamarin.Forms.Application.Current, "UpdateCoins", coins);
            }

            //Load ad video
            gainButton.Loading = true;
            //CrossMTAdmob.Current.TestDevices = new List<string> { "60394458335F479CE5061737251AC261" }; Telefn e patm
            CrossMTAdmob.Current.LoadRewardedVideo(AdsHelper.RewardId());
            isShowingAd = false;
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        public void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        //ADS
        public async void adShow()
        {
            gainButton.IsEnabled = false;

            //Check if ad is loaded
            if (!CrossMTAdmob.Current.IsRewardedVideoLoaded())
            {
                //Reload reward video
                gainButton.Loading = true;
                CrossMTAdmob.Current.LoadRewardedVideo(AdsHelper.RewardId());
                Costants.showToast("Nessun video disponibile, riprova tra qualche secondo");
            }

            //Check if he can watch a reward video
            var canWatch = await App.Ads.canWatchAd();
            bool success = Convert.ToBoolean(canWatch.Data);

            //Error in the API call
            if (!string.IsNullOrEmpty(canWatch.Message) || !success)
            {
                Costants.showToast(canWatch.Message);
                gainButton.IsEnabled = true;
                return;
            }


            //Success

            //Show video
            isShowingAd = true;
            CrossMTAdmob.Current.ShowRewardedVideo();
            gainButton.IsEnabled = true;
        }

        private async void Current_OnRewarded(object sender, MTEventArgs e)
        {
            //Try adding coin to user account
            var canWatch = await App.Ads.watchedAd();
            bool success = Convert.ToBoolean(canWatch.Data);

            //Error in the API call
            if (!string.IsNullOrEmpty(canWatch.Message) || !success)
            {
                await DisplayAlert("Attenzione", canWatch.Message, "Ok");
                return;
            }

            //Show success
            await DisplayAlert("Grande", "La sCoin è stata aggiunta al tuo account", "Chiudi");
        }


        public void coinsInfo_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("sCoin", "La SalveminiCoin è una moneta digitale presente nella SalveminiApp che puo' essere utilizzata per l'acquisto di biglietti di eventi organizzati dal Salvemini (come il makπ), sconti su gadget o incontri della scuola o l'acquisto di materiale digitale all'interno dell'app (Personaggi per i giochi, potenziamenti...)", "Ok");
        }

        #region Events from Ads

        public void setAdEvents()
        {
            CrossMTAdmob.Current.OnRewardedVideoStarted += Current_OnRewardedVideoStarted;
            CrossMTAdmob.Current.OnRewarded += Current_OnRewarded;
            CrossMTAdmob.Current.OnRewardedVideoAdClosed += Current_OnRewardedVideoAdClosed;
            CrossMTAdmob.Current.OnRewardedVideoAdFailedToLoad += Current_OnRewardedVideoAdFailedToLoad;
            CrossMTAdmob.Current.OnRewardedVideoAdLeftApplication += Current_OnRewardedVideoAdLeftApplication;
            CrossMTAdmob.Current.OnRewardedVideoAdLoaded += Current_OnRewardedVideoAdLoaded;
            CrossMTAdmob.Current.OnRewardedVideoAdOpened += Current_OnRewardedVideoAdOpened;
            CrossMTAdmob.Current.OnRewardedVideoAdCompleted += Current_OnRewardedVideoAdCompleted;
        }


        private void Current_OnRewardedVideoAdOpened(object sender, EventArgs e)
        {
            Debug.WriteLine("OnRewardedVideoAdOpened");
        }

        private void Current_OnRewardedVideoAdLoaded(object sender, EventArgs e)
        {
            gainButton.Loading = false;

            Debug.WriteLine("OnRewardedVideoAdLoaded");

        }

        private void Current_OnRewardedVideoAdLeftApplication(object sender, EventArgs e)
        {
            Debug.WriteLine("OnRewardedVideoAdLeftApplication");
        }

        private void Current_OnRewardedVideoAdFailedToLoad(object sender, MTEventArgs e)
        {
            Debug.WriteLine("OnRewardedVideoAdFailedToLoad");
        }

        private void Current_OnRewardedVideoAdClosed(object sender, EventArgs e)
        {
            Debug.WriteLine("OnRewardedVideoAdClosed");
        }


        private void Current_OnRewardedVideoStarted(object sender, EventArgs e)
        {
            Debug.WriteLine("OnRewardedVideoStarted");
        }

        private void Current_OnRewardedVideoAdCompleted(object sender, EventArgs e)
        {
            Debug.WriteLine("OnRewardedVideoAdCompleted");
        }


        #endregion
    }
}
