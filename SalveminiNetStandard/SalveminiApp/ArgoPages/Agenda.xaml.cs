﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Rg.Plugins.Popup.Extensions;

namespace SalveminiApp.ArgoPages
{
    public partial class Agenda : ContentPage
    {
        public Agenda()
        {
            InitializeComponent();

            //iPhone X optimization
            if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
            {
                mainLayout.Padding = new Thickness(0, 38, 0, 0);
            }

            //Auto select day
            var tomowrrowInt = Convert.ToInt32(DateTime.Now.DayOfWeek) + 1;
            if (tomowrrowInt == 7) tomowrrowInt = 1; //Take monday if saturday

            //Fill dropdown with days
            days.DataSource = Costants.getDays();
            days.SelectedIndex = tomowrrowInt - 1;
            days.Text = Costants.getDays()[tomowrrowInt - 1];
        }


        private async void giorno_Selected(object sender, EventArgs e)
        {
            downloadCompiti(days.SelectedIndex + 1);
        }

        public async void downloadCompiti(int day)
        {
            //Check internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection"); return;
            }

            //Download compiti
            loading.IsRunning = true; loading.IsVisible = true;
            var compiti = await App.Agenda.GetCompitiAgenda(day);

            if (compiti == null) //Error
            {
                compitiLayout.Children.Clear();
                Costants.showToast("Non è stato possibile scaricare i compiti per il giorno selezionato"); loading.IsRunning = false; loading.IsVisible = false; return;
            }

            //Success
            var nextDay = Costants.GetNextWeekday((DayOfWeek)(Enum.Parse(typeof(DayOfWeek), (days.SelectedIndex + 1).ToString()))); //Get next day occourrence
            var deletedList = CacheHelper.GetCache<List<string>>("deletedCompiti"); //Get deleted materie
            var customAdded = CacheHelper.GetCache<List<CustomCompito>>("customAdded" + nextDay.ToString("ddMMyyyy")); //Get custom added materie
            //Fill with custom
            if(customAdded != null)
            {
                foreach (var custom in customAdded) { compiti.Add(new RestApi.Models.Compiti { Materia = custom.Materia, desCompiti = custom.Compiti }); }
            }
            //Fill layout
            compitiLayout.Children.Clear(); //Clear previous children
            for (int i = 0; i < compiti.Count; i++)
            {
                if (deletedList == null || !deletedList.Contains(compiti[i].Materia + compiti[i].desCompiti)) //Remove deleted materie
                    compitiLayout.Children.Add(new Controls.CompitoAgenda(compitiLayout) { Title = compiti[i].Materia, Desc = compiti[i].desCompiti, FrameColor = Costants.ColorLoop(i).ToHex() });
            }
            loading.IsRunning = false; loading.IsVisible = false;

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Auto download per domani
            downloadCompiti(days.SelectedIndex + 1);
        }


       async void reset_Cached(object sender, EventArgs e)
        {
            var choice = await DisplayAlert("Attenzione", "Sei sicuro di voler ripristinare i compiti che hai eliminato?", "Si", "No");
            if (!choice)
                return;
            Barrel.Current.Empty("deletedCompiti");
            downloadCompiti(days.SelectedIndex + 1);
            Costants.showToast("I compiti che avevi eliminato sono stati riaggiunti");
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        public void add_Clicked(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new Helpers.Popups.AggiungiCompitoAgenda(days.SelectedIndex + 1, compitiLayout));
        }
    }

    public class CustomCompito
    {
        public string Materia { get; set; }
        public string Compiti { get; set; }
    }
}
