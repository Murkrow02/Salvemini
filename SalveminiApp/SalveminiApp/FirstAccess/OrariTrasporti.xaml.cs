using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Forms9Patch;
using System.Linq;
using UIKit;
using Intents;
using IntentsUI;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Foundation;
#endif

namespace SalveminiApp.FirstAccess
{

    public partial class OrariTrasporti : ContentPage
    {
        public List<RestApi.Models.Treno> Trains = new List<RestApi.Models.Treno>();
        public List<string> Stazioni = new List<string>(Costants.Stazioni.Values);

        public OrariTrasporti()
        {
            InitializeComponent();

            //Set Safe Area
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
#endif

            //Set heights
            topImage.WidthRequest = App.ScreenWidth / 1.9;

            //Set pickers lists
            trainStationPicker.ItemsSource = Stazioni;

        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Check connection and download orari
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Treni
                bool successTreni = await App.Treni.GetTrainJson();

                if (successTreni)
                {
                    await progress.ProgressTo(1, 500, Easing.CubicInOut);
                    loadingLayout.IsVisible = false;

                    //Enable continue button
                    doneBtn.Opacity = 1;
                    doneBtn.IsEnabled = true;
                    Preferences.Set("OrarioTrasportiVersion", 1);
                }
                else
                {
                    fail();
                    await DisplayAlert("Errore", "Non è stato possibile scaricare gli orari, contattaci se il problema persiste", "Ok");
                }
            }
            else
            {
                fail();
                await DisplayAlert("Errore", "Hai bisogno di una connessione ad internet per scaricare gli orari", "Ok");
            }

        }


        public void fail()
        {
            loading.IsRunning = false;
            loading.IsVisible = false;
            retry.IsVisible = true;
            downlaodStatus.Text = "Non è stato possibile scaricare gli orari";
            progress.ProgressTo(0, 500, Easing.CubicInOut);

        }

        void Retry_Clicked(object sender, System.EventArgs e)
        {
            downlaodStatus.Text = "Sto scaricando gli orari più recenti di bus e treni";
            loading.IsRunning = true;
            loading.IsVisible = true;
            retry.IsVisible = false;
            progress.ProgressTo(0, 500, Easing.CubicInOut);
            OnAppearing();
        }

        async void Continue_Clicked(object sender, System.EventArgs e)
        {
            if (trainStationPicker.SelectedItem != null && TrenoSegment.SelectedSegment != -1)
            {
                //Get station and direction
                var station = Costants.Stazioni.FirstOrDefault(x => x.Value == trainStationPicker.SelectedItem.ToString()).Key;
                bool direction = Convert.ToBoolean(TrenoSegment.SelectedSegment);

                //Save in preferences
                Preferences.Set("savedStation", station);
                Preferences.Set("savedDirection", direction);

#if __IOS__
                //Save values for siri intent
                var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
                defaults.AddSuite("group.com.codex.SalveminiApp");
                defaults.SetInt(station, new NSString("SiriStation"));
                defaults.SetBool(direction, new NSString("SiriDirection"));
#endif
            }
            else
            {
                await DisplayAlert("Attenzione", "Seleziona una stazione di partenza e una direzione", "Ok");
                return;
            }

            if (Preferences.Get("isFirstTime", true))
            {
                Xamarin.Forms.Application.Current.MainPage = new TabPage();
                Preferences.Set("isFirstTime", false);
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }

        private void TrenoSegment_OnSegmentSelected(object sender, Plugin.Segmented.Event.SegmentSelectEventArgs e)
        {

            if (string.IsNullOrEmpty(trainStationPicker.SelectedItem?.ToString()))
                return;



            //Sorrento to sorrento
            if (trainStationPicker.SelectedItem.ToString() == "Sorrento" && TrenoSegment.SelectedSegment == 0)
                TrenoSegment.SelectedSegment = 1;

            //Napoli to napoli
            if (trainStationPicker.SelectedItem.ToString() == "Napoli Porta Nolana" && TrenoSegment.SelectedSegment == 1)
                TrenoSegment.SelectedSegment = 0;


        }

        private void picker_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(trainStationPicker.SelectedItem?.ToString()))
                return;

            //Sorrento to sorrento
            if (trainStationPicker.SelectedItem.ToString() == "Sorrento" && TrenoSegment.SelectedSegment == 0)
                TrenoSegment.SelectedSegment = 1;

            //Napoli to napoli
            if (trainStationPicker.SelectedItem.ToString() == "Napoli Porta Nolana" && TrenoSegment.SelectedSegment == 1)
                TrenoSegment.SelectedSegment = 0;

        }

        public void skipClicked(object sender, System.EventArgs e)
        {
            if (Preferences.Get("isFirstTime", true))
            {
                Xamarin.Forms.Application.Current.MainPage = new TabPage();
                //Remove Pages behind MainPage
                //Navigation.RemovePage(Navigation.NavigationStack[0]);
                Preferences.Set("isFirstTime", false);
            }
            else
            {
                Navigation.PopModalAsync();
            }
        }

    }
}
