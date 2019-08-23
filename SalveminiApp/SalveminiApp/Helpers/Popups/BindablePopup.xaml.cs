using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace SalveminiApp.Helpers.Popups
{
    public partial class BindablePopup : PopupPage
    {
        public BindablePopup(string titleText, string svgSource, StackLayout childLayout)
        {
            InitializeComponent();

            //Init Sizes
            trailingSvg.WidthRequest = App.ScreenWidth / 13;
            trailingSvg.HeightRequest = App.ScreenWidth / 13;

            //Prepare Interface
            if (!string.IsNullOrEmpty(titleText))
            {
                title.Text = titleText;
            }
            if (!string.IsNullOrEmpty(svgSource))
            {
                trailingSvg.Source = svgSource;
            }
            if (childLayout != null)
            {
                layout.Children.Add(childLayout);
            }
        }
    }
}



