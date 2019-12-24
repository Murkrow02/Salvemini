using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace SalveminiApp.FlappyMimmo
{
	public partial class FlappyHome : ContentPage
	{
		public FlappyHome()
		{
			InitializeComponent();

			//Set Sizes
			buttonFrame.WidthRequest = App.ScreenWidth / 6;
			buttonFrame.HeightRequest = App.ScreenWidth / 6;
			buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
			singleImage.WidthRequest = App.ScreenWidth * 0.35;
			singleImage.HeightRequest = App.ScreenWidth * 0.35;
			boardImage.WidthRequest = App.ScreenWidth * 0.58;
			boardImage.HeightRequest = App.ScreenWidth * 0.35;
		}

        void Play_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new GamePage());
        }

		void Shop_Tapped(object sender, EventArgs e)
		{
			Navigation.PushPopupAsync(new Shop());
		}

		void Close_Clicked(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}
