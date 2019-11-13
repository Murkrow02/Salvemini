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
        ImagesToUpload choosenImageToUpload = null;

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
            //At least 2 objects noob
            if(Oggetti.Count < 2)
            {
                await DisplayAlert("Attenzione", "Devi aggiungere almeno 2 opzioni per inviare il sondaggio!", "Ok");
                createBtn.IsEnabled = true;
                return;
            }

            //Check if entered option name
            if (string.IsNullOrEmpty(pollTitle.Text) || string.IsNullOrWhiteSpace(pollTitle.Text))
            {
                await DisplayAlert("Attenzione", "Devi inserire una domanda per il tuo sondaggio", "Ok");
                createBtn.IsEnabled = true;
                return;
            }

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
            var response = await App.Sondaggi.PostSondaggio(sondaggio);

            //Show response
            await DisplayAlert(response[0], response[1], "Ok");

            //Success
            if (response[0] == "Grazie!")
               await Navigation.PopModalAsync();

            createBtn.IsEnabled = true;
        }

        async void OrderOpzioni()
        {
            //Reset
            widgetsLayout.Children.Clear();

            //Add initial space
            widgetsLayout.Children.Add(new Xamarin.Forms.ContentView { WidthRequest = 5 });

            int selectedWidget = 0;
            //Fill layout with sondaggio options
            foreach (var opzione in Oggetti)
            {
                //Create new layout to add a vote button under the option
                var layout = new StackLayout { Spacing = 2, VerticalOptions = LayoutOptions.FillAndExpand };
                var removeBtn = new Button { Text = "Rimuovi", BackgroundColor = Color.Transparent, FontSize = 15, Margin = 0, VerticalOptions = LayoutOptions.Start };
                removeBtn.Clicked += RemoveBtn_Clicked;

                ImageSource image = null;
                string hideImage = null;
                //Detect images
                if (opzione.imageSource != null)
                    image = opzione.imageSource;
                else
                    hideImage = "si";

                //Create new poll option
                var widget = new Helpers.PollOption {HideImage = hideImage, optionId = selectedWidget, Title = opzione.Nome, VerticalOptions = LayoutOptions.CenterAndExpand,PreviewImage = image };
                layout.Children.Add(widget);
                layout.Children.Add(removeBtn);
                widgetsLayout.Children.Add(layout);

                //Give widget an id for removal
                selectedWidget++;
            }

            void RemoveBtn_Clicked(object sender, EventArgs e)
            {
                try
                {
                    var button = sender as Button;
                    var layout = button.Parent as StackLayout;
                    var widget = layout.Children[0] as Helpers.PollOption;
                    Oggetti.RemoveAt(widget.optionId);
                }
                catch
                {
                    Oggetti.Clear();
                }
                OrderOpzioni();
            }
        }

        

        async void choosePhoto(object sender, System.EventArgs e)
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

            choosenImageToUpload = new ImagesToUpload();


            choosenImageToUpload.imageSource = ImageSource.FromStream(() =>
            {
                var streams = choosenImage.GetStreamWithImageRotatedForExternalStorage();
                return streams;
            });

            choosenImageToUpload.mediaFile = choosenImage;


            MainPage.isSelectingImage = false;
            attachBtn.IsEnabled = false;
            attachBtn.Text = "Hai allegato un file";
        }

        public void addOption(object sender, EventArgs e)
        {
            //Too many options
            if(Oggetti.Count >= 6)
            {
                DisplayAlert("Attenzione", "Puoi aggiungere massimo 6 opzioni al tuo sondaggio", "Ok");
                return;
            }

            //No name to option
            if (string.IsNullOrEmpty(opzioneEntry.Text))
            {
                DisplayAlert("Attenzione", "Inserisci un nome per questa opzione", "Ok");
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
                choosenImageToUpload = null;
            }

            //Add to list new option
            Oggetti.Add(newOggetto);

            //Clear entry
            opzioneEntry.Text = "";

            //reset attachment
            attachBtn.IsEnabled = true;
            attachBtn.Text = "Allega immagine (Opzionale)";

            //Notify success
           // DisplayAlert("Successo", "L'oggetto è stato aggiunto come opzione del sondaggio","Ok");
            OrderOpzioni();
        }
    }
}
