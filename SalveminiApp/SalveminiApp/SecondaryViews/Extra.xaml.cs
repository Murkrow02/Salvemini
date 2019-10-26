using System;
using System.Collections.Generic;

using Xamarin.Forms;

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
    }
}
