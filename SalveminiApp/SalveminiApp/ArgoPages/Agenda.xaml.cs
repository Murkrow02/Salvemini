using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.ArgoPages
{
    public partial class Agenda : ContentPage
    {
        public Agenda()
        {
            InitializeComponent();

            days.ItemsSource = Costants.getDays();
        }


        private async void picker_Unfocused(object sender, FocusEventArgs e)
        {
            compitiList.ItemsSource = await App.Agenda.GetCompitiAgenda(days.SelectedIndex + 1);
        }
        }
}
