using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;

namespace SalveminiApp.AreaVip
{
    public partial class EliminaAvviso : ContentPage
    {

        public List<RestApi.Models.Avvisi> Avvisi = new List<RestApi.Models.Avvisi>();

        public EliminaAvviso()
        {
            InitializeComponent();
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Show loading
            avvisiListCtrl.IsRefreshing = true;
            searchBar.IsEnabled = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Download avvisi
                Avvisi = await App.Avvisi.GetAvvisi();

                avvisiListCtrl.ItemsSource = Avvisi;
            }
            else
            {
                await DisplayAlert("Attenzione", "Non sei connesso ad internet!", "Ok");
            }

            //Stop loading
            avvisiListCtrl.IsRefreshing = false;
            searchBar.IsEnabled = true;

        }

        async void avvisoSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect cell
            if (e.SelectedItem == null)
                return;
            avvisiListCtrl.SelectedItem = null;

            //Get user selcted
            var data = (sender as Xamarin.Forms.ListView).SelectedItem as RestApi.Models.Avvisi;

            //Ask confermation
            var sure = await DisplayAlert("Attenzione", "Sei sicuro di voler eliminare questo avviso?", "Si", "No");
            if (!sure)
                return;

            //Delete avviso
            var response = await App.Avvisi.DeleteAvviso(data.id);
            await DisplayAlert("Response:", response, "Chiudi");
            OnAppearing();
        }

        async void Search_Action(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                avvisiListCtrl.ItemsSource = Avvisi;

            }
            else
            {
                var avvisiFiltered = Avvisi.Where(x => x.Titolo.ToLower() == e.NewTextValue.ToLower()).ToList();
                avvisiListCtrl.ItemsSource = avvisiFiltered;

            }
        }
    }
}
