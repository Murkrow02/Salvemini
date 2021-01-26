using System;

using Xamarin.Forms;

namespace SalveminiApp
{
    public class MyPage : ContentPage
    {
        public MyPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Il progetto SalveminiApp è stato momentaneamente sospeso dagli sviluppatori :(", VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center }
                }
            };
        }
    }
}

