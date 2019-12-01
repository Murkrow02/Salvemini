using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
#if __IOS__
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.ArgoPages
{
    public partial class Agenda : ContentPage
    {
        public Agenda()
        {
            InitializeComponent();

            //iPhone X optimization
#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 38, 0, 0);
            }
#endif
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
            compitiLayout.Children.Clear(); //Clear previous children
            var deletedList = CacheHelper.GetCache<List<string>>("deletedCompiti"); //Get deleted materie
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


        void reset_Cached(object sender, EventArgs e)
        {
            Barrel.Current.Empty("deletedCompiti");
            downloadCompiti(days.SelectedIndex + 1);
            Costants.showToast("I compiti che avevi eliminato sono stati riaggiunti");
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }


    }
}
