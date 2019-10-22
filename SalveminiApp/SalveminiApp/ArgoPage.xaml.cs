using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp
{
    public partial class ArgoPage : ContentPage
    {
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            widgetCollection.ItemsSource = new List<ArgoWidget> { new ArgoWidget { Title = "Assenze", Icon = "Assenze.svg", StartColor = "FF7272", EndColor = "FACA6F", Push = new ArgoPages.Assenze() }, new ArgoWidget { Title = "Voti", Icon = "Voti.svg", StartColor = "A940F5", EndColor = "6E9BFF", Push = new ArgoPages.Voti() }, new ArgoWidget { Title = "Promemoria", Icon = "Promemoria.svg", StartColor = "F86095", EndColor = "FF6073", Push = new ArgoPages.Promemoria() }, new ArgoWidget { Title = "Scrutinio", Icon = "VotiScru.svg", StartColor = "A940F5", EndColor = "6E9BFF", Push = new ArgoPages.VotiScrutinio() } };
            
        }

        void Widget_Tapped(object sender, EventArgs e)
        {
            //Get push page from model
            var widget = sender as ArgoWidget;

            //Push to selected page
            if (widget.Push != null)
                Navigation.PushModalAsync(widget.Push);
        }


        //void Assenze_Tapped(object sender, System.EventArgs e)
        //{
        //    Navigation.PushModalAsync(new ArgoPages.Assenze());
        //}

        //void Promemoria_Tapped(object sender, System.EventArgs e)
        //{
        //    Navigation.PushModalAsync(new ArgoPages.Promemoria());
        //}

        //void Voti_Tapped(object sender, System.EventArgs e)
        //{
        //    Navigation.PushModalAsync(new ArgoPages.Voti());
        //}

        //void VotiScru_Tapped(object sender, System.EventArgs e)
        //{
        //    Navigation.PushModalAsync(new ArgoPages.VotiScrutinio());
        //}

        //void Compiti_Tapped(object sender, System.EventArgs e)
        //{
        //    Navigation.PushModalAsync(new ArgoPages.CompitiArgomenti("compiti"));
        //}

        //void Argomenti_Tapped(object sender, System.EventArgs e)
        //{
        //    Navigation.PushModalAsync(new ArgoPages.CompitiArgomenti("argomenti"));
        //}
    }
}
