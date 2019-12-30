using System;
using System.Collections.Generic;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using Com.OneSignal;
#if __IOS__
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
namespace SalveminiApp.iCringe
{
    public partial class Notifiche : ContentPage
    {
        public static bool Pushed;
        public List<RestApi.Models.Notifiche> notifiche = new List<RestApi.Models.Notifiche>();

        public Notifiche()
        {
            InitializeComponent();

#if __IOS__
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && !iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 20, 0, 0);
            }
#endif

            //Get from cache
            var cachedNotifiche = CacheHelper.GetCache<List<RestApi.Models.Notifiche>>("cringenotifiche");
            if (cachedNotifiche != null)
                notificheList.ItemsSource = cachedNotifiche;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Pushed = false;

            //Detect internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            //Switch toggled?
            notificheSwitch.IsToggled = Preferences.Get("iCringePush", false);

            //Refresh list
            notificheList.IsRefreshing = true;

            //Download posts
            notifiche = await App.Cringe.GetNotifiche(false);

            //Error
            if (notifiche == null)
            {
                Costants.showToast("Si è verificato un errore, riprova più tardi o contattaci se il problema persiste");
                notificheList.IsRefreshing = false;
                return;
            }

            //Save last
            if (notifiche.Count > 0)
                Preferences.Set("lastNotifica", notifiche.First().id);

            //Update list
            notificheList.ItemsSource = notifiche;
            notificheList.IsRefreshing = false;
        }

        public void switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (notificheSwitch.IsToggled) //Toggled
            {
                Preferences.Set("iCringePush", true);
                OneSignal.Current.SendTag("Secrets", Preferences.Get("UserId", 0).ToString());
            }
            else //Not toggled
            {
                Preferences.Set("iCringePush", false);
                OneSignal.Current.DeleteTag("Secrets");
            }
        }

        public void Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }

        public void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void item_Selected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect Animation
            if (e.SelectedItem == null)
                return;

            notificheList.SelectedItem = null;

            var selectedPost = e.SelectedItem as RestApi.Models.Notifiche;

            //Prevent error
            if (selectedPost == null || selectedPost.Tipo == 0)
                return;

            //Create push
            var modalPush = new Commenti(selectedPost.idPost);

            //Modal figo
#if __IOS__
            modalPush.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
            Commenti.fromNotifiche = true;
            Navigation.PushModalAsync(modalPush);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Ios 13 bug
            try
            {

                if (!Pushed)
                {
                    Navigation.PopModalAsync();
                    Navigation.PopModalAsync();
                }
                else
                    Pushed = false;
            }
            catch
            {
                //fa nient
            }

        }
    }
}
