using System;
using System.Collections.Generic;
using Xamarin.Forms;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif
using Xamarin.Essentials;
using Plugin.Toasts;
using DLToolkit.Forms.Controls;
using System.Linq;
using Forms9Patch;

namespace SalveminiApp.SecondaryViews
{
    public partial class Avvisi : ContentPage
    {
        public List<RestApi.Models.Avvisi> Avvisis { get; private set; }

        public Avvisi()
        {
            InitializeComponent();

            //Set Safearea
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
#endif

            //Get avvisi from cache
            var cachedAvvisi = CacheHelper.GetCache<List<RestApi.Models.Avvisi>>("Avvisi");
            if (cachedAvvisi != null)
            {
                Avvisis = cachedAvvisi;
                avvisiCarousel.ItemsSource = Avvisis;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Check if is coming from imageviewer
            if (!canRefresh)
            {
                canRefresh = true;
                return;
            }

            MainPage.isSelectingImage = false;

            //Check internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            try
            {
                //Download new avvisi from api
                var newAvvisi = await App.Avvisi.GetAvvisi();

                //Check if all went good
                if (newAvvisi != null && newAvvisi.Count > 0)
                {
                    Avvisis = newAvvisi;
                    avvisiCarousel.ItemsSource = Avvisis;
                    Preferences.Set("LastAvviso", Avvisis[0].id);
                    MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "RemoveBadge", "Avvisi");
                    return;
                }

                //Cannot download avvisi
                Costants.showToast("Non è stato possibile scaricare i nuovi avvisi, controlla la tua connessione e riprova");
            }
            catch //Unexpected error
            {
                Costants.showToast("Si è verificato un errore, riprova più tardi");
            }
        }


        //Open full image list
        private void ImageList_ItemSelected(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            MainPage.isSelectingImage = true;
            var a = sender as FlowListView;
            var b = a.FlowItemsSource as List<string>;
            canRefresh = false;
            Navigation.PushModalAsync(new Helpers.ImageViewer(b));
        }

        //Show more text
        void ShowMore_Clicked(object sender, EventArgs e)
        {
            //Get the text
            var text = (((sender as Xamarin.Forms.Button).Parent as Xamarin.Forms.StackLayout).Children[2] as Xamarin.Forms.Label).Text;

            //Get the full description
            var fullDesc = Avvisis.FirstOrDefault(x => x.Descrizione.Length > 100 && x.Descrizione.Remove(100) + "..." == text).Descrizione;

            var bubblePopup = new Helpers.PopOvers().defaultPopOver;
            bubblePopup.Content = new Xamarin.Forms.ScrollView { VerticalOptions = LayoutOptions.FillAndExpand, Content = new Xamarin.Forms.Label { Text = fullDesc, VerticalOptions = LayoutOptions.FillAndExpand } };
            bubblePopup.IsVisible = true;
            bubblePopup.PointerDirection = PointerDirection.Down;
            bubblePopup.PreferredPointerDirection = PointerDirection.Down;
            bubblePopup.Target = sender as Xamarin.Forms.Button;
            bubblePopup.HeightRequest = App.ScreenHeight / 1.5;
            bubblePopup.IsVisible = true;
        }


        //Open menu avvisi
        void Menu_Clicked(object sender, EventArgs e)
        {
            var bubblePopup = new Helpers.PopOvers().defaultPopOver;
            bubblePopup.Content = new Xamarin.Forms.ListView { WidthRequest = App.ScreenWidth / 4, BackgroundColor = Color.Transparent, ItemsSource = Avvisis.Select(x => x.Titolo).ToList(), ItemTemplate = new DataTemplate(() => { var lbl = new Xamarin.Forms.Label { VerticalOptions = LayoutOptions.Center, TextColor = Color.White, HorizontalOptions = LayoutOptions.Center }; lbl.SetBinding(Xamarin.Forms.Label.TextProperty, "."); return new ViewCell { View = lbl }; }) };
            bubblePopup.WidthRequest = App.ScreenWidth / 9;
            bubblePopup.HeightRequest = App.ScreenWidth / 3;
            bubblePopup.PointerDirection = PointerDirection.Up;
            bubblePopup.PreferredPointerDirection = PointerDirection.Up;
            bubblePopup.Target = menuButton;
            bubblePopup.BackgroundColor = Color.FromHex("202020");
            bubblePopup.IsVisible = true;
        }

        //Close page
        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }


        bool canRefresh = true;



        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Ios 13 bug
            try
            {
                if (!MainPage.isSelectingImage)
                    Navigation.PopModalAsync();
                else
                    MainPage.isSelectingImage = false;
            }
            catch
            {
                //fa nient
            }

        }
    }
}
