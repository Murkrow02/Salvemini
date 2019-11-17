﻿using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using Intents;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
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
            var cachedUtente = CacheHelper.GetCache<RestApi.Models.Utente>("utenteLoggato");
            if (cachedUtente != null)
            {
                utente = cachedUtente;
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

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                var siriShortcuts = new Helpers.PushCell { Title = "Scorciatoie di Siri", Separator = "si" };
                siriShortcuts.GestureRecognizers.Add(tapGestureRecognizer);
                persLayout.Children.Add(siriShortcuts);
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
            var orariClasse = new Helpers.PushCell { Title = "Crea orario", Separator = "no", Push = new AreaVip.CreaOrario(Preferences.Get("Classe", 0).ToString() + Preferences.Get("Corso", "")) };
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
                        //Display user details
                        utente = user;
                        nameLbl.Text = utente.nomeCognome;
                        classLbl.Text = utente.classeCorso;
                        userImg.Source = utente.Immagine;

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

                        //Save cache
                        Barrel.Current.Add("utenteLoggato", user, TimeSpan.FromDays(10));
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
            shouldClose = false;
            Navigation.PushAsync(new CambiaPassword());
        }

        private async void changePic(object sender, EventArgs e)
        {
            var decision = await DisplayActionSheet("Come vuoi procedere", "Annulla", "Rimuovi immagine", "Scegli una foto", "Scatta una foto");

            //Prevent modal closing
            MainPage.isSelectingImage = true;

            //Check permissions
            bool garanted = await Helpers.CameraPermissions.checkPermissions();
            if (!garanted)
                await DisplayAlert("Attenzione", "Non ci hai permesso di accedere alla tua fotocamera o alla tua galleria", "Ok");

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
                }
            }
            catch
            {
                await DisplayAlert("Errore", "Non è stato possibile completare l'azione, contattaci se il problema persiste", "Ok");
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

                //Siri shortcuts
                if (cell.Title == "Scorciatoie di Siri")
                {
#if __IOS__
                    //Treni Button
                    var TreniAction = UIAlertAction.Create("Treni", UIAlertActionStyle.Default, (action) =>
                    {
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
                    //if (utente.id != 2106)
                    //{
                    //    controller.View.TintColor = UIColor.Black;
                    //}
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
    }

    public class UIAlertControllerExtension : UIAlertController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Create("Quale scorciatoia vuoi modificare", null, UIAlertControllerStyle.Alert);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.TintColor = UIColor.Black;
        }
    }
}
