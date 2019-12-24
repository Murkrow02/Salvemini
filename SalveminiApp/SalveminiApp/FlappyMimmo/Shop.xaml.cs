using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace SalveminiApp.FlappyMimmo
{
    public partial class Shop : PopupPage
    {
        public List<RestApi.Models.FlappySkinReturn> Skins = new List<RestApi.Models.FlappySkinReturn>();

        public Shop()
        {
            InitializeComponent();
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

        void Close_Tapped(object sender, EventArgs e)
        {

        }
    }
}
