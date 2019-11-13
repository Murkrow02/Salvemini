using System;
using System.Collections.Generic;
using Forms9Patch;
using Rg.Plugins.Popup;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using System.Linq;
using Plugin.Toasts;
using MonkeyCache.SQLite;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif

namespace SalveminiApp.ArgoPages
{
    public partial class CompitiArgomenti : ContentPage
    {

        public List<RestApi.Models.Compiti> Compitis = new List<RestApi.Models.Compiti>();
        public List<RestApi.Models.Argomenti> Argomentis = new List<RestApi.Models.Argomenti>();
        public bool showingAll = false;
        public bool reversed = false;
        public string type;
        public string noCompiti = "Oggi non è stato assegnato niente, clicca sull'icona dell'orologio per vedere tutti i compiti";
        public string noArgomenti = "Oggi non è stato spiegato niente, clicca sull'icona dell'orologio per vedere tutti gli argomenti";

        public CompitiArgomenti(string tipo)
        {
            InitializeComponent();

            //Argomenti o compiti
            type = tipo;
            if (type == "compiti") //Compiti
            {
                //Set layout
                clockLabel.Text = "Assegnati oggi";
                gradient.BackgroundGradientStartColor = Color.FromHex("03F829");
                gradient.BackgroundGradientEndColor = Color.FromHex("20B4C7");
                TitleLbl.Text = "Compiti";
                shadowImage.Source = "CompitiShadow.svg";
                searchFrame.BackgroundColor = Color.FromHex("119A46");
                xClose.TextColor = Color.FromHex("03F829");

                //Get cache
                var cachedCompiti = CacheHelper.GetCache<List<RestApi.Models.Compiti>>("Compiti");
                if (cachedCompiti != null)
                {
                    //Fill list with cached
                    Compitis = cachedCompiti;
                    FillListToday();
                }
            }
            else //Argomenti
            {
                //Set layout
                clockLabel.Text = "Spiegati oggi";
                gradient.BackgroundGradientStartColor = Color.FromHex("FF7272");
                gradient.BackgroundGradientEndColor = Color.FromHex("FACA6F");
                TitleLbl.Text = "Argomenti";
                shadowImage.Source = "ArgomentiShadow.svg";
                searchFrame.BackgroundColor = Color.FromHex("FFBF96");
                xClose.TextColor = Color.FromHex("FF7272");

                //Get cache
                var cachedArgomenti = CacheHelper.GetCache<List<RestApi.Models.Argomenti>>("Argomenti");
                if (cachedArgomenti != null)
                {
                    //Fill list with cached
                    Argomentis = cachedArgomenti;
                    FillListToday();
                }
            }

            //Hide status bar and iPhone X optimization
#if __IOS__
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            //UIApplication.SharedApplication.StatusBarHidden = true;
            if (iOS.AppDelegate.HasNotch)
                fullLayout.Padding = new Thickness(20, 35, 20, 25);
#endif

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            listFrame.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
        }




        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Tip iniziale
            var firstPopUp = new Helpers.PopOvers().defaultPopOver;
            firstPopUp.Content = new Xamarin.Forms.Label { Text = "Clicca per visualizzare" + Environment.NewLine + compArg(), TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            firstPopUp.PointerDirection = PointerDirection.Up;
            firstPopUp.PreferredPointerDirection = PointerDirection.Up;
            firstPopUp.Target = clockButton;
            if (type == "compiti")
                firstPopUp.BackgroundColor = Color.FromHex("#00D10D");
            else
                firstPopUp.BackgroundColor = Color.FromHex("FFC400");
            firstPopUp.Disappearing += Second_PoUp;

            //Start downloading things
            try
            {
                //Check connection status
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    //COMPITI
                    if (type == "compiti")
                    {
                        //First time?
                        if (!Preferences.Get("firstTimeCompiti", true))
                        {
                            firstPopUp.IsVisible = true; //Show tip
                        }
                        else
                        {
                            lista.IsRefreshing = true;
                        }

                        //Download compiti from api
                        var datas = await App.Argo.GetCompiti();

                        //Detect if call returned a message of error
                        if (!string.IsNullOrEmpty(datas.Message))
                        {
                            //Error occourred, notify the user
                            Costants.showToast(datas.Message);
                            //Stop loading list
                            lista.IsRefreshing = false;
                            return;
                        }

                        //Deserialize new object
                        Compitis = datas.Data as List<RestApi.Models.Compiti>;

                        //Fill List
                        FillListToday();
                        lista.IsRefreshing = false;
                    }
                    //ARGOMENTI
                    else
                    {

                        //First time?
                        if (!Preferences.Get("firstTimeArgomenti", true))
                        {
                            firstPopUp.IsVisible = true;
                            lista.IsRefreshing = true;
                        }
                        else
                        {
                            lista.IsRefreshing = true;
                        }


                        //Download compiti from api
                        var datas = await App.Argo.GetArgomenti();

                        //Detect if call returned a message of error
                        if (!string.IsNullOrEmpty(datas.Message))
                        {
                            //Error occourred, notify the user
                            Costants.showToast(datas.Message);
                            //Stop loading list
                            lista.IsRefreshing = false;
                            return;
                        }

                        //Deserialize new object
                        Argomentis = datas.Data as List<RestApi.Models.Argomenti>;

                        //Fill List
                        FillListToday();
                        lista.IsRefreshing = false;
                    }
                }
                else
                {
                    //No internet
                    Costants.showToast("connection");
                    lista.IsRefreshing = false;
                }
            }
            catch //Random error
            {
                Costants.showToast("Non è stato possibile aggiornare i dati");
            }
        }



        void ShowAll(object sender, EventArgs e)
        {
            //Animate clock rotation
            clockButton.Rotation = 0.1;
            clockButton.RotateTo(-360, 700, Easing.CubicIn);

            //Detect types
            if (type == "compiti") //Compiti
            {
                if (showingAll) //Show only today
                {
                    FillListToday();
                    showingAll = false;
                    clockLabel.Text = "Assegnati oggi";
                }
                else //Show all
                {
                    clockLabel.Text = "Tutti";
                    if (Compitis.Count > 0)
                    {
                        lista.IsVisible = true;
                        lista.ItemsSource = Compitis;
                        emptyLayout.IsVisible = false;
                        filterBtn.IsEnabled = true;
                        sortBtn.IsEnabled = true;
                    }
                    else
                    {
                        lista.ItemsSource = Compitis;
                        emptyLayout.IsVisible = true;
                        filterBtn.IsEnabled = false;
                        sortBtn.IsEnabled = false;
                    }
                    showingAll = true;
                }
            }
            else //Argomenti
            {
                if (showingAll) //Show only today
                {
                    FillListToday();
                    showingAll = false;
                    clockLabel.Text = "Spiegati oggi";
                }
                else //Show all
                {
                    clockLabel.Text = "Tutti";
                    if (Argomentis.Count > 0)
                    {
                        lista.IsVisible = true;
                        lista.ItemsSource = Argomentis;
                        emptyLayout.IsVisible = false;
                        filterBtn.IsEnabled = true;
                        sortBtn.IsEnabled = true;
                    }
                    else
                    {
                        lista.ItemsSource = Argomentis;
                        placeholderLabel.Text = noArgomenti;
                        emptyLayout.IsVisible = true;
                        filterBtn.IsEnabled = false;
                        sortBtn.IsEnabled = false;
                    }
                    showingAll = true;
                }
            }


        }

        void ShowOrder(object sender, EventArgs e)
        {
            if (type == "compiti")
            {
                var list = (List<RestApi.Models.Compiti>)lista.ItemsSource;
                list.Reverse();
                lista.ItemsSource = null;
                lista.ItemsSource = list;
            }
            else
            {
                var list = (List<RestApi.Models.Argomenti>)lista.ItemsSource;
                list.Reverse();
                lista.ItemsSource = null;
                lista.ItemsSource = list;
            }


            if (!reversed)
            {
                filtersLbl.Text = "Dal più vecchio";
                reversed = true;
            }
            else
            {
                filtersLbl.Text = "Dal più recente";
                reversed = false;
            }

        }

        async void filterSubject(object sender, EventArgs e)
        {
            var materie = new List<string>();

            if (type == "compiti")
            {
                var listForSubjects = new List<RestApi.Models.Compiti>();
                if (showingAll)
                {
                    listForSubjects = Compitis;
                }
                else
                {
                    listForSubjects = Compitis.Where(x => x.datGiorno == DateTime.Now.ToString("yyyy-MM-dd")).ToList();
                }
                foreach (RestApi.Models.Compiti compito in listForSubjects)
                {
                    if (!materie.Contains(compito.Materia))
                    {
                        materie.Add(compito.Materia);
                    }
                }

                var scelta = await DisplayActionSheet("Filtra per materia", "Annulla", "Tutti", materie.ToArray());

                if (scelta == "Tutti")
                {
                    if (showingAll)
                    {
                        clockLabel.Text = "Tutti";
                    }
                    else
                    {
                        clockLabel.Text = "Assegnati oggi";
                    }

                    lista.ItemsSource = Compitis;
                    return;
                }

                if (scelta == "Annulla")
                    return;

                clockLabel.Text = scelta;
                lista.ItemsSource = Compitis.Where(x => x.Materia == scelta).ToList();
            }
            else
            {
                var listForSubjects = new List<RestApi.Models.Argomenti>();
                if (showingAll)
                {
                    listForSubjects = Argomentis;
                }
                else
                {
                    listForSubjects = Argomentis.Where(x => x.datGiorno == DateTime.Now.ToString("yyyy-MM-dd")).ToList();
                }
                foreach (RestApi.Models.Argomenti argomento in listForSubjects)
                {
                    if (!materie.Contains(argomento.Materia))
                    {
                        materie.Add(argomento.Materia);
                    }
                }

                var scelta = await DisplayActionSheet("Filtra per materia", "Annulla", "Tutti", materie.ToArray());

                if (scelta == "Tutti")
                {
                    if (showingAll)
                    {
                        clockLabel.Text = "Tutti";
                    }
                    else
                    {
                        clockLabel.Text = "Spiegati oggi";
                    }
                    lista.ItemsSource = Argomentis;
                    return;
                }

                if (scelta == "Annulla")
                    return;

                clockLabel.Text = scelta;
                lista.ItemsSource = Argomentis.Where(x => x.Materia == scelta).ToList();

            }
        }

        void Search(object sender, TextChangedEventArgs e)
        {
            if (type == "compiti")
            {
                if (string.IsNullOrEmpty(e.NewTextValue))
                    lista.ItemsSource = Compitis;
                else
                {
                    lista.ItemsSource = Compitis.Where(x => x.Contenuto.ToLower().Contains(e.NewTextValue.ToLower()) || x.Materia.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(e.NewTextValue))
                    lista.ItemsSource = Argomentis;
                else
                {
                    lista.ItemsSource = Argomentis.Where(x => x.Contenuto.ToLower().Contains(e.NewTextValue.ToLower()) || x.Materia.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
                }
            }

        }

        public void FillListToday()
        {
            if (type == "compiti")
            {
                //Get only today
                var today = Compitis.Where(x => x.datGiorno == DateTime.Now.ToString("yyyy-MM-dd")).ToList();
                if (today.Count > 0) //There is something today
                {
                    lista.IsVisible = true;
                    lista.ItemsSource = today;
                    emptyLayout.IsVisible = false;
                    filterBtn.IsEnabled = true;
                    sortBtn.IsEnabled = true;
                }
                else
                {
                    lista.ItemsSource = today;
                    placeholderLabel.Text = noCompiti;
                    emptyLayout.IsVisible = true;
                    filterBtn.IsEnabled = false;
                    sortBtn.IsEnabled = false;
                }
            }
            else
            {
                //Get only today
                var today = Argomentis.Where(x => x.datGiorno == DateTime.Now.ToString("yyyy-MM-dd")).ToList();
                if (today.Count > 0) //There is something today
                {
                    lista.IsVisible = true;
                    lista.ItemsSource = today;
                    emptyLayout.IsVisible = false;
                    filterBtn.IsEnabled = true;
                    sortBtn.IsEnabled = true;
                }
                else
                {
                    lista.ItemsSource = today;
                    placeholderLabel.Text = noArgomenti;
                    emptyLayout.IsVisible = true;
                    filterBtn.IsEnabled = false;
                    sortBtn.IsEnabled = false;
                }
            }

        }

        //String dynamic
        public string compArg()
        {
            if (type == "compiti")
                return "tutti i compiti";
            else
                return "tutti gli argomenti";
        }

        void Second_PoUp(object sender, EventArgs e)
        {
            var secondPopUp = new Helpers.PopOvers().defaultPopOver;
            secondPopUp.Content = new Xamarin.Forms.Label { Text = "Clicca per ordinare" + Environment.NewLine + "dai più vecchi ai più nuovi" + Environment.NewLine + "e viceversa", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            secondPopUp.IsVisible = true;
            secondPopUp.PointerDirection = PointerDirection.Up;
            secondPopUp.PreferredPointerDirection = PointerDirection.Up;
            secondPopUp.Target = sortBtn;
            if (type == "compiti")
                secondPopUp.BackgroundColor = Color.FromHex("#00D10D");
            else
                secondPopUp.BackgroundColor = Color.FromHex("FFC400"); secondPopUp.Disappearing += Third_PoUp;
        }

        void Third_PoUp(object sender, EventArgs e)
        {
            var thirdPopUp = new Helpers.PopOvers().defaultPopOver;
            thirdPopUp.Content = new Xamarin.Forms.Label { Text = "Clicca per filtrare" + Environment.NewLine + "per materia", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            thirdPopUp.IsVisible = true;
            thirdPopUp.PointerDirection = PointerDirection.Up;
            thirdPopUp.PreferredPointerDirection = PointerDirection.Up;
            thirdPopUp.Target = filterBtn;
            if (type == "compiti")
                thirdPopUp.BackgroundColor = Color.FromHex("#00D10D");
            else
                thirdPopUp.BackgroundColor = Color.FromHex("FFC400");

            //Change this to false before release 
            if (type == "compiti")
                Preferences.Set("firstTimeCompiti", true);
            else
                Preferences.Set("firstTimeArgomenti", true);
        }

        //Close modal
        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        //Reset all values on page closing
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Reset values
            if (type == "compiti")
                clockLabel.Text = "Assegnati oggi";
            else
                clockLabel.Text = "Spiegati oggi";
            filtersLbl.Text = "Dal più recente";

            showingAll = false;
            reversed = false;
        }
    }
}
