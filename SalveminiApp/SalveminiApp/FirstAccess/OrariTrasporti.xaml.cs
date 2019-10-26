using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Forms9Patch;
using System.Linq;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
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
                    continueLayout.IsVisible = true;
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

        void Continue_Clicked(object sender, System.EventArgs e)
        {
            switch (MezzoSegment.SelectedSegment)
            {
                case 0:
                    //Treni
                    if (trainStationPicker.SelectedItem != null && TrenoSegment.SelectedSegment != -1)
                    {
                        Preferences.Set("savedStation", Costants.Stazioni.FirstOrDefault(x => x.Value == trainStationPicker.SelectedItem.ToString()).Key);
                        bool direction = Convert.ToBoolean(TrenoSegment.SelectedSegment);
                        Preferences.Set("savedDirection", direction);
                    }
                    else
                    {
                        DisplayAlert("Attenzione", "Seleziona una stazione di partenza e una direzione", "Ok");
                        return;
                    }
                    break;
                case 1:
                    //Bus
                    break;
                case 2:
                    //Aliscafi
                    break;
            }

            if (Preferences.Get("isFirstTime", true))
            {
                Navigation.PushModalAsync(new TabPage());
                //Remove Pages behind MainPage
                //Navigation.RemovePage(Navigation.NavigationStack[0]);
                Preferences.Set("isFirstTime", false);
            }
            else
            {
                Navigation.PopModalAsync();
            }
        }

        public void ChoseMezzo(object o, int e)
        {

            switch (MezzoSegment.SelectedSegment)
            {
                case 0:
                    //Treni
                    trenoLayout.IsVisible = true;
                    busLayout.IsVisible = false;
                    break;
                case 1:
                    //Bus
                    trenoLayout.IsVisible = false;
                    busLayout.IsVisible = true;
                    break;
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
                Navigation.PushModalAsync(new TabPage());
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
