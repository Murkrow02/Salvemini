using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class AppInfo : ContentPage
    {
        public AppInfo()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Download current
            var currentInfo = await App.Analytics.GetAppInfo();
            if(currentInfo!= null)
            {
                version.Text = currentInfo.AppVersion.ToString();
                orario.Text = currentInfo.OrarioVersion.ToString();
                version.IsEnabled = true;
               // orario.IsEnabled = true;
            }
          
        }

        public async void updateVersions(object sender, EventArgs e)
        {
            var appinfo = new RestApi.Models.AppInfo();

            try
            {
                //Get values
                appinfo.AppVersion = Convert.ToDecimal(version.Text);
                appinfo.OrarioVersion = Convert.ToInt32(orario.Text);

                //Upload new values
                var success = await App.Analytics.PostAppInfo(appinfo);
                if (success)
                    Costants.showToast("Successo");
                else
                    Costants.showToast("Errore");
            }
            catch
            {
                Costants.showToast("Errore (Valori non validi)");
            }

        }
    }
}
