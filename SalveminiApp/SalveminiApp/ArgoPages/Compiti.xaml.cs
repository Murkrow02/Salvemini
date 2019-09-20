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
    public partial class Compiti : ContentPage
    {
        public List<RestApi.Models.Compiti> Compitis = new List<RestApi.Models.Compiti>();
        public bool showingAll = false;

        public Compiti()
        {
            InitializeComponent();
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
            UIApplication.SharedApplication.StatusBarHidden = true;
#endif
            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            compitiList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
        }

        void Close_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Tips (falli vedere solo la prima volta)
            var firstPopUp = new Helpers.PopOvers().defaultPopOver;
            firstPopUp.Content = new Xamarin.Forms.Label { Text = "Clicca per visualizzare" + Environment.NewLine + "tutti i compiti", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            firstPopUp.IsVisible = true;
            firstPopUp.PointerDirection = PointerDirection.Up;
            firstPopUp.PreferredPointerDirection = PointerDirection.Up;
            firstPopUp.Target = clockButton;
            firstPopUp.BackgroundColor = Color.FromHex("#00D10D");
            firstPopUp.Disappearing += Second_PoUp;

            //Start loading
            if (!Preferences.Get("firstTimeCompiti", true))
            compitiList.IsRefreshing = true;

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

                }

                //Fill List
                compitiList.ItemsSource = Compitis.Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
                compitiList.IsRefreshing = false;

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
            secondPopUp.BackgroundColor = Color.FromHex("#00D10D");
            secondPopUp.Disappearing += Third_PoUp; 
        }

        private void Third_PoUp(object sender, EventArgs e)
        {
            var thirdPopUp = new Helpers.PopOvers().defaultPopOver;
            thirdPopUp.Content = new Xamarin.Forms.Label { Text = "Clicca per filtrare" + Environment.NewLine + "per materia", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            thirdPopUp.IsVisible = true;
            thirdPopUp.PointerDirection = PointerDirection.Up;
            thirdPopUp.PreferredPointerDirection = PointerDirection.Up;
            thirdPopUp.Target = filterBtn;
            thirdPopUp.BackgroundColor = Color.FromHex("#00D10D");
            //Preferences.Set("firstTimeCompiti", false);
        }

        private  void ShowAll(object sender, EventArgs e)
        {
            clockButton.Rotation = 0.1;
             clockButton.RotateTo(-360, 800, Easing.CubicIn);

            if (showingAll)
            {
                compitiList.ItemsSource = Compitis.Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
                showingAll = false;
                clockLabel.Text = "Assegnati oggi";
            }
            else
            {
                clockLabel.Text = "Tutti";
                compitiList.ItemsSource = Compitis;
                showingAll = true;
            }

        }

        private void ShowOrder(object sender, EventArgs e)
        {
            var lista = (List<RestApi.Models.Compiti>)compitiList.ItemsSource;
            lista.Reverse();
            compitiList.ItemsSource = null;
            compitiList.ItemsSource = lista;
        }

       private async void filterSubject(object sender, EventArgs e)
        {
            var materie = new List<string>();
            foreach (RestApi.Models.Compiti compito in Compitis)
            {
                if (!materie.Contains(compito.Materia))
                {
                    materie.Add(compito.Materia);
                }
            }

             var scelta = await DisplayActionSheet("Filtra per materia", "Annulla", "Tutti", materie.ToArray());

            if(scelta == "Tutti")
            {
                clockLabel.Text = "Tutti";
                compitiList.ItemsSource = Compitis;
                return;
            }

            if (scelta == "Annulla")
                return;

            clockLabel.Text = scelta;
            compitiList.ItemsSource = Compitis.Where(x => x.Materia == scelta).ToList();
        }

        private void Search(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.OldTextValue))
                compitiList.ItemsSource = Compitis;
            else
            {
                compitiList.ItemsSource = Compitis.Where(x => x.desCompiti.ToLower().Contains(e.OldTextValue.ToLower()) || x.Materia.ToLower().Contains(e.OldTextValue.ToLower())).ToList();
            }
        }

    }
}
