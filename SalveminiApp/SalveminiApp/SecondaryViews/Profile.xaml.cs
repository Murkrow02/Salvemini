using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.SecondaryViews
{
    public partial class Profile : ContentPage
    {

        public RestApi.Models.Utente utente = new RestApi.Models.Utente();

        public Profile()
        {
            InitializeComponent();
            

            //Get cache
            if (Barrel.Current.Exists("utenteLoggato"))
            {
                utente = Barrel.Current.Get<RestApi.Models.Utente>("utenteLoggato");
                nameLbl.Text = utente.nomeCognome;
                classLbl.Text = utente.classeCorso;
            }


            //Create tapped gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            //Fill lists
            //Personalizza
            var notifiche = new Helpers.PushCell { Title = "Notifiche", Separator = "si" };
            notifiche.GestureRecognizers.Add(tapGestureRecognizer);
            persLayout.Children.Add(notifiche);
            var countdown = new Helpers.PushCell { Title = "Countdown", Separator = "No" };
            countdown.GestureRecognizers.Add(tapGestureRecognizer);
            persLayout.Children.Add(countdown);

            //Contattaci
            var insta = new Helpers.PushCell { Title = "Il nostro team", Separator = "si" ,Push = new SecondaryViews.Team()};
            insta.GestureRecognizers.Add(tapGestureRecognizer);
            contactLayout.Children.Add(insta);
            var Mail = new Helpers.PushCell { Title = "Mail", Separator = "No" };
            countdown.GestureRecognizers.Add(tapGestureRecognizer);
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Get current user
                var user = await App.Utenti.GetUtente(Preferences.Get("UserId", 0));
                if (user != null)
                {
                    //Display cose
                    utente = user;
                    nameLbl.Text = utente.nomeCognome;
                    classLbl.Text = utente.classeCorso;
                    userImg.Source = utente.Immagine;

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

            //Ios 13 bug
            try
            {
                Navigation.PopModalAsync();
            }
            catch
            {
                //fa nient
            }
        }

        private void esci_Clicked(object sender, EventArgs e)
        {
            //Perform logout
            Xamarin.Forms.Application.Current.MainPage = new FirstAccess.Login();
            Preferences.Set("UserId", 0);
            Preferences.Set("Token", "");
        }
    }
}
