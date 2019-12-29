using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Plugin.Segmented.Control;
using Plugin.Segmented.Event;

using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews.Trasporti
{
    public partial class Bus : ContentPage
    {
        public List<RestApi.Models.Linea> SitaSource = new List<RestApi.Models.Linea>();
        public List<RestApi.Models.Linea> EavSource = new List<RestApi.Models.Linea>();

        public Bus()
        {
            InitializeComponent();

            //IphoneX optimization
#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(20, 50);
            }
#endif
            SitaSource.Add(new RestApi.Models.Linea
            {
                Name = "S.Agata-Massa-Sorrento",
                Buses = new List<RestApi.Models.Bus> { new RestApi.Models.Bus { Partenza = "16:30", Stazione = 0, Direzione = true, Variazioni = "F" } },
                Stazioni = new Dictionary<int, string>
                {
                    {0, "Sorrento"},
                    {1, "Massa Lubrense"},
                    {2, "Bivio Tigliano"},
                    {3, "S.Agata"},
                    {4, "Torca"}
                }
            });

            EavSource.Add(new RestApi.Models.Linea { Name = "S.Agata-Massa-PEDO", Buses = new List<RestApi.Models.Bus> { new RestApi.Models.Bus { Partenza = "PEDOFILO", Stazione = 0, Direzione = true, Variazioni = "F" } } });

            linePicker.ItemsSource = SitaSource;
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void TypeChanged(object sender, SegmentSelectEventArgs e)
        {
            stationPicker.IsEnabled = false;
            directionSegment.IsEnabled = false;

            //Set itemssource of lines basing on segment
            linePicker.ItemsSource = e.NewValue == 0 ? SitaSource : EavSource;
        }

        private void StationPicker_Unfocused(object sender, FocusEventArgs e)
        {
            //Check if a start station is selected
            if (stationPicker.SelectedItem != null)
            {
                //Enable direction segment control
                directionSegment.IsEnabled = true;
            }
        }

        void linePicker_Unfocused(object sender, FocusEventArgs e)
        {
            //Check line is selected
            if (linePicker.SelectedItem != null && (linePicker.SelectedItem as RestApi.Models.Linea).Stazioni != null)
            {
                //Cast the selection as line
                var data = linePicker.SelectedItem as RestApi.Models.Linea;

                //Fill the picker with stations
                stationPicker.ItemsSource = data.Stazioni.Values.ToList();

                //Re-enable the picker
                stationPicker.IsEnabled = true;

                //Set values to the direction segmented control
                directionSegment.Children[0].Text = data.Stazioni[0];
                directionSegment.Children[1].Text = data.Stazioni[data.Stazioni.Count - 1];
            }
        }
    }
}
