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
            stationPicker.ItemsSource = Stazioni;

        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Check connection and download orari
            if(Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Treni
                bool successTreni = await App.Treni.GetTrainJson();

                if (successTreni)
                {
                    await progress.ProgressTo(1, 500,Easing.CubicInOut);
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
            downlaodStatus.Text = "Sto scaricando gli orari più recenti di bus, treni e aliscafi";
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
                    if(stationPicker.SelectedItem != null && TrenoSegment.SelectedSegment != -1)
                    {
                        Preferences.Set("savedStation", Costants.Stazioni.FirstOrDefault(x => x.Value == stationPicker.SelectedItem.ToString()).Key);
                        bool direction = TrenoSegment.SelectedSegment.ToString() == Costants.Stazioni[Costants.Stazioni.Count - 1];
                        Preferences.Set("savedDirection", direction);
                    }
                    else
                    {
                        DisplayAlert("Attenzione", "Seleziona una stazione di partenza e una direzione","Ok");
                    }
                    break;
                case 1:
                    //Bus
                    break;
                case 2:
                    //Aliscafi
                    break;
            }
        }

        public void ChoseMezzo(object o, int e)
        {
            switch (MezzoSegment.SelectedSegment)
            {
                case 0:
                    //Treni
                    break;
                case 1:
                    //Bus
                    break;
                case 2:
                    //Aliscafi
                    break;
            }
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
