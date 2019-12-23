using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace SalveminiApp.FlappyMimmo
{
    public partial class Shop : PopupPage
    {


        public Shop()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Check connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }


        }
    }
}
