using System;
using System.Collections.Generic;
using P42.Utils;
using Xamarin.Forms;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Syncfusion.SfChart.XForms;

namespace SalveminiApp
{
    public partial class ArgoPage : ContentPage
    {
        public List<RestApi.Models.Pentagono> Medie = new List<RestApi.Models.Pentagono>();

        public ArgoPage()
        {
            InitializeComponent();

            //Hide Navigation Bar
            NavigationPage.SetHasNavigationBar(this, false);

            //Set Right Padding on Notch devices
#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 40);
            }
#endif

            //Cache Grafico
            if (Barrel.Current.Exists("Medie"))
            {
                Medie = Barrel.Current.Get<List<RestApi.Models.Pentagono>>("Medie");
                radarChart.ItemsSource = Medie;
            }

            //Set Sizes
            chart.WidthRequest = App.ScreenWidth;
            chart.HeightRequest = App.ScreenHeight / 3;

            //Add widgets
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += Widget_Tapped;
            var assenze = new ArgoWidget { Title = "Assenze", Icon = "Assenze.svg", StartColor = "FF7272", EndColor = "FACA6F", Push = new ArgoPages.Assenze() };
            assenze.GestureRecognizers.Add(gestureRecognizer);
            var voti = new ArgoWidget { Title = "Voti", Icon = "Voti.svg", StartColor = "A940F5", EndColor = "6E9BFF", Push = new ArgoPages.Voti() };
            voti.GestureRecognizers.Add(gestureRecognizer);
            var promemoria = new ArgoWidget { Title = "Promemoria", Icon = "Promemoria.svg", StartColor = "F86095", EndColor = "FF6073", Push = new ArgoPages.Promemoria() };
            promemoria.GestureRecognizers.Add(gestureRecognizer);
            var bacheca = new ArgoWidget { Title = "Bacheca", Icon = "Promemoria.svg", StartColor = "F86095", EndColor = "FF6073", Push = new ArgoPages.Bacheca() };
            bacheca.GestureRecognizers.Add(gestureRecognizer);
            var scrutinio = new ArgoWidget { Title = "Scrutinio", Icon = "VotiScru.svg", StartColor = "A940F5", EndColor = "6E9BFF", Push = new ArgoPages.VotiScrutinio() };
            scrutinio.GestureRecognizers.Add(gestureRecognizer);
            var compiti = new ArgoWidget { Title = "Compiti", Icon = "Compiti.svg", StartColor = "03F829", EndColor = "20B4C7", Push = new ArgoPages.CompitiArgomenti("compiti") };
            compiti.GestureRecognizers.Add(gestureRecognizer);
            var argomenti = new ArgoWidget { Title = "Argomenti", Icon = "Argomenti.svg", StartColor = "FF7272", EndColor = "FACA6F", Push = new ArgoPages.CompitiArgomenti("argomenti") };
            argomenti.GestureRecognizers.Add(gestureRecognizer);
            var note = new ArgoWidget { Title = "Note", Icon = "Assenze.svg", StartColor = "FF7272", EndColor = "FACA6F", Push = new ArgoPages.Note() };
            note.GestureRecognizers.Add(gestureRecognizer);
            firstRowWidgets.Children.Clear();
            firstRowWidgets.Children.Add(new ContentView { WidthRequest = -30 });
            firstRowWidgets.Children.AddRange(new List<ArgoWidget> { assenze, voti, promemoria, bacheca });
            firstRowWidgets.Children.Add(new ContentView { WidthRequest = 0 });
            secondRowWidgets.Children.Clear();
            secondRowWidgets.Children.Add(new ContentView { WidthRequest = -30 });
            secondRowWidgets.Children.AddRange(new List<ArgoWidget> { scrutinio, compiti, argomenti, note });
            secondRowWidgets.Children.Add(new ContentView { WidthRequest = 0 });
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var dates = await App.Argo.GetPentagono();

                if (!string.IsNullOrEmpty(dates.Message))
                {

                }
                else
                {
                    Medie = dates.Data as List<RestApi.Models.Pentagono>;
                   // foreach(var a in Medie) { a.Materia = "Dmax"; }
                }

                if (Medie.Count >= 3)
                {
                    chart.IsVisible = true;
                    noSubjectsLayout.IsVisible = false;
                    radarChart.ItemsSource = Medie;
                }
                else
                {
                    chart.IsVisible = false;
                    noSubjectsLayout.IsVisible = true;
                }
            }
        }

        void Widget_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync((sender as ArgoWidget).Push);
        }

        private void Chart_LabelCreated(object sender, ChartAxisLabelEventArgs e)
        {
            if (e.LabelContent.Length > 8 )
            {
                e.LabelContent = e.LabelContent.Remove(8);
            }
        }
    }
}
