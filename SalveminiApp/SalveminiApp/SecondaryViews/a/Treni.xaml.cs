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

namespace SalveminiApp.SecondaryViews.Trasporti
{
    public partial class Treni : ContentPage
    {
        public List<RestApi.Models.Treno> Trains = new List<RestApi.Models.Treno>();
        public List<string> Stazioni = new List<string>(Costants.Stazioni.Values);

        public Treni()
        {
            InitializeComponent();
            stationPicker.ItemsSource = Stazioni;
#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(20, 50);
            }
#endif

        }

        private void picker_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(stationPicker.SelectedItem?.ToString()))
                return;
            //Sorrento to sorrento
            if (stationPicker.SelectedItem.ToString() == "Sorrento" && TrenoSegment.SelectedSegment == 0)
                TrenoSegment.SelectedSegment = 1;

            //Napoli to napoli
            if (stationPicker.SelectedItem.ToString() == "Napoli Porta Nolana" && TrenoSegment.SelectedSegment == 1)
                TrenoSegment.SelectedSegment = 0;

            getTrains();
        }

        private void TrenoSegment_OnSegmentSelected(object sender, Plugin.Segmented.Event.SegmentSelectEventArgs e)
        {

            if (string.IsNullOrEmpty(stationPicker.SelectedItem?.ToString()))
                return;

            //Sorrento to sorrento
            if (stationPicker.SelectedItem.ToString() == "Sorrento" && TrenoSegment.SelectedSegment == 0)
                TrenoSegment.SelectedSegment = 1;

            //Napoli to napoli
            if (stationPicker.SelectedItem.ToString() == "Napoli Porta Nolana" && TrenoSegment.SelectedSegment == 1)
                TrenoSegment.SelectedSegment = 0;

            getTrains();

        }

        async void getTrains()
        {
            if (stationPicker.SelectedItem != null)
            {
                bool direction = Convert.ToBoolean(TrenoSegment.SelectedSegment);
                Trains = await App.Treni.GetTrains(Costants.Stazioni.FirstOrDefault(x => x.Value == stationPicker.SelectedItem.ToString()).Key, direction);
                if (Trains != null)
                {
                    treniList.IsVisible = true;
                    noTrainLayout.IsVisible = false;
                    treniList.ItemsSource = Trains;
                }
            }
        }

        void Close_Page(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
