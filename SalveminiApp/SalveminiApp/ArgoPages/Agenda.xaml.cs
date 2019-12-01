using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;

namespace SalveminiApp.ArgoPages
{
    public partial class Agenda : ContentPage
    {
        public Agenda()
        {
            InitializeComponent();

#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 30, 0, 0);
            }
#endif

            days.DataSource = Costants.getDays();
        }


        private async void picker_Unfocused(object sender, FocusEventArgs e)
        {
            compitiList.ItemsSource = await App.Agenda.GetCompitiAgenda(days.SelectedIndex + 1);
        }
        }
}
