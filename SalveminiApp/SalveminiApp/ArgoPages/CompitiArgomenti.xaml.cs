using System;
using System.Collections.Generic;
using Forms9Patch;
using Rg.Plugins.Popup;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using System.Linq;
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

        public CompitiArgomenti(string tipo)
        {
            InitializeComponent();

            //Argomenti o compiti
            type = tipo;
            if (type == "compiti")
            {
                clockLabel.Text = "Assegnati oggi";
                gradient.BackgroundGradientStartColor = Color.FromHex("0DC637");
                gradient.BackgroundGradientEndColor = Color.FromHex("54D991");
                TitleLbl.Text = "Compiti";
                shadowImage.Source = "CompitiShadow.svg";
                searchFrame.BackgroundColor = Color.FromHex("119A46");
                xClose.TextColor = Color.FromHex("21db18");
            }
            else
            {
                clockLabel.Text = "Spiegati oggi";
                gradient.BackgroundGradientStartColor = Color.FromHex("FFC400");
                gradient.BackgroundGradientEndColor = Color.FromHex("FFEA4B");
                TitleLbl.Text = "Argomenti";
                shadowImage.Source = "ArgomentiShadow.svg";
                searchFrame.BackgroundColor = Color.FromHex("ffd500");
                xClose.TextColor = Color.FromHex("FFC400");

            }

            //Hide status bar
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif
            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            lista.HeightRequest = App.ScreenHeight / 1.55;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
        }

        //Close modal
        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
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

            //COMPITI
            if (type == "compiti")
            {

                //First time?
                if (!Preferences.Get("firstTimeCompiti", true))
                {
                    firstPopUp.IsVisible = true;
                }
                else
                {
                    lista.IsRefreshing = true;
                }

                //Api Call
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    var datas = await App.Argo.GetCompiti();
                    if (string.IsNullOrEmpty(datas.Message))
                    {
                        Compitis = datas.Data as List<RestApi.Models.Compiti>;
                    }
                    else
                    {
                        await DisplayAlert("Errore", datas.Message, "Ok");
                        return;
                    }

                    //Fill List
                    lista.ItemsSource = Compitis.Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
                    lista.IsRefreshing = false;
                }


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

                //Api Call
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    var datas = await App.Argo.GetArgomenti();
                    if (string.IsNullOrEmpty(datas.Message))
                    {
                        Argomentis = datas.Data as List<RestApi.Models.Argomenti>;
                    }
                    else
                    {
                        await DisplayAlert("Errore", datas.Message, "Ok");
                        return;
                    }

                    //Fill List
                    lista.ItemsSource = Argomentis.Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
                    lista.IsRefreshing = false;
                }

            }
        }

        private void Second_PoUp(object sender, EventArgs e)
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

        private void Third_PoUp(object sender, EventArgs e)
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

        private void ShowAll(object sender, EventArgs e)
        {
            clockButton.Rotation = 0.1;
            clockButton.RotateTo(-360, 800, Easing.CubicIn);

            if (type == "compiti")
            {
                if (showingAll)
                {
                    lista.ItemsSource = Compitis.Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
                    showingAll = false;
                    clockLabel.Text = "Assegnati oggi";
                }
                else
                {
                    clockLabel.Text = "Tutti";
                    lista.ItemsSource = Compitis;
                    showingAll = true;
                }
            }
            else
            {
                if (showingAll)
                {
                    lista.ItemsSource = Argomentis.Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
                    showingAll = false;
                    clockLabel.Text = "Spiegati oggi";
                }
                else
                {
                    clockLabel.Text = "Tutti";
                    lista.ItemsSource = Argomentis;
                    showingAll = true;
                }
            }


        }

        private void ShowOrder(object sender, EventArgs e)
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

        private async void filterSubject(object sender, EventArgs e)
        {
            var materie = new List<string>();

            if (type == "compiti")
            {
                foreach (RestApi.Models.Compiti compito in Compitis)
                {
                    if (!materie.Contains(compito.Materia))
                    {
                        materie.Add(compito.Materia);
                    }
                }

                var scelta = await DisplayActionSheet("Filtra per materia", "Annulla", "Tutti", materie.ToArray());

                if (scelta == "Tutti")
                {
                    clockLabel.Text = "Tutti";
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
                foreach (RestApi.Models.Argomenti argomento in Argomentis)
                {
                    if (!materie.Contains(argomento.Materia))
                    {
                        materie.Add(argomento.Materia);
                    }
                }

                var scelta = await DisplayActionSheet("Filtra per materia", "Annulla", "Tutti", materie.ToArray());

                if (scelta == "Tutti")
                {
                    clockLabel.Text = "Tutti";
                    lista.ItemsSource = Argomentis;
                    return;
                }

                if (scelta == "Annulla")
                    return;

                clockLabel.Text = scelta;
                lista.ItemsSource = Argomentis.Where(x => x.Materia == scelta).ToList();

            }
        }

        private void Search(object sender, TextChangedEventArgs e)
        {
            if (type == "compiti")
            {
                if (string.IsNullOrEmpty(e.OldTextValue))
                    lista.ItemsSource = Compitis;
                else
                {
                    lista.ItemsSource = Compitis.Where(x => x.Contenuto.ToLower().Contains(e.OldTextValue.ToLower()) || x.Materia.ToLower().Contains(e.OldTextValue.ToLower())).ToList();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(e.OldTextValue))
                    lista.ItemsSource = Argomentis;
                else
                {
                    lista.ItemsSource = Argomentis.Where(x => x.Contenuto.ToLower().Contains(e.OldTextValue.ToLower()) || x.Materia.ToLower().Contains(e.OldTextValue.ToLower())).ToList();
                }
            }
               
        }


        public string compArg()
        {
            if (type == "compiti")
                return "tutti i compiti";
            else
                return "tutti gli argomenti";
        }

    }
}
