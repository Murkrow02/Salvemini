using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using SalveminiApp.RestApi;
#if __IOS__
using UIKit;
#endif
namespace SalveminiApp.iCringe
{
    public partial class Commenti : ContentPage
    {
        public RestApi.Models.CommentiReturn commenti = new RestApi.Models.CommentiReturn();
        int idPost;
        public static bool Pushed;

        public Commenti(int idPost_, string cachedQuestion = null)
        {
            InitializeComponent();

#if __IOS__
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && !iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 20, 0, 0);
            }
#endif
            chatEntry.WidthRequest = App.ScreenWidth - 70;

            idPost = idPost_;

            //Get from cache
            var cachedCommenti = CacheHelper.GetCache<RestApi.Models.CommentiReturn>("commenti" + idPost_);
            if (cachedCommenti != null)
            {
                commentsList.ItemsSource = cachedCommenti.Commenti;
                header.Text = cachedCommenti.Domanda;
            }

            //Get from push
            if (!string.IsNullOrEmpty(cachedQuestion))
                header.Text = cachedQuestion;

#if __IOS__
            //Handle keyboard animation
            UIKit.UIKeyboard.Notifications.ObserveWillShow((s, e) =>
            {
                var r = UIKit.UIKeyboard.FrameEndFromNotification(e.Notification);
                entryFrame.TranslateTo(0, -r.Height, (uint)(e.AnimationDuration * 1000));
                //  list.ScaleHeightTo(list.Height - r.Height, (uint)(e.AnimationDuration * 1000));
            });

            UIKit.UIKeyboard.Notifications.ObserveWillHide((s, e) =>
            {
                var r = UIKit.UIKeyboard.FrameBeginFromNotification(e.Notification);
                entryFrame.TranslateTo(0, 0, (uint)(e.AnimationDuration * 1000));
                // list.ScaleHeightTo(listHeight, (uint)(e.AnimationDuration * 1000));
            });
#endif
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

            //Refresh list
            commentsList.IsRefreshing = true;

            //Download posts
            commenti = await App.Cringe.GetCommenti(idPost);

            //Error
            if (commenti == null)
            {
                Costants.showToast("Si è verificato un errore, riprova più tardi o contattaci se il problema persiste");
                commentsList.IsRefreshing = false;
                return;
            }

            //Update list
            commentsList.ItemsSource = commenti.Commenti;
            header.Text = commenti.Domanda;
            commentsList.IsRefreshing = false;
        }

        public void comments_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Ios 13 bug
            try
            {

                if (!Pushed)
                    Navigation.PopModalAsync();
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
