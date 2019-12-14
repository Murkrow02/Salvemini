using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Plugin.Iconize;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class CambiaPassword : ContentPage
    {
        public CambiaPassword()
        {
            InitializeComponent();

            argoLogo.WidthRequest = App.ScreenWidth / 3;
        }

        void EntryEye_Tapped(object sender, EventArgs e)
        {
            //Get the Eye iconlabel
            var eye = sender as IconLabel;

            //Get the selected entry
            var entry = (eye.Parent as Grid).Children[5] as Entry;

            //Update IsPassword statement
            eye.Text = eye.Text == "fas-eye-slash" || eye.Text == "eye-slash" ? "eye" : "eye-slash";
            entry.IsPassword = eye.Text == "fas-eye-slash" || eye.Text == "eye-slash";
        }

        async void changePwd_Clicked(object sender, EventArgs e)
        {
            //Check old password exists
            if (string.IsNullOrWhiteSpace(oldPwdEntry.Text))
            {
                oldPwdEntry.PlaceholderColor = Color.Red;
                return;
            }
            oldPwdEntry.PlaceholderColor = Color.Default;

            //Check new password exists
            if (string.IsNullOrWhiteSpace(newPwdEntry.Text))
            {
                newPwdEntry.PlaceholderColor = Color.Red;
                return;
            }
            newPwdEntry.PlaceholderColor = Color.Default;

            //Check new password is at least 8 characters and contains a special character
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (newPwdEntry.Text.Length < 8 || regexItem.IsMatch(newPwdEntry.Text))
            {
                Costants.showToast("La password deve essere lunga almeno 8 caratteri e contenere almeno un carattere speciale");
                return;
            }

            //Check passwords are the same
            if (newPwdEntry.Text != confirmPwdEntry.Text)
            {
                Costants.showToast("Le password non combaciano");
                return;
            }

            //Check user has internet access
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Confirm the logout
                bool confirm = await DisplayAlert("Confermi?", "Una volta cambiata la password dovrai rieffettuare l'accesso", "Conferma", "Annulla");
                if (!confirm)
                    return;

                //Attempt call
                var data = await App.Utenti.ChangePwd(new RestApi.Models.changeBlock { nuovaPassword = newPwdEntry.Text, vecchiaPassword = oldPwdEntry.Text });
                if (data.Message != null)
                {
                    //Error occurred
                    Costants.showToast(data.Message);
                }
                else
                {
                    //Success
                    if ((bool)data.Data)
                    {
                        await DisplayAlert("Successo", "La password è stata modificata con successo", "Ok");

                        //Logout
                        Costants.Logout();
                    }
                }
            }
        }



    }
}
