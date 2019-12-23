using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace SalveminiApp.iCringe
{
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var posts = await App.Cringe.GetFeed(-1);
            postsList.ItemsSource = posts;
        }


        public void posts_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }

        public void newPost_Clicked(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new iCringe.NewPost());
        }
    }
}
