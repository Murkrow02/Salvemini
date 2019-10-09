using System;
using System.Collections.Generic;
using Xamarin.Forms;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
using Xamarin.Essentials;

namespace SalveminiApp.SecondaryViews
{
    public partial class Avvisi : ContentPage
    {
        public List<RestApi.Models.Avvisi> Avvisis = new List<RestApi.Models.Avvisi>();

        public Avvisi()
        {
            InitializeComponent();

            //Set Safearea
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
#endif

            //imageList.FlowItemsSource = new List<string> { "https://raw.githubusercontent.com/daniel-luberda/DLToolkit.Forms.Controls/master/FlowListView/Screenshots/flowlistview5.png", "https://raw.githubusercontent.com/daniel-luberda/DLToolkit.Forms.Controls/master/FlowListView/Screenshots/flowlistview_ios2.png", "https://raw.githubusercontent.com/daniel-luberda/DLToolkit.Forms.Controls/master/FlowListView/Screenshots/flowlistview1.png" }; 
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Avvisis = await App.Avvisi.GetAvvisi();

                if (Avvisis != null)
                {
                    avvisiCarousel.ItemsSource = Avvisis;
                }
            }
        }
    }
}
