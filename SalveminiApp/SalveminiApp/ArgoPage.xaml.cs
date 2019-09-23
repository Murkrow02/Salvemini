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

            //Set Sizes
            assenzeIcon.WidthRequest = App.ScreenWidth / 3.8;
            compitiIcon.WidthRequest = App.ScreenWidth / 3.8;
            promemoriaIcon.WidthRequest = App.ScreenWidth / 3.8;
            votiIcon.WidthRequest = App.ScreenWidth / 3.8;
            argoIcon.WidthRequest = App.ScreenWidth / 3.8;
            votiScruIcon.WidthRequest = App.ScreenWidth / 3.8;


        }

        void Assenze_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.Assenze());
        }

        void Promemoria_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.Promemoria());
        }

        void Voti_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.Voti());
        }

        void VotiScru_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.VotiScrutinio());
        }

        void Compiti_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.CompitiArgomenti("compiti"));
        }

        void Argomenti_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ArgoPages.CompitiArgomenti("argomenti"));
        }
    }
}
