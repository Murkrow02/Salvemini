﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
using Xamarin.Essentials;
using Plugin.Toasts;
using DLToolkit.Forms.Controls;

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

        }

        private void ImageList_ItemSelected(object sender, ItemTappedEventArgs e)
        {
            var a = sender as FlowListView;
            var b = a.FlowItemsSource as List<string>;
            Navigation.PushModalAsync(new Helpers.ImageViewer(b));
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var notificator = DependencyService.Get<IToastNotificator>();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Avvisis = await App.Avvisi.GetAvvisi();

                if (Avvisis != null)
                {
                    avvisiCarousel.ItemsSource = Avvisis;
                }
            }
            else
            {
                var options = new NotificationOptions()
                {
                    Description = "Nessuna connessione ad internet 🚀",
                };

                var result = await notificator.Notify(options);
            }
        }
    }
}
