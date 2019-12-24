using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;
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
