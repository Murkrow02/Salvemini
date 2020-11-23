using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class NewLive : ContentPage
    {
        public NewLive()
        {
            InitializeComponent();
        }

       async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Uri uriResult;
            bool valid = Uri.TryCreate(liveLink.Text, UriKind.Absolute, out uriResult)
 && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!valid)
            {
                await DisplayAlert("Errore", "Inserisci un link valido", "Ok");
                return;
            }

            var success = await App.Index.PostLiveLink(liveLink.Text);

            if (!success)
            {
                await DisplayAlert("Errore", "Si è verificato un errore", "Ok");
                return;
            }
            else
            {
                await DisplayAlert("Successo", "La transazione è stata pubblicata con successo", "Ok");
                await Navigation.PopAsync();
            }
        }
    }
}
