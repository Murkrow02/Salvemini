using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.iCringe
{
    public partial class NewPost : PopupPage
    {

        public NewPost()
        {
            InitializeComponent();
        }

        public async void send_Clicked(object sender, EventArgs e)
        {
            //Check internet
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            if (string.IsNullOrEmpty(domanda.Text))
            {
                Costants.showToast("Scrivi qualcosa!");
                return;
            }

            //Show loading
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;
            sendBtn.IsEnabled = false;

            //Post question
            var response = await App.Cringe.PostDomanda(domanda.Text);
            if (response[0] == "Successo")
            {
                MessagingCenter.Send((App)Application.Current, "RefreshPosts");
                await DisplayAlert("Successo", response[1], "Ok");
                await Navigation.PopPopupAsync();

            }
            else
            {
                Costants.showToast(response[1]);
            }

            //Hide loading
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
            sendBtn.IsEnabled = true;
        }

    }
}
