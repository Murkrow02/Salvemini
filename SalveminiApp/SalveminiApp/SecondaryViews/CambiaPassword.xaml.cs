using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class CambiaPassword : ContentPage
    {
        public CambiaPassword()
        {
            InitializeComponent();
        }

        void oldPwdEye_Tapped(object sender, EventArgs e)
        {
            oldPwdEye.Text = oldPwdEye.Text == "fas-eye-slash" || oldPwdEye.Text == "eye-slash" ? "eye" : "eye-slash";
            oldPwdEntry.IsPassword = oldPwdEye.Text == "fas-eye-slash" || oldPwdEye.Text == "eye-slash";
        }
        void newPwdEye_Tapped(object sender, EventArgs e)
        {
            newPwdEye.Text = newPwdEye.Text == "fas-eye-slash" || newPwdEye.Text == "eye-slash" ? "eye" : "eye-slash";
            newPwdEntry.IsPassword = newPwdEye.Text == "fas-eye-slash" || newPwdEye.Text == "eye-slash";
        }
        void confirmPwdEye_Tapped(object sender, EventArgs e)
        {
            confirmPwdEye.Text = confirmPwdEye.Text == "fas-eye-slash" || confirmPwdEye.Text == "eye-slash" ? "eye" : "eye-slash";
            confirmPwdEntry.IsPassword = confirmPwdEye.Text == "fas-eye-slash" || confirmPwdEye.Text == "eye-slash";
        }


        void changePwd_Clicked(object sender, EventArgs e)
        {
            //todo
        }



    }
}
