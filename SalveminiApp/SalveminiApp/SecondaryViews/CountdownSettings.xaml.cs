using System;
using System.Collections.Generic;
using Plugin.Segmented.Event;
using Xamarin.Forms;
using Xamarin.Essentials;
namespace SalveminiApp.SecondaryViews
{
    public partial class CountdownSettings : ContentPage
    {
        public DateTime FineDellaScuola = new DateTime(2020, 6, 6, 13, 40, 0);
        public DateTime Natale = new DateTime(2019, 12, 25, 0, 0, 0);
        public DateTime Pasqua = new DateTime(2020, 04, 12, 0, 0, 0);

        public CountdownSettings()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Select Segment of the countdown
            int pointerInt = 0;
            switch (Preferences.Get("DateToPoint", FineDellaScuola).ToString("dd-MM-yyyy"))
            {
                //case "25-12-2019":
                //    //Natale
                //    pointerInt = 1;
                //    break;
                case "12-04-2020":
                    //Pasqua
                    pointerInt = 2;
                    break;
            }
            //Select Segment
            pointerSegment.SelectedSegment = pointerInt;

            //Toggle switch to count holydays
            holydaysSwitch.IsToggled = !Preferences.Get("CountHolidays", false);
        }

        bool firstSelection = true;
        public void DateSegment_SelectionChanged(object sender, SegmentSelectEventArgs e)
        {
            //Skip first selection cause it saves the wrong thing
            if (firstSelection)
            {
                firstSelection = false;
                return;
            }
            var dateToSave = new DateTime();
            switch (e.NewValue)
            {
                case 0:
                    //Fine della scuola
                    dateToSave = FineDellaScuola;
                    holydaysSwitch.IsEnabled = true;

                    break;
                //case 1:
                //    //Natale
                //    dateToSave = Natale;
                //    holydaysSwitch.IsToggled = false;
                //    holydaysSwitch.IsEnabled = false;
                //    break;
                case 1:
                    //Pasqua
                    dateToSave = Pasqua;
                    holydaysSwitch.IsToggled = false;
                    holydaysSwitch.IsEnabled = false;
                    break;
            }

            //Aggiorna la data a cui deve puntare il countdown
            Preferences.Set("DateToPoint", dateToSave);
        }

        public void HolydaysSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("CountHolidays", !e.Value);
        }
    }
}
