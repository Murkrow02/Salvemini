using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;

namespace SalveminiApp.SecondaryViews
{
    public partial class Extra : ContentPage
    {


        public Extra()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            mainLayout.Children.RemoveAt(1);
            //Ios 13 bug
            try
            {
                Navigation.PopModalAsync();
            }
            catch
            {
                //fa nient
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            mainLayout.Children.Insert(1, new Helpers.CountDown());
        }

        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
