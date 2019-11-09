using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class UtentiList : ContentPage
    {
        public bool superVip = false;

        public UtentiList(bool superVip_)
        {
            InitializeComponent();

            //Detect if super vip
            superVip = superVip_;
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

            //Show loading
            utentiListCtrl.IsRefreshing = true;
            searchBar.IsEnabled = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Download users
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

            //Stop loading
            utentiListCtrl.IsRefreshing = false;
            searchBar.IsEnabled = true;

        }

        async void userSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect cell
            if (e.SelectedItem == null)
                return;
            utentiListCtrl.SelectedItem = null;

            //Get user selcted
            var data = (sender as Xamarin.Forms.ListView).SelectedItem as RestApi.Models.Utente;

            //Access with feature if super vip
            if (superVip)
            {
                //Show options
                var userEdit = await DisplayActionSheet("Come vuoi procedere?", "Annulla", "Accedi", "Rendi VIP","Rendi rappresentante","Rendi plebeo");
                switch (userEdit)
                {
                    case "Rendi VIP":

                        //Vip user
                        var response1 = await App.Utenti.ChangeStatus(data.id, 2);

                        //Notify status of the request
                        await DisplayAlert(response1[0], response1[1], "Ok");
                        break;

                    case "Rendi rappresentante":

                        //Re-Enable user
                        var response2 = await App.Utenti.ChangeStatus(data.id, 1);

                        //Notify status of the request
                        await DisplayAlert(response2[0], response2[1], "Ok");
                        break;
                    case "Rendi plebeo":

                        //Re-Enable user
                        var response3 = await App.Utenti.ChangeStatus(data.id, 0);

                        //Notify status of the request
                        await DisplayAlert(response3[0], response3[1], "Ok");
                        break;
                    case "Accedi":
                        Costants.Logout(true);
                        Preferences.Set("UserId", data.id);
                        Preferences.Set("Token", data.ArgoToken);
                        Preferences.Set("Classe", data.Classe);
                        Preferences.Set("Corso", data.Corso);
                        App.refreshCalls();
                        await DisplayAlert("Successo", "Ora sei loggato come " + data.nomeCognome, "Yo");
                        //Remove cached home profile pic
                        MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "ReloadUserPic");
                        break;
                }
              
            }
            else
            {
                //Show options
                var userEdit = await DisplayActionSheet("Come vuoi procedere?", "Annulla", "Disabilita utente", "Abilita utente");
                switch (userEdit)
                {
                    case "Disabilita utente":
                        //The user is vip
                        if (data.Stato >= 2)
                        {
                            await DisplayAlert("Non puoi disabilitare un VIP!", null, "Cazz");
                            return;
                        }

                        //Disable user
                        var response1 = await App.Utenti.ChangeStatus(data.id, -1);

                        //Notify status of the request
                        await DisplayAlert(response1[0], response1[1], "Ok");
                        break;

                    case "Abilita utente":

                        //Re-Enable user
                        var response2 = await App.Utenti.ChangeStatus(data.id, 0);

                        //Notify status of the request
                        await DisplayAlert(response2[0], response2[1], "Ok");
                        break;
                }
            }
        }


    }



}

