using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class Profile : ContentPage
    {
        public Profile()
        {
            InitializeComponent();

            //Fill lists
            persLayout.Children.Add( new Helpers.PushCell { Title="Notifiche",Separator="si"});
        }
    }
}
