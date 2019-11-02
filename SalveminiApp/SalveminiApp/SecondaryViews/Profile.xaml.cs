using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class Profile : ContentPage
    {

        public RestApi.Models.Utente utente = new RestApi.Models.Utente();
        public bool shouldClose = true;
        public MediaFile newPic;

        public Profile()
        {
            InitializeComponent();


            //Get cache
            if (Barrel.Current.Exists("utenteLoggato"))
            {
                utente = Barrel.Current.Get<RestApi.Models.Utente>("utenteLoggato");
                nameLbl.Text = utente.nomeCognome;
                classLbl.Text = utente.classeCorso;
                userImg.Source = utente.Immagine;
            }

            //Set dimensions
            userImg.WidthRequest = App.ScreenWidth / 3.5;

            //Create tapped gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            //Fill lists
            //Personalizza
            var notifiche = new Helpers.PushCell { Title = "Notifiche", Separator = "si" };
            notifiche.GestureRecognizers.Add(tapGestureRecognizer);
            persLayout.Children.Add(notifiche);
            var countdown = new Helpers.PushCell { Title = "Countdown", Separator = "si" };
            countdown.GestureRecognizers.Add(tapGestureRecognizer);
            persLayout.Children.Add(countdown);
            var argoPwd = new Helpers.PushCell { Title = "Password di Argo", Separator = "si" };
            argoPwd.GestureRecognizers.Add(tapGestureRecognizer);
            persLayout.Children.Add(argoPwd);
            var profileImg = new Helpers.PushCell { Title = "Immagine di profilo", Separator = "No" };
            profileImg.GestureRecognizers.Add(tapGestureRecognizer);
            persLayout.Children.Add(profileImg);


            //Contattaci
            var insta = new Helpers.PushCell { Title = "Il nostro team", Separator = "si", Push = new SecondaryViews.Team() };
            insta.GestureRecognizers.Add(tapGestureRecognizer);
            contactLayout.Children.Add(insta);
            var Mail = new Helpers.PushCell { Title = "Mail", Separator = "No" };
            Mail.GestureRecognizers.Add(tapGestureRecognizer);
            contactLayout.Children.Add(Mail);

            //Vip
            var avviso = new Helpers.PushCell { Title = "Crea avviso", Separator = "si", Push = new AreaVip.CreaAvviso() };
            avviso.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(avviso);
            var stats = new Helpers.PushCell { Title = "Statistiche", Separator = "si", Push = new AreaVip.Analytics() };
            stats.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(stats);
            var utenti = new Helpers.PushCell { Title = "Utenti", Separator = "si", Push = new AreaVip.UtentiList() };
            utenti.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(utenti);
            var sondaggio = new Helpers.PushCell { Title = "Crea sondaggio", Separator = "si", Push = new AreaVip.CreaSondaggio() };
            sondaggio.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(sondaggio);
            var orari = new Helpers.PushCell { Title = "Crea orario", Separator = "no", Push = new AreaVip.CreaOrario() };
            orari.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(orari);

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                //Get push page from model
                var cell = sender as Helpers.PushCell;

                //Mail
                if (cell.Title == "Mail")
                {
                    try
                    {
                        Device.OpenUri(new Uri("mailto:support@codexdevelopment.net"));
                    }
                    catch
                    {
                        DisplayAlert("Errore", "Non è possiile inviare e-mail da questo dispositivo, in alternativa puoi scrivere a questo indirizzo: support@codexdevelopment.net", "Ok");
                    }
                    return;
                }

                //Profile image
                if (cell.Title == "Immagine di profilo")
                {
                    changePic();
                }

                //Push to selected page
                if (cell.Push != null)
                {
                    //Other
                    shouldClose = false;
                    Navigation.PushAsync(cell.Push); //Push
                }
            }
            catch (Exception ex)
            {
                //Page not set or some random error, sticazzi
                return;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Get current user
                var user = await App.Utenti.GetUtente(Preferences.Get("UserId", 0));
                if (user != null)
                {
                    //Display user details
                    utente = user;
                    nameLbl.Text = utente.nomeCognome;
                    classLbl.Text = utente.classeCorso;
                    userImg.Source = utente.Immagine;

                    //Show vip area if vip
                    if (utente.Stato > 0)
                    {
                        vipLayout.IsEnabled = true;
                        vipLayout.Opacity = 1;
                    }


                    //Save cache
                    Barrel.Current.Add("utenteLoggato", user, TimeSpan.FromDays(10));
                }
                else
                {
                    //todo handle error
                }
            }
            else
            {
                //todo handle no connection
            }

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }

        private void esci_Clicked(object sender, EventArgs e)
        {
            //Perform logout
            Costants.Logout();

        }

        private async void changePic()
        {
            var decision = await DisplayActionSheet("Come vuoi procedere", "Annulla", "Rimuovi immagine", "Scegli una foto", "Scatta una foto");

           MainPage.isSelectingImage = true;
            if (decision == "Scegli una foto")
            {
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

                //Upload image
                var success = await App.Immagini.uploadImages(choosenImage.GetStreamWithImageRotatedForExternalStorage(), Preferences.Get("UserId", 0).ToString(), "users");

                if (success)
                {
                    reloadImage();
                }
                else
                {
                    MainPage.isSelectingImage = false;
                    await DisplayAlert("Errore", "Si è verificato un errore durante il caricamento dell'immagine, contattaci se il problema persiste", "Ok");
                }

            }
            else if (decision == "Scatta una foto")
            {
                if (!CrossMedia.Current.IsCameraAvailable)
                {
                    await DisplayAlert("Attenzione", "Non è stato possibile accedere alla fotocamera", "Ok");
                    MainPage.isSelectingImage = false;
                    return;
                }


                var choosenImage = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    CompressionQuality = 80,
                    AllowCropping = false,
                    PhotoSize = PhotoSize.Medium,
                    RotateImage = true,
                    DefaultCamera = CameraDevice.Rear
                });

                if (choosenImage == null)
                {
                    MainPage.isSelectingImage = false;
                    return;
                }

                //Upload image
                var success = await App.Immagini.uploadImages(choosenImage.GetStreamWithImageRotatedForExternalStorage(), Preferences.Get("UserId", 0).ToString(), "users");

                if (success)
                {
                    reloadImage();
                }
                else
                {
                    await DisplayAlert("Errore", "Si è verificato un errore durante il caricamento dell'immagine, contattaci se il problema persiste", "Ok");
                    MainPage.isSelectingImage = false;

                }
            }
        }

        public async void reloadImage()
        {
            //Remove cache
            await ImageService.Instance.InvalidateCacheEntryAsync(utente.Immagine, CacheType.All, removeSimilar: true);
            userImg.Source = "";
            userImg.Source = utente.Immagine;
            userImg.ReloadImage();
            userImg.WidthRequest = App.ScreenWidth / 3.5;
            userImg.HeightRequest = App.ScreenWidth / 3.5;

            //Remove cached home profile pic
            MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "ReloadUserPic");
        }
    }
}
