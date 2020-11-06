using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;
using MonkeyCache.SQLite;
using Rg.Plugins.Popup.Extensions;
using Plugin.Toasts;
namespace SalveminiApp.ArgoPages
{
    public partial class Note : ContentPage
    {
        public List<RestApi.Models.Note> Notes = new List<RestApi.Models.Note>();

        public Note()
        {
            InitializeComponent();

            if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
                fullLayout.Padding = new Thickness(20, 35, 20, 25);

            //Set Sizes
            shadowImage.WidthRequest = App.ScreenWidth * 1.5;
            shadowImage.HeightRequest = App.ScreenWidth * 1.5;
            noteList.HeightRequest = App.ScreenHeight / 1.8;
            //buttonFrame.WidthRequest = App.ScreenWidth / 6;
            //buttonFrame.HeightRequest = App.ScreenWidth / 6;
            //buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;

            //Load Cache
            var cachedNote = CacheHelper.GetCache<List<RestApi.Models.Note>>("Note");
            if (cachedNote != null)
            {
                Notes = cachedNote;
                noteList.ItemsSource = Notes;
                emptyLayout.IsVisible = Notes.Count <= 0;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Show loading
            noteList.IsRefreshing = true;

            //Check connection status
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    //Download note from api
                    var response = await App.Argo.GetNote();

                    //Detect if call returned a message of error
                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        //Error occourred, notify the user
                       // Costants.showToast(response.Message);
                        //Stop loading list
                        noteList.IsRefreshing = false;
                        return;
                    }

                    //Deserialize object
                    Notes = response.Data as List<RestApi.Models.Note>;

                    //Fill List
                    noteList.ItemsSource = Notes;
                    emptyLayout.IsVisible = Notes.Count <= 0;

                }
                catch //Random error
                {
                    Costants.showToast("Non è stato possibile aggiornare le note");
                }
            }
            else //No connection
            {
                Costants.showToast("connection");
            }

            //Stop loading list
            noteList.IsRefreshing = false;
        }
        public void updateList(object sender, EventArgs e)
        {
            OnAppearing();
        }
        void Close_Clicked(object sender, System.EventArgs e)
        {
            //Close Page
            Navigation.PopModalAsync();
        }
    }
}
