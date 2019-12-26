using System;
using System.Collections.Generic;
using P42.Utils;
using Xamarin.Forms;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Syncfusion.SfChart.XForms;
using System.Linq;
using System.Diagnostics;
#if __IOS__
using UIKit;
#endif
namespace SalveminiApp
{
    public partial class ArgoPage : ContentPage
    {
        public List<RestApi.Models.Pentagono> Medie = new List<RestApi.Models.Pentagono>();

        int appearedTimes;
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (((this.Parent as Helpers.CustomNavigationPage).Parent as TabbedPage).CurrentPage != this.Parent as Helpers.CustomNavigationPage)
                return;

            //Show agenda only if orario downloaded
            if (Preferences.Get("OrarioSaved", false))
                agendaFrame.IsVisible = true;

            //Do Appearing only every 5 times or from pull to refresh or connection lost or first
            var lastDigit = appearedTimes % 10;
            if (lastDigit != 0 && lastDigit != 5 && appearedTimes != 1)
                return;

            //Check Internet
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                //Nessuna connessione
                Costants.showToast("connection");
                appearedTimes = 5; //Repeat this every time
                if (Medie.IsNullOrEmpty()) { chartLayout.IsVisible = false; }
                else
                {
                    showChart(); //Got from cache
                }
                return;
            }

            //Increment number of appeared times
            appearedTimes++;

            try
            {
                chartLoading.IsRunning = true; chartLoading.IsVisible = true;
                //Get values from pentagono api
                var dates = await App.Argo.GetPentagono();

                //Detect if call returned a message of error
                if (!string.IsNullOrEmpty(dates.Message))
                {
                    //Error occourred, notify the user
                    Costants.showToast(dates.Message);
                    return;
                }

                //Get new medie
                var newMedie = dates.Data as List<RestApi.Models.Pentagono>;


                //Fill medie list
                Medie = newMedie;
                showChart();

                //If previously hidden from no connection
                chartLayout.IsVisible = true;

                //Stop activityindicator
                chartLoading.IsRunning = false; chartLoading.IsVisible = false;
            }
            catch //Random error
            {
                Costants.showToast("Non è stato possibile aggiornare il tuo grafico");
            }

        }

        //Push from widget to argo page
        void Widget_Tapped(object sender, EventArgs e)
        {
#if __IOS__
            //UIApplication.SharedApplication.StatusBarHidden = true;
#endif
            Navigation.PushModalAsync((sender as ArgoWidget).Push);
        }

        //Fix chart label too long
        private void Chart_LabelCreated(object sender, ChartAxisLabelEventArgs e)
        {
            if (e.LabelContent.Length > 12)
            {
                e.LabelContent = e.LabelContent.Remove(12) + ".";
            }
        }

        //Shows the chart if abbastanza materie
        public void showChart()
        {
            if (Medie.Count >= 3)
            {
                chart.IsVisible = true;
                chart.Opacity = 1;
                noSubjectsLayout.IsVisible = false;
                radarChart.ItemsSource = Medie;
            }
            else
            {
                //Non abbastanza :(
                chart.IsVisible = false;
                noSubjectsLayout.IsVisible = true;
            }
        }

        public ArgoPage()
        {
            InitializeComponent();

            //Hide Navigation Bar
            NavigationPage.SetHasNavigationBar(this, false);

            //Set Right Padding on Notch devices
#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(20, 18, 20, 30);
            }
            //secondRowWidgets.Margin = new Thickness ( 0, 0, 0, 5 );
#endif

            //Show agenda fix
            MessagingCenter.Subscribe<App>(this, "showAgenda", (sender) =>
            {
                agendaFrame.IsVisible = true;
            });
        }

        public async void agenda_Clicked(object sender, EventArgs e)
        {
            var view = sender as Xamarin.Forms.PancakeView.PancakeView;
            await view.TranslateTo(App.ScreenWidth, 0, 700);
            await Navigation.PushModalAsync(new ArgoPages.Agenda());
            await view.TranslateTo(0, 0, 0);
        }

        public void initializeInterface()
        {
            //Cache Grafico
            var cachedMedie = CacheHelper.GetCache<List<RestApi.Models.Pentagono>>("Medie");
            if (cachedMedie != null)
            {
                chart.IsVisible = true;
                Medie = cachedMedie;
                radarChart.ItemsSource = Medie;
            }

            //Add widgets
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += Widget_Tapped;
            var assenze = new ArgoWidget { Title = "Assenze", Icon = "Assenze.svg", StartColor = "FF7272", EndColor = "FACA6F", Push = new ArgoPages.Assenze() };
            assenze.GestureRecognizers.Add(gestureRecognizer);
            var voti = new ArgoWidget { Title = "Voti", Icon = "Voti.svg", StartColor = "A940F5", EndColor = "6E9BFF", Push = new ArgoPages.Voti() };
            voti.GestureRecognizers.Add(gestureRecognizer);
            var promemoria = new ArgoWidget { Title = "Promemoria", Icon = "Promemoria.svg", StartColor = "F86095", EndColor = "FF6073", Push = new ArgoPages.Promemoria() };
            promemoria.GestureRecognizers.Add(gestureRecognizer);
            var bacheca = new ArgoWidget { Title = "Bacheca", Icon = "Bacheca.svg", StartColor = "03F829", EndColor = "20B4C7", Push = new ArgoPages.Bacheca() };
            bacheca.GestureRecognizers.Add(gestureRecognizer);
            var scrutinio = new ArgoWidget { Title = "Scrutinio", Icon = "VotiScru.svg", StartColor = "A940F5", EndColor = "6E9BFF", Push = new ArgoPages.VotiScrutinio() };
            scrutinio.GestureRecognizers.Add(gestureRecognizer);
            var compiti = new ArgoWidget { Title = "Compiti", Icon = "Compiti.svg", StartColor = "03F829", EndColor = "20B4C7", Push = new ArgoPages.CompitiArgomenti("compiti") };
            compiti.GestureRecognizers.Add(gestureRecognizer);
            var argomenti = new ArgoWidget { Title = "Argomenti", Icon = "Argomenti.svg", StartColor = "FF7272", EndColor = "FACA6F", Push = new ArgoPages.CompitiArgomenti("argomenti") };
            argomenti.GestureRecognizers.Add(gestureRecognizer);
            var note = new ArgoWidget { Title = "Note", Icon = "NoteDisciplinari.svg", StartColor = "F86095", EndColor = "FF6073", Push = new ArgoPages.Note() };
            note.GestureRecognizers.Add(gestureRecognizer);
            firstRowWidgets.Children.Clear();
            firstRowWidgets.Children.Add(new ContentView { WidthRequest = -20 });
            firstRowWidgets.Children.AddRange(new List<ArgoWidget> { assenze, voti, promemoria, bacheca });
            firstRowWidgets.Children.Add(new ContentView { WidthRequest = 0 });
            secondRowWidgets.Children.Clear();
            secondRowWidgets.Children.Add(new ContentView { WidthRequest = -20 });
            secondRowWidgets.Children.AddRange(new List<ArgoWidget> { scrutinio, compiti, argomenti, note });
            secondRowWidgets.Children.Add(new ContentView { WidthRequest = 0 });
           
        }
    }
}
