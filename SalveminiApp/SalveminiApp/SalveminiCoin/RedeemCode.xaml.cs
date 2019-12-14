using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if __IOS__
using UIKit;
#endif
using Xamarin.Forms;
using Xamarin.Essentials;

#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.SalveminiCoin
{
    public partial class RedeemCode : ContentPage
    {
        bool closedFromButton = false;
        public RedeemCode()
        {
            InitializeComponent();

            Compass.ReadingChanged += Compass_ReadingChanged;

#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
        }

        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            Console.WriteLine(e.Reading.HeadingMagneticNorth);
        }

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
            //Ios 13 bug
            try
            {
#if __IOS__
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    if (!closedFromButton)
                    {
                        Navigation.PopModalAsync();
                    }
                    else
                    {
                        closedFromButton = false;
                    }
                }
#endif
            }
            catch
            {
                //fa nient
            }
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            closedFromButton = true;
            Navigation.PopModalAsync();
        }

        private async void Confirm_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(codeEntry.Text))
            {
                return;
            }

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
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
                string response = await App.Coins.PostCode(new RestApi.Models.PostCode { xPosition = (decimal)location.Longitude, yPosition = (decimal)location.Latitude, Codice = Convert.ToInt32(codeEntry.Text) });

                infoLabel.Text = response;
                if (response.ToLower().Contains("scoin"))
                {
                    await Task.WhenAll(bgArrowFrame.ColorTo(bgArrowFrame.BackgroundColor, Color.FromHex("#529FFF"), x => bgArrowFrame.BackgroundColor = x, 300), exitButton.ColorTo(exitButton.BackgroundColor, Color.FromHex("#529FFF"), x => exitButton.BackgroundColor = x, 300));
                    await arrowLabel.FadeTo(0, 300);
                    arrowLabel.Text = "check";
                    exitButton.Text = "Fatto";
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
        }

        private void Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Check if lenght of code is 6 numbers
            if (e.NewTextValue.Length == 6)
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
