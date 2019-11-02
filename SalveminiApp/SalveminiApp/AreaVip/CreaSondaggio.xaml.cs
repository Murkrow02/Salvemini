using System;
using System.Collections.Generic;
using System.IO;
using Acr.UserDialogs;
using Plugin.Media;
using SalveminiApp.RestApi;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class CreaSondaggio : ContentPage
    {
        List<OggettiToUpload> Oggetti = new List<OggettiToUpload>();
        ImagesToUpload choosenImageToUpload = new ImagesToUpload();

        public CreaSondaggio()
        {
            InitializeComponent();
        }

        public async void Crea_Clicked(object sender, EventArgs e)
        {
            //Create poll and set initial values
            var sondaggio = new Sondaggi();
            sondaggio.Creatore = Preferences.Get("UserId", 0);
            sondaggio.Nome = pollTitle.Text;
            var opzioni_ = new List<OggettiSondaggi>();
            createBtn.IsEnabled = false;

            //todo checks

            //PROGRESS DIALOG
            using (IProgressDialog progress = UserDialogs.Instance.Progress("Caricamento sondaggio", null, null, true, MaskType.Black))
            {
                progress.PercentComplete = 10;

                //CARICAMENTO ALLEGATI
                progress.PercentComplete = 15;

                int imageIndex = 1;
                foreach (var opzione in Oggetti)
                {
                    var opzione_ = new OggettiSondaggi();
                    if (opzione.mediaFile != null)
                    {
                        byte[] byteArray = File.ReadAllBytes(opzione.mediaFile.Path);
                        Stream convertedStream = new MemoryStream(byteArray);
                        var nomeImmagine = (Preferences.Get("UserId", 0).ToString() + imageIndex.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss")).Trim().Replace(' ', '_');
                        bool caricaImmagine = await App.Immagini.uploadImages(convertedStream, nomeImmagine, "sondaggi");

                        //Image upload
                        if (!caricaImmagine)
                        {
                            await DisplayAlert("Ooops", "Si è verificato un errore durante il caricamento delle immagini, riprova o prova con una differente", "Ok");
                            createBtn.IsEnabled = true;
                            return;
                        }
                        else
                        {
                            opzione_.Immagine = nomeImmagine;
                            imageIndex++;
                        }
                    }

                    //Set option name
                    opzione_.Nome = opzione.Nome;
                    progress.PercentComplete += 5;
                    opzioni_.Add(opzione_);
                }
            }

            //Post sondaggio
            sondaggio.OggettiSondaggi = opzioni_;
            var response = App.Sondaggi.PostSondaggio(sondaggio);
        }

        async void choosePhoto(object sender, System.EventArgs e)
        {
            MainPage.isSelectingImage = true;
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Attenzione", "Non ci hai dato il permesso di accedere alle tue foto :(", "Ok");
                MainPage.isSelectingImage = false;
                return;
            }
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

            choosenImageToUpload = new ImagesToUpload();


            choosenImageToUpload.imageSource = ImageSource.FromStream(() =>
            {
                var streams = choosenImage.GetStreamWithImageRotatedForExternalStorage();
                choosenImageToUpload.mediaFile = choosenImage;
                return streams;
            });

            MainPage.isSelectingImage = false;

        }

        public void addOption(object sender, EventArgs e)
        {
            //Check if entered option name
            if(string.IsNullOrEmpty(opzioneEntry.Text) || string.IsNullOrWhiteSpace(opzioneEntry.Text))
            {
                DisplayAlert("Attenzione", "Devi inserire una domanda per il tup sondaggio", "Ok");
                return;
            }

            //Too many options
            if(Oggetti.Count >= 6)
            {
                DisplayAlert("Attenzione", "Puoi aggiungere massimo 6 opzioni al tuo sondaggio", "Ok");
                return;
            }

            //All right, create new option
            var newOggetto = new OggettiToUpload();
            newOggetto.Nome = opzioneEntry.Text;

            //Is there any image?
            if (choosenImageToUpload != null)
            {
                newOggetto.mediaFile = choosenImageToUpload.mediaFile;
                newOggetto.imageSource = choosenImageToUpload.imageSource;
                newOggetto = null;
            }

            //Add to list new option
            Oggetti.Add(newOggetto);

            //Clear entry
            opzioneEntry.Text = "";

            //Notify success
            DisplayAlert("Successo", "L'oggetto è stato aggiunto come opzione del sondaggio","Ok");
        }
    }
}
