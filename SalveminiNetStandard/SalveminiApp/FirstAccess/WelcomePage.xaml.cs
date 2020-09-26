using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SalveminiApp.FirstAccess
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();

            //Initialize Interface
            appIcon.WidthRequest = App.ScreenWidth / 4.5;

            //Set Safearea
            DependencyService.Get<IPlatformSpecific>().SetSafeArea(this);


            //Hide navigation bar
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Animate Appearing
            titleLayout.TranslationY = -200;
            await Task.WhenAll(titleLayout.TranslateTo(0, 0, 1000, Easing.CubicOut), titleLayout.FadeTo(1, 1000));
            await touchToContinueLabel.FadeTo(0.8, 800);
        }

       async void Layout_Tapped(object sender, System.EventArgs e)
        {
            await this.FadeTo(0,300, Easing.CubicIn);
           await Navigation.PushAsync(new Login(),false);
        }
    }
}
