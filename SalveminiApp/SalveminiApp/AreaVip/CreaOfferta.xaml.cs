using System;
using System.Collections.Generic;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class CreaOfferta : ContentPage
    {
        public MediaFile foto = null; 

        public CreaOfferta()
        {
            InitializeComponent();
        }


        async void Crea_Clicked(object sender, System.EventArgs e)
        {
            //Something missing
            if(string.IsNullOrEmpty(title.Text) || string.IsNullOrEmpty(desc.Text) || foto == null){
                await DisplayAlert("Errore", "Non hai completato tutti i campi", "Ok");
                return;
            }

            //Upload image
            Stream convertedStream = foto.GetStreamWithImageRotatedForExternalStorage();
            var nomeImmagine = (title.Text + Preferences.Get("UserId", 0).ToString() + DateTime.Now.ToString("yyyyMMddHHmmss")).Trim().Replace(' ', '_');
            bool caricaImmagine = await App.Immagini.uploadImages(convertedStream, nomeImmagine, "card");

            //Failed uploading image
            if (!caricaImmagine)
            {
                await DisplayAlert("Errore", "Non è stato possibile caricare l'immagine", "Ok");
                return;
            }

            //Create offerta
            var offerta = new RestApi.Models.Offerte();
            offerta.Nome = title.Text;
            offerta.Descrizione = desc.Text;
            offerta.Immagine = nomeImmagine;

            var response = await App.Card.PostOfferta(offerta);

            //Notify user the response
            await DisplayAlert(response[0], response[1],"Ok");
        }

        async void attachImage_Clicked(object sender, System.EventArgs e)
        {
            //Prevent modal closing
            MainPage.isSelectingImage = true;

            //No gallery
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Attenzione", "Non è stato possibile accedere alle foto", "Ok");
                MainPage.isSelectingImage = false;
                return;
            }

            //Check permissions
            bool garanted = await Helpers.CameraPermissions.checkPermissions();
            if (!garanted)
                await DisplayAlert("Attenzione", "Non ci hai permesso di accedere alla tua fotocamera o alla tua galleria", "Ok");

            var choosenImage = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 80

            });

            if (choosenImage == null)
            {
                MainPage.isSelectingImage = false;
                return;
            }

            //save picked photo
            foto = choosenImage;


            MainPage.isSelectingImage = false;
        }
    }
}
