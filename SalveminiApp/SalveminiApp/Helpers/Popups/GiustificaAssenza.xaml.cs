using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace SalveminiApp.Helpers.Popups
{
    public partial class GiustificaAssenza : PopupPage
    {
        public RestApi.Models.Assenza Assenza = new RestApi.Models.Assenza();

        public GiustificaAssenza(RestApi.Models.Assenza assenza)
        {
            InitializeComponent();

            //Save useful parameters
            Assenza = assenza;

            //Init Interface
            assenzeImage.HeightRequest = App.ScreenWidth / 13;
            assenzeImage.WidthRequest = App.ScreenWidth / 13;
            datAssenzaLabel.Text = "Assenza di " + Assenza.Data;
        }

        async void Giustifica_Clicked(object sender, System.EventArgs e)
        {

            if (string.IsNullOrEmpty(giustifica.Text))
            {
                await DisplayAlert("Errore", "Inserisci un motivo per la tua assenza", "Ok");
                return;
            }

            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;
            sendBtn.IsEnabled = false;
            try
            {
                var giustificaModel = new RestApi.Models.AssenzaModel();
                giustificaModel.listaAssenze = new List<RestApi.Models.ListaAssenze>() { new RestApi.Models.ListaAssenze { binUid = Assenza.binUid, datAssenza = Convert.ToDateTime(Assenza.datAssenza).ToString("dd/MM/yyyy") } };

                giustificaModel.motivazione = !string.IsNullOrEmpty(giustifica.Text) ? giustifica.Text : "";

                var success = await App.Argo.GiustificaAssenza(giustificaModel);

                if (string.IsNullOrEmpty(success.Message))
                {
                    MessagingCenter.Send((App)Application.Current, "ReloadAssenze");
                    await Navigation.PopPopupAsync();
                }
                else
                {
                    giustificaEntry.ErrorText = success.Message;
                }
            }
            catch
            {
                Costants.showToast("Si è verificato un errore");
            }

            sendBtn.IsEnabled = true;
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }
}
