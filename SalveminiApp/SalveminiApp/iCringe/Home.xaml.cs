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
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
namespace SalveminiApp.iCringe
{
    public partial class Home : ContentPage
    {
        List<DomandeReturn> Posts = new List<DomandeReturn>();


        public Home()
        {
            InitializeComponent();

            //Get cache
            var cachedPosts = CacheHelper.GetCache<List<DomandeReturn>>("cringefeed");
            if (cachedPosts != null)
                postsList.ItemsSource = cachedPosts;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Detect internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            //Refresh list
            postsList.IsRefreshing = true;

            //Check new notifications in BG
            await Task.Run((Action)checkNotifications);

            //Download posts
            Posts = await App.Cringe.GetFeed(-1);

            //Error
            if (Posts == null)
            {
                Costants.showToast("Si è verificato un errore, riprova più tardi o contattaci se il problema persiste");
                postsList.IsRefreshing = false;
                return;
            }

            //Update list
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

            //Modal figo
#if __IOS__
            modalPush.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
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

        public async void checkNotifications()
        {
            var lastNotifica = Preferences.Get("lastNotifica", 0);

            var nuove = await App.Cringe.GetNotifiche(true, 0);

            //No new notifications
            if (nuove == null || nuove.Count < 1)
                return;

            //Popover notifiche
            var notifichePopup = new Helpers.PopOvers().defaultPopOver;
            notifichePopup.PointerDirection = PointerDirection.Up;
            notifichePopup.PreferredPointerDirection = PointerDirection.Up;
            notifichePopup.Target = bell;
            notifichePopup.BackgroundColor = Styles.BGColor;

            //Crea contenuto
            var layout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 8};
            var commentiCount = nuove.Where(x => x.Tipo == 2).Count();
            var accettateCount = nuove.Where(x => x.Tipo == 1).Count();
            var rifiutateCount = nuove.Where(x => x.Tipo == 0).Count();

            //Add custom
            if(commentiCount > 0)
            {
                var stack = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 5};
                var text = new Xamarin.Forms.Label { TextColor = Styles.SecretsPrimary, FontSize = 16, Text = commentiCount.ToString(), FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                var icon = new IconLabel { TextColor = Styles.SecretsPrimary, FontSize = 18, Text = "fas-comment", FontAttributes = FontAttributes.Bold ,HorizontalTextAlignment = TextAlignment.Center};
                stack.Children.Add(text); stack.Children.Add(icon);
                layout.Children.Add(stack);
            }
            if (accettateCount > 0)
            {
                var stack = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 5 };
                var text = new Xamarin.Forms.Label { TextColor = Styles.SecretsPrimary, FontSize = 16, Text = accettateCount.ToString(), FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                var icon = new IconLabel { TextColor = Styles.SecretsPrimary, FontSize = 18, Text = "fas-check", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                stack.Children.Add(text); stack.Children.Add(icon);
                layout.Children.Add(stack);
            }
            if (rifiutateCount > 0)
            {
                var stack = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 5 };
                var text = new Xamarin.Forms.Label { TextColor = Styles.SecretsPrimary, FontSize = 16, Text = rifiutateCount.ToString(), FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                var icon = new IconLabel { TextColor = Styles.SecretsPrimary, FontSize = 18, Text = "fas-times", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
                stack.Children.Add(text); stack.Children.Add(icon);
                layout.Children.Add(stack);
            }

            notifichePopup.Content = layout;

            Device.BeginInvokeOnMainThread(() =>
            {
                notifichePopup.IsVisible = true;
            });
        }

        public void notifiche_Clicked(object sender, EventArgs e)
        {
            //Create push
            var modalPush = new Notifiche();

            //Modal figo
#if __IOS__
            modalPush.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
            Navigation.PushModalAsync(modalPush);
        }
    }
}
