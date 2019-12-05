using System;
using System.Collections.Generic;
using MonkeyCache.SQLite;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace SalveminiApp.Helpers.Popups
{
    public partial class AggiungiCompitoAgenda : PopupPage
    {
        int giorno = 1;
        StackLayout layout;
        public AggiungiCompitoAgenda(int giorno_, StackLayout parent)
        {
            InitializeComponent();

            giorno = giorno_;
            layout = parent;
            //Init Interface
            assenzeImage.HeightRequest = App.ScreenWidth / 13;
            assenzeImage.WidthRequest = App.ScreenWidth / 13;
        }

        async void add_Clicked(object sender, System.EventArgs e)
        {
            //Compiti or materia not set
            if (string.IsNullOrEmpty(materia.Text) || string.IsNullOrEmpty(compiti.Text))
                return;

            //Get next day with dayofweek
           var nextDay = Costants.GetNextWeekday((DayOfWeek)(Enum.Parse(typeof(DayOfWeek), giorno.ToString())));
            //Get list of custom of that day
            var customAdded = CacheHelper.GetCache<List<ArgoPages.CustomCompito>>("customAdded" + nextDay.ToString("ddMMyyyy"));
            //If there is no one create a new one
            if (customAdded == null) customAdded = new List<ArgoPages.CustomCompito>();
            //Add element to list
            customAdded.Add(new ArgoPages.CustomCompito { Compiti = compiti.Text, Materia = materia.Text });
            //Add new list to cache
            Barrel.Current.Add<List<ArgoPages.CustomCompito>>("customAdded" + nextDay.ToString("ddMMyyyy"),customAdded,TimeSpan.FromDays(30));

            //Add new element to layout
            layout.Children.Add(new Controls.CompitoAgenda(layout) { Title = materia.Text, Desc = compiti.Text, FrameColor = Costants.RandomColor().ToHex() });
            await Navigation.PopPopupAsync();
        }


    }
}
