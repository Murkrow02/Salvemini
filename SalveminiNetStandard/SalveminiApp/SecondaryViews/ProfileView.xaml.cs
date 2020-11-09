using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class ProfileView : ContentPage
    {
        public RestApi.Models.Utenti utente;

        public ProfileView(RestApi.Models.Utenti utente_)
        {
            InitializeComponent();

            //Load initial interface
            userInfo.User = utente_;
            utente = utente_;

            //Set formsheet
            DependencyService.Get<IPlatformSpecific>().SetFormSheet(this);

            DependencyService.Get<IPlatformSpecific>().SetSafeArea(this);
            if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
                mainLayout.Padding = new Thickness(25, 20, 25, 0);


            //compleannoLbl.Text = utente.Compleanno.ToString("dd MMMM");
            //comuneLbl.Text = utente.Residenza;
            if(utente.Stato > 0)
            {
                roleLayout.IsVisible = true;
                switch (utente.Stato)
                {
                    case 1:roleLbl.Text = "Studente";break;
                    case 2:roleLbl.Text = "Rappresentante d'istituto";break;
                    case 3:roleLbl.Text = "CEO della SalveminiApp"; break;
                }
            }
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            OrarioFoto.ClasseCorso = utente.classeCorso;
        }

        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

    }
}
