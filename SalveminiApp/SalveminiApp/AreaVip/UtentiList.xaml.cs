﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class UtentiList : ContentPage
    {
        public bool superVip = false;

        public UtentiList()
        {
            InitializeComponent();
        }

        public List<RestApi.Models.Utente> utentiList = new List<RestApi.Models.Utente>();

        async void Search_Action(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                {
                    var sortedList = utentiList.OrderBy(x => x.Nome);
                    utentiListCtrl.ItemsSource = sortedList;

                }
                else
                {
                    //var utentiFiltered = utentiList.Where(x => x.Nome.ToLower().Contains(e.NewTextValue.ToLower()) || x.Cognome.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
                    var utentiFiltered = utentiList.Where(y => y.nomeCognome.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
                    utentiListCtrl.ItemsSource = utentiFiltered;

                }
            }
            else
            {
                await DisplayAlert("Attenzione bro", "Non sei connesso ad internet!!", "Provvedo");
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            utentiListCtrl.IsRefreshing = true;
            searchBar.IsEnabled = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                utentiList = await App.Utenti.GetUtenti();
                if (utentiList.Count < 1)
                {
                    bool decision = await DisplayAlert("Attenzione", "Non è stato possibile caricare gli utenti", "Riprova", "Annulla");
                    if (decision)
                        OnAppearing();
                    else
                        return;
                }
                var sortedList = utentiList.OrderBy(x => x.Nome);
                utentiListCtrl.ItemsSource = sortedList;
            }
            else
            {
                await DisplayAlert("Attenzione", "Non sei connesso ad internet!", "Ok");
            }

            utentiListCtrl.IsRefreshing = false;
            searchBar.IsEnabled = true;

            //Optional super vip feature
            var utente = await App.Utenti.GetUtente(Preferences.Get("UserId", 0));
            if (utente.Stato > 1)
                superVip = true;
        }

        async void userSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            utentiListCtrl.SelectedItem = null;

            var data = (sender as Xamarin.Forms.ListView).SelectedItem as RestApi.Models.Utente;
            if (superVip)
            {
                var userEdit = await DisplayActionSheet("Come vuoi procedere?", "Annulla","Disabilita utente", "Abilita utente", "Accedi");
                switch (userEdit)
                {
                    case "Accedi":
                        Preferences.Set("UserId", data.id);
                        Preferences.Set("Token", data.ArgoToken);
                        App.refreshCalls();
                        await DisplayAlert("Successo", "Ora sei loggato come " + data.nomeCognome, "Yo");
                        break;
                }
            }
           

                    //    case "Disabilita utente":
                    //        if (data.Stato == 2)
                    //        {
                    //            await DisplayAlert("Non puoi disabilitare un VIP!", null, "Cazz");
                    //            return;
                    //        }
                    //        var success1 = await App.Utenti.editUser(data.id, "disable");
                    //        if (success1 == true)
                    //        {
                    //            await DisplayAlert("Successo", "L'account di " + data.Nome + " è stato disabilitato", "Ok");
                    //        }
                    //        else
                    //        {
                    //            await DisplayAlert("Oooops!", "Si è verificato un problema durante la tua richiesta", "Ok");

                    //        }
                    //        break;

                    //    case "Abilita utente":
                    //        var success2 = await App.Utenti.editUser(data.id, "enable");
                    //        if (success2 == true)
                    //        {
                    //            await DisplayAlert("Successo", "L'account di " + data.Nome + " è stato abilitato", "Ok");
                    //        }
                    //        else
                    //        {
                    //            await DisplayAlert("Oooops!", "Si è verificato un problema durante la tua richiesta", "Ok");

                    //        }
                    //        break;

                    //    case "Invia messaggio":
                    //        await DisplayAlert("Aspè", "Non ancora pronto", "Ok");
                    //        break;
                    //}

            }
    }
}