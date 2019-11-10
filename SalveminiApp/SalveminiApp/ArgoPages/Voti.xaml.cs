using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Forms9Patch;
using Syncfusion.SfChart.XForms;
using System.Linq;
using System.Reflection;
using Plugin.Toasts;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif
using Xamarin.Forms;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using System.Globalization;

namespace SalveminiApp.ArgoPages
{
    public partial class Voti : ContentPage
    {
        public static ObservableCollection<RestApi.Models.GroupedVoti> GroupedVoti = new ObservableCollection<RestApi.Models.GroupedVoti>();
        public static double TotalMedia;

        private int _myProperty;
        public Voti()
        {
            InitializeComponent();
#if __IOS__
           // On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True);
           // UIApplication.SharedApplication.StatusBarHidden = true;
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FullScreen);
            if (iOS.AppDelegate.HasNotch)
                fullLayout.Padding = new Thickness(20, 35, 20, 25);

#endif
            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            votiList.HeightRequest = App.ScreenHeight / 1.5;
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Get cache
            var cachedVoti = CacheHelper.GetCache<ObservableCollection<RestApi.Models.GroupedVoti>>("Voti");
            if (cachedVoti != null)
                votiList.ItemsSource = GroupedVoti;

            //Subscribe to messaging center
            MessagingCenter.Subscribe<App, double>(this, "TotalMediaChanged", (sender, arg) =>
            {
                fullMediaLabel.Text = "Media totale: " + string.Format("{0:0.00}", arg);
            });
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            votiList.IsRefreshing = true;

            //Check internet connection
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Download voti from api
                    var response = await App.Argo.GetVoti();

                    //Detect if call returned a message of error
                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        //Error occourred, notify the user
                        Costants.showToast(response.Message);
                        //Stop loading list
                        votiList.IsRefreshing = false;
                        return;
                    }

                    //Deserialize object
                    GroupedVoti = response.Data as ObservableCollection<RestApi.Models.GroupedVoti>;

                    //Update list
                    votiList.ItemsSource = GroupedVoti;

                    //Calculate media
                    RestApi.Models.GroupedVoti.calcTotalMedia();
                }
                catch
                {
                    Costants.showToast("Si è verificato un errore");
                }
            }
            else //No connection
            {
                Costants.showToast("connection");
            }
            votiList.IsRefreshing = false;
        }

        void NonFaMedia_Clicked(object sender, System.EventArgs e)
        {
            var menuItem = sender as MenuItem;
            menuItem.Text = menuItem.Text == "Non fa media" ? menuItem.Text = "Fa media" : menuItem.Text = "Non fa media";
            menuItem.IsDestructive = !menuItem.IsDestructive;
        }

        void Cell_Tapped(object sender, System.EventArgs e)
        {
            //Get The mark string
            var codVoto = ((((sender as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.Label).Text;
            //Get the date of the mark
            var Data = ((((sender as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.StackLayout).Children[1] as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.Label).Text;
            //Get the teacher of the mark
            var docente = ((((sender as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.StackLayout).Children[1] as Xamarin.Forms.StackLayout).Children[1] as Xamarin.Forms.Label).Text;
            //Get the subject of the mark
            var materia = ((sender as Xamarin.Forms.StackLayout).Children[1] as Xamarin.Forms.Label).Text;

            //Get The Mark
            var Voto = GroupedVoti.FirstOrDefault(x => x.Materia == materia).FirstOrDefault(x => x.codVoto == codVoto && x.Data == Data && x.docente == docente);

            //Get the three dots of the cell
            var pallini = (((sender as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.StackLayout).Children[1] as Xamarin.Forms.StackLayout;

            //Check if the comment exists
            if (!string.IsNullOrEmpty(Voto.desCommento))
            {
                //Display Popup with the comment
                var secondPopUp = new Helpers.PopOvers().defaultPopOver;
                secondPopUp.Content = new Xamarin.Forms.Label { Text = Voto.desCommento, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, WidthRequest = App.ScreenWidth / 1.5 };
                secondPopUp.PointerDirection = PointerDirection.Vertical;
                secondPopUp.PreferredPointerDirection = PointerDirection.Vertical;
                secondPopUp.Target = pallini;
                secondPopUp.BackgroundColor = Color.White;
                secondPopUp.IsVisible = true;
            }
        }

        void Chart_Clicked(object sender, System.EventArgs e)
        {
            //Get the button
            Plugin.Iconize.IconButton chartButton = sender as Plugin.Iconize.IconButton;

            //Get the subject
            var materia = ((chartButton.Parent as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.Label).FormattedText.Spans[0].Text;

            //Get the marks to display in the chart
            var source = new List<RestApi.Models.Voti>();
            try
            {
                source = GroupedVoti.FirstOrDefault(x => x.Materia == materia).Voti.OrderBy(x => DateTime.ParseExact(x.datGiorno, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();
            }
            catch
            {
                source = GroupedVoti.FirstOrDefault(x => x.Materia == materia).Voti.ToList(); //Failed to order in chronological
            }

            //Create popover layout
            var layout = new Xamarin.Forms.StackLayout { Spacing = 0 };

            //Create the chart
            SfChart chart = new SfChart();
            chart.Title.Text = materia;
            chart.Title.FontAttributes = FontAttributes.Bold;
            chart.Title.TextColor = Styles.TextColor;

            CategoryAxis primaryAxis = new CategoryAxis();
            primaryAxis.LabelStyle.TextColor = Color.Transparent;
            chart.PrimaryAxis = primaryAxis;


            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Maximum = 10.5;
            secondaryAxis.Minimum = 0;
            chart.SecondaryAxis = secondaryAxis;


            SplineSeries series = new SplineSeries();
            series.StrokeWidth = 3;
            series.EnableTooltip = true;
            series.ItemsSource = source;
            series.Color = Styles.PrimaryColor;
            series.XBindingPath = "datGiorno";
            series.YBindingPath = "decValore";

            //Adding Series to the Chart Series Collection
            chart.Series.Add(series);

            //Set Chart Sizes
            chart.HeightRequest = App.ScreenHeight / 5;
            chart.WidthRequest = App.ScreenWidth / 1.3;

            //Add chart to layout
            layout.Children.Add(chart);

            //Add stonks label
            if (source.Count >= 2)
            {
                try
                {
                    layout.Children.Add(new Xamarin.Forms.Label { Text = stonks(source[source.Count - 2].decValore, source[source.Count - 1].decValore), VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, TextColor = Styles.TextColor, FontSize = 15 });
                }
                catch
                { //Fa niente
                }
            }

            //Display Popup
            var secondPopUp = new Helpers.PopOvers().defaultPopOver;
            secondPopUp.Content = layout;
            secondPopUp.PointerDirection = PointerDirection.Up;
            secondPopUp.PreferredPointerDirection = PointerDirection.Up;
            secondPopUp.Target = chartButton;
            secondPopUp.BackgroundColor = Color.White;
            secondPopUp.IsVisible = true;
        }


        void Close_Clicked(object sender, System.EventArgs e)
        {
            //Close Page
            Navigation.PopModalAsync();
        }


        string stonks(double? primoVoto, double? secondoVoto)
        {
            if (secondoVoto > primoVoto)
                return "Stonks";
            else if (secondoVoto == primoVoto)
                return "";
            else
                return "Not stonks";
        }
    }
}
