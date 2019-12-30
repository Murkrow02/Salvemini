using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using SalveminiApp.RestApi;
using MarcTron.Plugin;
using System.Diagnostics;
using System.Threading.Tasks;
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
        public static bool fromNotifiche;

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

            MessagingCenter.Subscribe<App, RestApi.Models.Commenti>(this, "removeCommento", (sender, commento) =>
             {
                 try
                 {
                     commenti.Commenti.Remove(commento);
                 }
                 catch { }
             });
        }

        bool fromNewComment;
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

            //load interstitial
            if (!fromNewComment)
            {
                CrossMTAdmob.Current.LoadInterstitial(AdsHelper.InterstitialId());
                CrossMTAdmob.Current.OnInterstitialLoaded += Current_OnInterstitialLoaded;
                fromNewComment = false;
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

        private void Current_OnInterstitialLoaded(object sender, EventArgs e)
        {
            Debug.WriteLine("Loaded ad");
        }

        public void comments_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }

        public async void postCommento_Clicked(object sender, EventArgs e)
        {
            //Detect internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            //Create commento
            var commento = new RestApi.Models.Commenti { Anonimo = anonimo.IsToggled, Commento = chatEntry.Text, idPost = idPost };

            //Post commento
            var response = await App.Cringe.PostCommento(commento);

            if (response[0] == "Successo")
            {
                //Empty bar
                chatEntry.Text = "";

                //Refresh if success
                fromNewComment = true;
                OnAppearing();

                //Show interstitial
                if (CrossMTAdmob.Current.IsInterstitialLoaded())
                {
                    Pushed = true;
                    CrossMTAdmob.Current.ShowInterstitial(); return;
                }
                else
                {
                    CrossMTAdmob.Current.LoadInterstitial(AdsHelper.InterstitialId());
                }

                //Show response
                Costants.showToast(response[1]);
            }
            else
            {
                //Error
                await DisplayAlert("Errore", response[1], "Ok");
            }
        }

        public void checkText(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(chatEntry.Text) || string.IsNullOrWhiteSpace(chatEntry.Text))
            {
                sendButton.IsEnabled = false;
                sendButton.Opacity = 0.8;
            }
            else
            {
                sendButton.IsEnabled = true;
                sendButton.Opacity = 1;
            }
        }

        public void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Ios 13 bug
            try
            {
                if (fromNotifiche)
                {
                    Pushed = false;
                    return;
                }
                else
                    fromNotifiche = false;
                if (!Pushed)
                    Navigation.PopModalAsync();
                else
                {
                    fromNotifiche = false;
                    Pushed = false;
                }
            }
            catch
            {
                //fa nient
            }

        }
    }
}
