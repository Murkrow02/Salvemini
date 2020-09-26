using System;
using System.Collections.Generic;
using System.Linq;
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


            if (Device.RuntimePlatform == Device.iOS)
            {
                if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
                {

                    mainLayout.Padding = new Thickness(20, 42);

                }


                //Fix spacing only on iOS
                headerLayout.Spacing = 15;
                headerLayout.Margin = new Thickness(0);
            }


            //Fill picker
            stationPicker.ItemsSource = Stazioni;

            //Try intelligent autoselect
            try
            {
                stationPicker.SelectedIndex = Preferences.Get("savedStation", -1);
                TrenoSegment.SelectedSegment = Convert.ToInt32(Preferences.Get("savedDirection", false));
            }
            catch (Exception ex)
            {
                //Fa niente
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
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

        private void Safari_Clicked(object sender, EventArgs e)
        {
            var page = new IconNavigationPage(new ContentPage { Title = "Orari treni", Content = new WebView { Source = "https://www.eavsrl.it/web/sites/default/files/eavferro/NAPOLI%20SORRENTO%20L1.pdf" } }) { BarBackgroundColor = Color.FromHex("FA8265"), BarTextColor = Color.White };
            page.ToolbarItems.Add(new IconToolbarItem { IconImageSource = "fas-times", IconColor = Color.White });
            page.ToolbarItems[0].Clicked += WebClose_Clicked;
            Navigation.PushModalAsync(page);
        }

        private void WebClose_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
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

        void Starred_Clicked(object sender, EventArgs e)
        {
            if (stationPicker.SelectedItem != null && TrenoSegment.SelectedSegment != -1)
            {
                (sender as IconButton).Text = "check-circle";

                var station = Costants.Stazioni.FirstOrDefault(x => x.Value == stationPicker.SelectedItem.ToString()).Key;
                var direction = Convert.ToBoolean(TrenoSegment.SelectedSegment);

                //Save new preferences
                Preferences.Set("savedStation", station);
                Preferences.Set("savedDirection", direction);


                //Save values for siri intent
#warning sirikit
                //var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
                //defaults.AddSuite("group.com.codex.SalveminiApp");
                //defaults.SetInt(station, new NSString("SiriStation"));
                //defaults.SetBool(direction, new NSString("SiriDirection"));


                //Reload main page
                MainPage.forceAppearing = true;
            }
        }
    }
}
