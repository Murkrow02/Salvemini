using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class AddEvento : ContentPage
    {
        public AddEvento()
        {
            InitializeComponent();

            //Item source for raggio in metres
            raggioPicker.ItemsSource = new List<int> { 20, 50, 100, 200 };
            //Item source for valore in sCoin
            valorePicker.ItemsSource = ValoreValues();
        }



        public async void save_Clicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;
            //Check values
            if (string.IsNullOrEmpty(eventName.Text) || valorePicker.SelectedItem == null || raggioPicker.SelectedItem == null)
            {
                await DisplayAlert("Attenzione", "Non hai completato tutti i campi", "Ok");
                return;
            }

            //Check location permissions
            var garanted = await Helpers.Permissions.locationPermission();
            if (!garanted) return; //No permission to localize

            //Get location
            var location = await Geolocation.GetLocationAsync();
            if(location == null) //Error localizing
            {
                await DisplayAlert("Errore","Non è stato possibile localizzare il dispositivo, controlla la tua copertura e riprova", "Ok");
                return;
            }
            //Check accuracy
            var validityMessage = await Helpers.Permissions.positionValidity(location);
            if (!string.IsNullOrEmpty(validityMessage))
            {
                await DisplayAlert("Attenzione", validityMessage, "Ok");
                return;
            }

            //All right, create event
            var evento = new RestApi.Models.Evento { idCreatore = Preferences.Get("UserId", -1), Nome = eventName.Text, Raggio = Convert.ToDecimal(raggioPicker.SelectedItem.ToString()), Valore = (int)valorePicker.SelectedItem, xAttivazione = (decimal)location.Longitude, yAttivazione = (decimal)location.Latitude };
            var response = await App.Coins.PostEvento(evento);

            //Show response
            (sender as Button).IsEnabled = true;
            await DisplayAlert(response[0], response[1], "Ok");
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();




        }





        //Create valori list from 50 to 300
        public List<int> ValoreValues()
        {
            var valori = new List<int>();
            for (int i = 50; i <= 300; i+=50)
            {
                valori.Add(i);
            }
            return valori;
        }

    }
}
