using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.Controls
{
    public partial class ExtraWidget : ContentView
    {
        public ExtraWidget(string title, string desc, string image, Color startColor, Color endColor, Page pushPage)
        {
            InitializeComponent();

            //Set values
            TitleLbl.Text = title;
            SubTitleLbl.Text = desc;
            frameIcon.Source = image;
            frame.BackgroundGradientStartColor = startColor;
            frame.BackgroundGradientEndColor = endColor;

            //Set width
            frameIcon.WidthRequest = App.ScreenWidth / 8;

            //Create tapped gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            this.GestureRecognizers.Add(tapGestureRecognizer);

            //Push to selected page
            void TapGestureRecognizer_Tapped(object sender, EventArgs e)
            {
                try
                {
                    var ExtraPage = this.GetParentPage();

                    if (TitleLbl.Text == "Flappy Mimmo")
                    {
                        MainPage.isSelectingImage = true;
                        ExtraPage.Navigation.PushModalAsync(new FlappyMimmo.FlappyHome());
                        return;
                    }

                    ExtraPage.Navigation.PushAsync(pushPage);

                }
                catch { }
            }

        }


    }
}
