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
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;

#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Intents;
using Foundation;
using UIKit;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class Profile : ContentPage
    {

        public RestApi.Models.Utenti utente = new RestApi.Models.Utenti();
        public MediaFile newPic;

        public Profile()
        {
            InitializeComponent();

            //Load info from cached
            userInfo.CachedUserId = Preferences.Get("UserId", 0);


            //Create tapped gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            //Set version label
            versionLabel.Text = VersionTracking.CurrentVersion;

            //Fill lists
            //Personalizza
#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                //Check if downloaded orario
                var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
                defaults.AddSuite("group.com.codex.SalveminiApp");
                bool exists = !string.IsNullOrEmpty(defaults.StringForKey("SiriClass"));

                if (exists) //Orario saved
                {
                    //Exists, add shortcut cell
                    var siriShortcuts = new Helpers.PushCell { Title = "Scorciatoie di Siri", Separator = "si" };
                    siriShortcuts.GestureRecognizers.Add(tapGestureRecognizer);
                    persLayout.Children.Add(siriShortcuts);
                }
            }
            else
            {
                persLayout.Children.Add(new Label { Text = "Aggiorna ad iOS 13 per usare le scorciatoie di Siri", Margin = 5, TextColor = Styles.TextGray, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center });
            }
#endif
            var countdown = new Helpers.PushCell { Title = "Countdown", Separator = "si", Push = new SecondaryViews.CountdownSettings() };
            countdown.GestureRecognizers.Add(tapGestureRecognizer);
            persLayout.Children.Add(countdown);
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

            //Rappresentanti
            var orariClasse = new Helpers.PushCell { Title = "Modifica orario", Separator = "no", Push = new AreaVip.CreaOrario(Preferences.Get("Classe", 0).ToString() + Preferences.Get("Corso", "")) };
            orariClasse.GestureRecognizers.Add(tapGestureRecognizer);
            rapprLayout.Children.Add(orariClasse);

            //Vip
            var avviso = new Helpers.PushCell { Title = "Crea avviso", Separator = "si", Push = new AreaVip.CreaAvviso() };
            avviso.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(avviso);
            var stats = new Helpers.PushCell { Title = "Statistiche", Separator = "si", Push = new AreaVip.Analytics() };
            stats.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(stats);
            var utenti = new Helpers.PushCell { Title = "Controlla utenti", Separator = "si", Push = new AreaVip.UtentiList(false) };
            utenti.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(utenti);
            var evento = new Helpers.PushCell { Title = "Crea evento sCoin", Separator = "si", Push = new AreaVip.AddEvento() };
            evento.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(evento);
            var eventi = new Helpers.PushCell { Title = "Visualizza eventi sCoin", Separator = "si", Push = new AreaVip.ListaEventi(true) };
            eventi.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(eventi);
            var sondaggio = new Helpers.PushCell { Title = "Crea sondaggio", Separator = "no", Push = new AreaVip.CreaSondaggio() };
            sondaggio.GestureRecognizers.Add(tapGestureRecognizer);
            vipLayout.Children.Add(sondaggio);

            //SuperVip
            var orari = new Helpers.PushCell { Title = "Crea orario", Separator = "si", Push = new AreaVip.CreaOrario() };
            orari.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(orari);
            var offerta = new Helpers.PushCell { Title = "Crea offerta", Separator = "si", Push = new AreaVip.CreaOfferta() };
            offerta.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(offerta);
            var console = new Helpers.PushCell { Title = "Console", Separator = "si", Push = new AreaVip.EventLogs() };
            console.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(console);
            var deleteAvvisi = new Helpers.PushCell { Title = "Elimina avviso", Separator = "si", Push = new AreaVip.EliminaAvviso() };
            deleteAvvisi.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(deleteAvvisi);
            var appInfo = new Helpers.PushCell { Title = "App Info", Separator = "si", Push = new AreaVip.AppInfo() };
            appInfo.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(appInfo);
            var giornalino = new Helpers.PushCell { Title = "Carica giornalino", Separator = "si" };
            giornalino.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(giornalino);
            var iCringe = new Helpers.PushCell { Title = "iCringe", Separator = "si", Push = new AreaVip.ApprovaCringe() };
            iCringe.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(iCringe);
            var accediCon = new Helpers.PushCell { Title = "Controlla utenti", Separator = "no", Push = new AreaVip.UtentiList(true) };
            accediCon.GestureRecognizers.Add(tapGestureRecognizer);
            superVipLayout.Children.Add(accediCon);

        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Status bar color
#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            }
#endif

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Get current user
                    var user = await App.Utenti.GetUtente(Preferences.Get("UserId", 0));
                    if (user != null)
                    {
                        //Save in global variable
                        utente = user;

                        //Update user widget
                        userInfo.User = utente;

                        //Update home image
                        MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "ReloadUserPic", utente.fullImmagine);

                        //Show areas according to user status
                        switch (utente.Stato)
                        {
                            case 1: //Rappresentante
                                rapprLayout.IsEnabled = true;
                                rapprLayout.Opacity = 1;
                                break;
                            case 2: //Vip
                                rapprLayout.IsEnabled = true;
                                rapprLayout.Opacity = 1;
                                vipLayout.IsEnabled = true;
                                vipLayout.Opacity = 1;
                                break;
                            case 3: //Super vip
                                rapprLayout.IsEnabled = true;
                                rapprLayout.Opacity = 1;
                                vipLayout.IsEnabled = true;
                                vipLayout.Opacity = 1;
                                superVip.IsVisible = true;
                                break;
                            default: //Hide everything
                                rapprLayout.IsEnabled = false;
                                rapprLayout.Opacity = 0.6;
                                vipLayout.IsEnabled = false;
                                vipLayout.Opacity = 0.6;
                                superVip.IsVisible = false;
                                break;
                        }
                    }
                    else //No user returned
                    {
                        Costants.showToast("Non è stato possibile recuperare il tuo profilo");
                    }
                }
                catch //Error
                {
                    Costants.showToast("Non è stato possibile recuperare il tuo profilo");
                }
            }
            else //No connection
            {
                Costants.showToast("connection");
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

        private void ChangePwd_Clicked(object sender, EventArgs e)
        {
            //Push to changePwd Page
            Navigation.PushAsync(new CambiaPassword());
        }

        private async void changePic(object sender, EventArgs e)
        {
            var decision = await DisplayActionSheet("Come vuoi procedere", "Annulla", "Rimuovi immagine", "Scegli una foto", "Scatta una foto");

            //Prevent modal closing
            MainPage.isSelectingImage = true;

            //Check permissions
            bool garanted = await Helpers.Permissions.checkPermissions();
            if (!garanted)
                return;

            try
            {
                if (decision == "Scegli una foto")
                {
                    //No gallery
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("Attenzione", "Non è stato possibile accedere alle foto", "Ok");
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
                else if (decision == "Rimuovi immagine")
                {
                    var success = await App.Immagini.DeleteImage();
                    await DisplayAlert(success[0], success[1], "Ok");
                    reloadImage();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Errore", "Non è stato possibile completare l'azione, contattaci se il problema persiste", "Ok");
            }

        }

        public async void reloadImage()
        {
            OnAppearing();

           
        }

        //Handle cell tapped
        async private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
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
                    changePic(null, null);
                }

                //Profile image
                if (cell.Title == "Carica giornalino")
                {
                    uploadGiornalino();
                }

                //Siri shortcuts
                if (cell.Title == "Scorciatoie di Siri")
                {
#if __IOS__
                    //Treni Button
                    var TreniAction = UIAlertAction.Create("Treni", UIAlertActionStyle.Default, (action) =>
                    {
                        if (Preferences.Get("savedStation", -1) == -1 && Preferences.Get("savedDirection", false) == false)
                        {
                            DisplayAlert("Attenzione", "Non hai salvato una stazione preferita. Per farlo vai alla home e clicca sul widget treni, apri la sezione treni e clicca sulla stella per salvare la tratta che preferisci", "Ok");
                            return;
                        }

                        //Create intent
                        var trenoIntent = new TrenoIntent();
                        trenoIntent.SuggestedInvocationPhrase = "Prossimo treno";
                        INShortcut shortcut = new INShortcut(trenoIntent);
                        Navigation.PopModalAsync();
                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(new iOS.SiriShortcutPopup(shortcut, "Treno"), true, null);

                    });

                    //Orario Button
                    var OrarioAction = UIAlertAction.Create("Orario classe", UIAlertActionStyle.Default, (action) =>
                    {
                        //Create intent
                        var orarioIntent = new OrarioIntent();
                        orarioIntent.SuggestedInvocationPhrase = "Orario di domani";
                        INShortcut shortcut = new INShortcut(orarioIntent);
                        Navigation.PopModalAsync();
                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(new iOS.SiriShortcutPopup(shortcut, "Orario"), true, null);
                    });

                    //Display Cool Alert
                    var controller = UIAlertController.Create("Quale scorciatoia vuoi modificare", null, UIAlertControllerStyle.Alert);
                    controller.AddAction(TreniAction);
                    controller.AddAction(OrarioAction);
                    controller.AddAction(UIAlertAction.Create("Annulla", UIAlertActionStyle.Cancel, null));

                    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController.PresentViewController(controller, true, null);
#endif
                }


                //Push to selected page
                if (cell.Push != null)
                {
                    //Other
                    Navigation.PushAsync(cell.Push); //Push
                }
            }
            catch (Exception ex)
            {
                //Page not set or some random error, sticazzi
                return;
            }
        }

        public async void uploadGiornalino()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;
                //string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
                bool successo = await App.Immagini.UploadGiornalinoAsync(fileData.GetStream());
                await DisplayAlert("successo", successo.ToString(), "Chiudi");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", ex.ToString(), "Chiudi");
            }
        }
    }


}
