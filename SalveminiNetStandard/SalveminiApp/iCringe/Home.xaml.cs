using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Forms9Patch;
using System.Linq;
using Plugin.Iconize;
using System.Collections.ObjectModel;
using Com.OneSignal;

namespace SalveminiApp.iCringe
{
    public partial class Home : ContentPage
    {
        ObservableCollection<DomandeReturn> Posts = new ObservableCollection<DomandeReturn>();


        public Home()
        {
            InitializeComponent();

            //Get cache
            var cachedPosts = CacheHelper.GetCache<List<DomandeReturn>>("cringefeed");
            if (cachedPosts != null)
                postsList.ItemsSource = cachedPosts;


            postsList.HeightRequest = App.ScreenHeight;

            //Android custom color
            if (Device.RuntimePlatform == Device.Android)
            {
                postsList.RefreshControlColor = Styles.TextColor;
                loading.Color = Styles.TextColor;
            }

            //Messaging Centers
            MessagingCenter.Subscribe<App>(this, "RefreshPosts", (sender) =>
            {
                OnAppearing();
            });
        }

        int appearedTimes = 0;
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            appearedTimes++;

            //Detect firstTime and subscribe to notifications
            if (Preferences.Get("firstTimeCringe", true))
            {
                //    await Navigation.PushModalAsync(new WelcomePage());
                Preferences.Set("firstTimeCringe", false);
                OneSignal.Current.SendTag("Secrets", Preferences.Get("UserId", 0).ToString());
            }


            //Detect internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            //Refresh list
            postsList.IsRefreshing = true;

            //Check new notifications in BG
            if(Device.RuntimePlatform == Device.iOS)
            await Task.Run((Action)checkNotifications);

            //Download posts
            var posts_ = await App.Cringe.GetFeed(-1);

            //Error
            if (posts_ == null)
            {
                Costants.showToast("Si è verificato un errore, riprova più tardi o contattaci se il problema persiste");
                postsList.IsRefreshing = false;
                return;
            }

            //Update list
            Posts.Clear();
            Posts = posts_.ToObservableCollection();
            postsList.ItemsSource = Posts;
            postsList.IsRefreshing = false;
        }

        void Post_Selected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect Animation
            if (e.SelectedItem == null)
                return;

            postsList.SelectedItem = null;

            var selectedPost = e.SelectedItem as DomandeReturn;

            //Prevent error
            if (selectedPost == null)
                return;

            //Create push
            var modalPush = new Commenti(selectedPost.id, selectedPost.Domanda);

            Navigation.PushModalAsync(modalPush);

        }

        public void posts_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }

        public void newPost_Clicked(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new iCringe.NewPost());
        }

        public async void user_tapped(object sender, EventArgs e)
        {
            //Get domanda
            var domanda = ((sender as Xamarin.Forms.StackLayout).Children[2] as Xamarin.Forms.Button).CommandParameter as DomandeReturn;

            //Prevent error
            if (domanda == null)
                return;

            //Push to user page
            //Create push
            if (domanda.Utente == null)
                return;

            var modalPush = new SecondaryViews.ProfileView(domanda.Utente);

            await Navigation.PushModalAsync(modalPush);
        }

        public async void deletePost_Clicked(object sender, EventArgs e)
        {
            //Get domanda
            var domanda = (sender as IconButton).CommandParameter as DomandeReturn;

            //Conferma
            bool confirm = await this.GetParentPage().DisplayAlert("Sei sicuro?", "Vuoi eliminare questo post?", "Elimina", "Annulla");
            if (!confirm)
                return;

            //Elimina
            var successo = await App.Cringe.DeletePost(domanda.id);
            if (successo[0] == "Successo")
            {
                //Refresh list
                Posts.Remove(domanda);
            }
            Costants.showToast(successo[1]);
        }

        public async void checkNotifications()
        {
            try
            {
                var lastNotifica = Preferences.Get("lastNotifica", 0);

                var nuove = await App.Cringe.GetNewNotifiche(lastNotifica);

                //No new notifications
                if (nuove == null || nuove.Count < 3)
                    return;

                //Popover notifiche
                var notifichePopup = new Helpers.PopOvers().defaultPopOver;
                notifichePopup.PointerDirection = PointerDirection.Up;
                notifichePopup.PreferredPointerDirection = PointerDirection.Up;
                notifichePopup.Target = bell;
                notifichePopup.BackgroundColor = Styles.BGColor;

                //Crea contenuto
                var layout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 8 };
                var commentiCount = nuove.SingleOrDefault(x => x.Tipo == 2);
                var accettateCount = nuove.SingleOrDefault(x => x.Tipo == 1);
                var rifiutateCount = nuove.SingleOrDefault(x => x.Tipo == 0);

                //Add custom
                if (commentiCount.Count != "0")
                {
                    var stack = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 5 };
                    var text = new Xamarin.Forms.Label { TextColor = Styles.SecretsPrimary, FontSize = 16, Text = commentiCount.Count, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                    var icon = new IconLabel { TextColor = Styles.SecretsPrimary, FontSize = 18, Text = "fas-comment", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                    stack.Children.Add(text); stack.Children.Add(icon);
                    layout.Children.Add(stack);
                }
                if (accettateCount.Count != "0")
                {
                    var stack = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 5 };
                    var text = new Xamarin.Forms.Label { TextColor = Styles.SecretsPrimary, FontSize = 16, Text = accettateCount.Count, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                    var icon = new IconLabel { TextColor = Styles.SecretsPrimary, FontSize = 18, Text = "fas-check", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                    stack.Children.Add(text); stack.Children.Add(icon);
                    layout.Children.Add(stack);
                }
                if (rifiutateCount.Count != "0")
                {
                    var stack = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 5 };
                    var text = new Xamarin.Forms.Label { TextColor = Styles.SecretsPrimary, FontSize = 16, Text = rifiutateCount.Count, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                    var icon = new IconLabel { TextColor = Styles.SecretsPrimary, FontSize = 18, Text = "fas-times", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                    stack.Children.Add(text); stack.Children.Add(icon);
                    layout.Children.Add(stack);
                }

                notifichePopup.Content = layout;

                //Nothing added in popup
                if (layout.Children.Count() < 1)
                    return;

                Device.BeginInvokeOnMainThread(() =>
                {
                    notifichePopup.IsVisible = true;
                });
            }
            catch
            {
                return;
            }
        }

        public void notifiche_Clicked(object sender, EventArgs e)
        {
            //Create push
            var modalPush = new Notifiche();

            Navigation.PushModalAsync(modalPush);
        }

        public void AndroidFix()
        {
            if (appearedTimes == 0)
            {
                OnAppearing();
            }
        }

        //Infinite scroll
        private async void ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            //Get appeared item
            var appearedItem = e.Item as DomandeReturn;

            //Check if is last post
            if (Posts.LastOrDefault() != appearedItem)
                return;

            //Show loading
            loading.IsVisible = true;
            loading.IsRunning = true;

            //Get new posts
            var newPosts = await App.Cringe.GetFeed(appearedItem.id);

            //Error
            if (newPosts == null)
            {
                Costants.showToast("Si è verificato un errore durante il download dei post precedenti, riprova più tardi o contattaci se il problema persiste");
                loading.IsVisible = false;
                loading.IsRunning = false;
                return;
            }

            //No older
            if (newPosts.Count < 1)
            {
                Costants.showToast("Non ci sono post precedenti");
                loading.IsVisible = false;
                loading.IsRunning = false;
                return;
            }

            //Add new posts
            Posts.AddRange(newPosts);


        }
    }
}
