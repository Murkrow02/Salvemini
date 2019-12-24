using System;
using System.Collections.Generic;
using SalveminiApp.RestApi.Models;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class ApprovaCringe : ContentPage
    {
        public List<Domande> domande = new List<Domande>();

        public ApprovaCringe()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            domandeList.IsRefreshing = true;
            domande = await App.Cringe.approveList();
            if (domande == null)
                DisplayAlert("Attenzione", "Si è verificato un errore nel download delle domande", "Chiudi");
            domandeList.ItemsSource = domande;
            domandeList.IsRefreshing = false;

        }

        public void domande_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }

        async void domanda_Selected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect Animation
            if (e.SelectedItem == null)
                return;

            domandeList.SelectedItem = null;

            var selectedPost = e.SelectedItem as Domande;

            //Prevent error
            if (selectedPost == null)
                return;

            bool decision = await DisplayAlert("Scegli", "Cosa vuoi fare con questo post?", "Accetta", "Rifiuta");
            var response = await App.Cringe.ApprovaDomanda(selectedPost.id, decision);
            await DisplayAlert(response[0], response[1], "Ok");
        }
    }
}
