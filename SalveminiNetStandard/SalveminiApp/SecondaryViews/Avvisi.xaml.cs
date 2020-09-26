using System;
using System.Collections.Generic;
using Xamarin.Forms;
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
            DependencyService.Get<IPlatformSpecific>().SetSafeArea(this);

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
                //Show loading
                loading.IsRunning = true;

                //Download new avvisi from api
                var newAvvisi = await App.Avvisi.GetAvvisi();

                //Check if all went good
                if (newAvvisi != null)
                {
                    Avvisis = newAvvisi;
                    avvisiCarousel.ItemsSource = Avvisis;
                    Preferences.Set("LastAvviso", Avvisis[0].id);
                    MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "RemoveBadge", "Avvisi");
                    loading.IsRunning = false;
                    return;
                }

                //Cannot download avvisi
                Costants.showToast("Non è stato possibile scaricare i nuovi avvisi, controlla la tua connessione e riprova");
                loading.IsRunning = true;
            }
            catch //Unexpected error
            {
                Costants.showToast("Si è verificato un errore, riprova più tardi");
                loading.IsRunning = true;
            }
        }


        //Open full image list
        private void ImageList_ItemSelected(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            MainPage.isSelectingImage = true;
            canRefresh = false;

            try
            {
                //Get images
                var listView = sender as FlowListView;
                var immagini = listView.FlowItemsSource as List<string>;
                var selected = e.Item as string;
                //Get selected item

                //Get avviso title
                var title = "";
                try { title = ((listView.Parent as Xamarin.Forms.StackLayout).Children[0] as Xamarin.Forms.Label).Text; } catch { };

                //Create stormlion image list
                var imageList = new List<PhotoBrowser.Photo>();
                int index = 1; foreach (var immagine in immagini) { imageList.Add(new PhotoBrowser.Photo { Title = title + " " + index + "/" + immagini.Count, URL = immagine }); index++; }
                var imageViewer = new PhotoBrowser.PhotoBrowser();
                imageViewer.Photos = imageList;
                imageViewer.StartIndex = immagini.IndexOf(selected);
                imageViewer.Show();
            }
            catch
            {
                Costants.showToast("Non è stato possibile caricare le immagini, riprova più tardi");
            }

        }

        //Show more text
        void ShowMore_Clicked(object sender, EventArgs e)
        {
            //Get the text
            var text = (((sender as Xamarin.Forms.Button).Parent as Xamarin.Forms.StackLayout).Children[2] as Xamarin.Forms.Label).Text;

            //Get the full description
            var fullDesc = Avvisis.FirstOrDefault(x => x.Descrizione.Contains(text.Replace("...", ""))).Descrizione;

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
            bubblePopup.Content = new Xamarin.Forms.ListView { WidthRequest = App.ScreenWidth / 8, BackgroundColor = Color.Transparent, ItemsSource = Avvisis, ItemTemplate = new DataTemplate(() => { var lbl = new Xamarin.Forms.Label { VerticalOptions = LayoutOptions.Center, TextColor = Color.White, HorizontalOptions = LayoutOptions.Center }; lbl.SetBinding(Xamarin.Forms.Label.TextProperty, "Title"); return new ViewCell { View = lbl }; }) };
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

            if (MainPage.isSelectingImage)
                MainPage.isSelectingImage = false;
        }
    }
}
