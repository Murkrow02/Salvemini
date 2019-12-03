using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class ListaEventi : ContentPage
    {
        public ListaEventi()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection"); return;
            }

            //Download eventi
            eventsList.IsRefreshing = true;
            var eventi = await App.Coins.ListaEventi();
            eventsList.IsRefreshing = false;
            if (eventi == null)
            {
                Costants.showToast("Non è stato possibile scaricare gli eventi");
                return;
            }
            eventsList.ItemsSource = eventi;
        }



        async void eventSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Get event selcted
            var data = e.SelectedItem as RestApi.Models.Evento;

            //Deselect cell
            if (e.SelectedItem == null)
                return;
            eventsList.SelectedItem = null;

            //Ask confirm
            var conferma = await DisplayAlert("Attenzione", "Sei sicuro di voler attivare/disattivare questo evento?", "Si", "No");
            if (!conferma) return;

            //Api call to toggle event activation
            var response = await App.Coins.ToggleEvento(data.Codice);

            //Show result
            OnAppearing();
            await DisplayAlert(response[0], response[1], "Ok");
        }


    }
}
