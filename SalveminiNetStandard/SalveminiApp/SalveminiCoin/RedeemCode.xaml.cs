using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.Permissions;
namespace SalveminiApp.SalveminiCoin
{
    public partial class RedeemCode : ContentPage
    {
        bool closedFromButton = false;
        public RedeemCode()
        {
            InitializeComponent();

        }

        //private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        //{
        //    Console.WriteLine(e.Reading.HeadingMagneticNorth);
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            confirmFrame.HeightRequest = stationFrame.Height;
            confirmFrame.WidthRequest = stationFrame.Height;
            //confirmFrame.CornerRadius = (float)stationFrame.Height / 2;
            confirmFrame.FadeTo(1, 300);
            try
            {
                Compass.Start(SensorSpeed.UI);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                //mqmmt
            }
            catch (Exception ex)
            {
                // Some other exception has occurred
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }

        //private void Close_Clicked(object sender, EventArgs e)
        //{
        //    closedFromButton = true;
        //    Navigation.PopModalAsync();
        //}

        private async void Confirm_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(codeEntry.Text))
            {
                return;
            }

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
            }

            try
            {
                //Check location permissions
                var garanted = await Helpers.Permissions.locationPermission();
                if (!garanted) return; //No permission to localize
               
                //Get location
                var location = await Geolocation.GetLocationAsync();
                if (location == null) //Error localizing
                {
                    await DisplayAlert("Errore", "Non è stato possibile localizzare il dispositivo, controlla la tua copertura e riprova", "Ok");
                    return;
                }

                //Check accuracy
                var validityMessage = await Helpers.Permissions.positionValidity(location);
                if (!string.IsNullOrEmpty(validityMessage))
                {
                    await DisplayAlert("Attenzione", validityMessage, "Ok");
                    return;
                }

                //Authenticate with API
                confirmButton.IsEnabled = false;
                string response = await App.Coins.PostCode(new RestApi.Models.PostCode { xPosition = (decimal)location.Longitude, yPosition = (decimal)location.Latitude, Codice = Convert.ToInt32(codeEntry.Text) });

                infoLabel.Text = response;
                if (response.ToLower().Contains("scoin"))
                {
                    await Task.WhenAll(bgArrowFrame.ColorTo(bgArrowFrame.BackgroundColor, Color.FromHex("#529FFF"), x => bgArrowFrame.BackgroundColor = x, 300));
                    await arrowLabel.FadeTo(0, 300);
                    arrowLabel.Text = "check";
                    //exitButton.Text = "Fatto";
                    arrowLabel.TextColor = Color.White;
                    await arrowLabel.FadeTo(1, 300);
                }
                else
                {
                    await bgArrowFrame.ColorTo(bgArrowFrame.BackgroundColor, Color.FromHex("#EC6666"), x => bgArrowFrame.BackgroundColor = x, 300);
                    await arrowLabel.FadeTo(0, 300);
                    arrowLabel.Text = "times";
                    arrowLabel.TextColor = Color.White;
                    await arrowLabel.FadeTo(1, 300);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Errore", "Il tuo dispositivo non supporta la localizzazione", "Chiudi");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Errore", "I servizi di localizzazione sono spenti, accendi il GPS per continuare", "Chiudi");

            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                bool decision = await DisplayAlert("Errore", "Non ci hai concesso di accedere alla tua posizione", "Apri impostazioni", "Chiudi");
                if (decision)
                    CrossPermissions.Current.OpenAppSettings();
            }
            catch (Exception ex)
            {
                // Unable to get location
                await DisplayAlert("Errore", "Si è verificato un errore sconosciuto, contattaci se il problema persiste", "Chiudi");
            }

            confirmButton.IsEnabled = true;

        }

        private void Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Check if lenght of code is 6 numbers
            if (e.NewTextValue.Length > 0)
            {
                confirmButton.IsEnabled = true;
            }
            else
            {
                confirmButton.IsEnabled = false;
            }
        }
    }
}
