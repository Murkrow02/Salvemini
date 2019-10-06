using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class VipMenu : ContentPage
    {
        public VipMenu()
        {
            InitializeComponent();
        }


        void creaAvviso_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreaAvviso());
        }

        void statistiche_Tapped(object sender, System.EventArgs e)
        {
        }

        void utenti_Tapped(object sender, System.EventArgs e)
        {
        }
    }
}
