using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using SalveminiApp.RestApi;
using System.Diagnostics;
using System.Threading.Tasks;
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

            if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
                mainLayout.Padding = new Thickness(0, 20, 0, 0);

            chatEntry.WidthRequest = App.ScreenWidth - 70;

            //Set formsheet
            DependencyService.Get<IPlatformSpecific>().SetFormSheet(this);

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

            //Keyboard overlap on ios
            DependencyService.Get<IPlatformSpecific>().AnimateKeyboard(entryFrame);

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
            this.ForceLayout();
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
                Costants.showToast("Si è verificato un errore, il post potrebbe essere stato eliminato, riprova più tardi o contattaci se il problema persiste");
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
            var commento = new RestApi.Models.Commenti { Anonimo = false, Commento = chatEntry.Text, idPost = idPost };

            //Post commento
            var response = await App.Cringe.PostCommento(commento);

            if (response[0] == "Successo")
            {
                //Empty bar
                chatEntry.Text = "";

                //Refresh if success
                fromNewComment = true;
                OnAppearing();

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
