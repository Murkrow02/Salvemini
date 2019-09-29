using System;
using System.Collections.Generic;
using System.Linq;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;

#endif
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.Iconize;

namespace SalveminiApp.SecondaryViews.BusETreni
{
    public partial class Treni : ContentPage
    {
        public List<RestApi.Models.Treno> Trains = new List<RestApi.Models.Treno>();
        public List<string> Stazioni = new List<string>(Costants.Stazioni.Values);

        public Treni()
        {
            InitializeComponent();
            stationPicker.ItemsSource = Stazioni;
            directionPicker.ItemsSource = new List<string> { Costants.Stazioni[0], Costants.Stazioni[Costants.Stazioni.Count - 1] };

            //stationFrame.WidthRequest = App.ScreenWidth / 1.8;
            //directonFrame.WidthRequest = App.ScreenWidth / 1.8;


        }

        private void picker_Unfocused(object sender, FocusEventArgs e)
        {
            getTrains();
        }

        private void Station_Selected(object sender, EventArgs e)
        {
            if (stationPicker.SelectedItem.ToString() == Costants.Stazioni[0])
            {
                directionPicker.SelectedItem = Costants.Stazioni[Costants.Stazioni.Count - 1];
            }
            else if (stationPicker.SelectedItem.ToString() == Costants.Stazioni[Costants.Stazioni.Count - 1])
            {
                directionPicker.SelectedItem = Costants.Stazioni[0];
            }
        }

        private void Direction_Selected(object sender, EventArgs e)
        {
            if (stationPicker.SelectedItem != null)
            {
                if (directionPicker.SelectedItem.ToString() == Costants.Stazioni[Costants.Stazioni.Count - 1] && stationPicker.SelectedItem.ToString() == Costants.Stazioni[Costants.Stazioni.Count - 1])
                {
                    directionPicker.SelectedItem = Costants.Stazioni[0];
                }

                if (directionPicker.SelectedItem.ToString() == Costants.Stazioni[0] && stationPicker.SelectedItem.ToString() == Costants.Stazioni[0])
                {
                    directionPicker.SelectedItem = Costants.Stazioni[Costants.Stazioni.Count - 1];
                }


            }
        }

        async void getTrains()
        {
            if (stationPicker.SelectedItem != null && directionPicker.SelectedItem != null)
            {
                bool direction = directionPicker.SelectedItem.ToString() == Costants.Stazioni[Costants.Stazioni.Count - 1];
                Trains = await App.Treni.GetTrains(Costants.Stazioni.FirstOrDefault(x => x.Value == stationPicker.SelectedItem.ToString()).Key, direction);
                if (Trains != null)
                {
                    treniList.IsVisible = true;
                    noTrainLayout.IsVisible = false;
                    treniList.ItemsSource = Trains;
                    starButton.Text = "star";
                }
            }
        }

        void Starred_Clicked(object sender, EventArgs e)
        {
            if (stationPicker.SelectedItem != null && directionPicker.SelectedItem != null)
            {
                (sender as IconButton).Text = "check-circle";
                Preferences.Set("savedStation", Costants.Stazioni.FirstOrDefault(x => x.Value == stationPicker.SelectedItem.ToString()).Key);
                bool direction = directionPicker.SelectedItem.ToString() == Costants.Stazioni[Costants.Stazioni.Count - 1];
                Preferences.Set("savedDirection", direction);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
        }

        void Close_Page(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
