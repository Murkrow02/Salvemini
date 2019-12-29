using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Extensions;
using System.Collections.ObjectModel;

namespace SalveminiApp.FlappyMimmo
{
    public partial class Shop : PopupPage
    {
        public ObservableCollection<RestApi.Models.FlappySkinReturn> Skins = new ObservableCollection<RestApi.Models.FlappySkinReturn>();

        public Shop()
        {
            InitializeComponent();

            //Set interface
            multiplierValue.Text = "x" + Preferences.Get("multiplier", 1).ToString();
        }

        private void Skin_Selected(object sender, SelectedItemChangedEventArgs e)
        {
            //Check connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            var data = e.SelectedItem as RestApi.Models.FlappySkinReturn;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Check connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            Skins = await App.Flappy.GetSkins();
            if (Skins != null)
            {
                skinsList.FlowItemsSource = Skins;
            }
        }

       async void BuyMultiplier_Tapped(object sender, EventArgs e)
        {
            if (Preferences.Get("multiplier", 1) != 10)
            {
                string buy = await App.Flappy.Upgrade(Preferences.Get("multiplier", 1) + 1);
                if (buy != null)
                {
                    Costants.showToast(buy);
                }
                else
                {
                    multiplierValue.Text = "x" + Preferences.Get("multiplier", 1).ToString();
                }
            }
            else
            {
                Costants.showToast("Hai comprato tutti i potenziamenti");
            }
        }

        void Close_Tapped(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}
