using System;
using System.Collections.Generic;
using P42.Utils;
using Xamarin.Forms;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Syncfusion.SfChart.XForms;
using System.Linq;
#if __IOS__
using UIKit;
#endif
namespace SalveminiApp
{
    public partial class ArgoPage : ContentPage
    {
        public List<RestApi.Models.Pentagono> Medie = new List<RestApi.Models.Pentagono>();

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Show agenda only if orario downloaded
            if (Preferences.Get("OrarioSaved", false))
                agendaFrame.IsVisible = true;

            //Check internet status
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
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

                }
                catch //Random error
                {
                    Costants.showToast("Non è stato possibile aggiornare il tuo grafico");
                }
            }
            else //No internet
            {
                //Costants.showToast("connection");
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
                chartA.IsVisible = true;
                chartI.IsVisible = true;
                chartI.Opacity = 1;
                chartA.Opacity = 1;
                noSubjectsLayout.IsVisible = false;
                radarChartA.ItemsSource = Medie;
                radarChartI.ItemsSource = Medie;

            }
            else
            {
                //Non abbastanza :(
                chartA.IsVisible = false;
                chartI.IsVisible = false;
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

            //Cache Grafico
            var cachedMedie = CacheHelper.GetCache<List<RestApi.Models.Pentagono>>("Medie");
            if (cachedMedie != null)
            {
                chartA.IsVisible = true;
                chartI.IsVisible = true;
                Medie = cachedMedie;
                radarChartA.ItemsSource = Medie;
                radarChartI.ItemsSource = Medie;

            }

            //Set Sizes
            //chart.WidthRequest = App.ScreenWidth;
            //chart.HeightRequest = App.ScreenHeight / 3;

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

        public async void agenda_Clicked(object sender, EventArgs e)
        {
            var view = sender as Xamarin.Forms.PancakeView.PancakeView;
            await view.TranslateTo(App.ScreenWidth, 0, 700);
            await Navigation.PushModalAsync(new ArgoPages.Agenda());
            await view.TranslateTo(0, 0, 0);
        }
    }
}
