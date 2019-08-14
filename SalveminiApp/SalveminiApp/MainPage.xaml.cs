using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SalveminiApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public List<RestApi.Models.Lezione> Orario = new List<RestApi.Models.Lezione>();

        public MainPage()
        {
            InitializeComponent();

            //Initialize interface
            adsFrame.HeightRequest = App.ScreenHeight / 14;
            todayFrame.WidthRequest = App.ScreenWidth / 2.6;
            calendarImage.WidthRequest = todayFrame.WidthRequest / 9;
            trainFrame.WidthRequest = App.ScreenWidth / 2.6;
            trainImage.WidthRequest = todayFrame.WidthRequest / 9;
            adviceImage.WidthRequest = App.ScreenWidth / 22;
            clockImage.WidthRequest = App.ScreenWidth / 22;
            labelView.HeightRequest = App.ScreenHeight / 10;

#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Spacing = 20;
            }
#endif
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Orario = await App.Orari.GetOrario("3FCAM", 1);

            if (Orario != null)
            {
                orarioList.ItemsSource = Orario;
            }

            //Avvisi Label starts to flow
            flowLabel();
           



        }

        void flowLabel()
        {
            var speed = ((double)avvisiLabel.Text.Length / 10) * 1000 + 6000;
            Point pointF = avvisiScroll.GetScrollPositionForElement(avvisiLabel, ScrollToPosition.End);
            pointF.X += App.ScreenWidth * 2;
            var animationF = new Animation(
                callback: x => avvisiScroll.ScrollToAsync(x, pointF.Y, animated: false),
                start: avvisiScroll.ScrollX,
                end: pointF.X);
            avvisiLabel.TranslationX = App.ScreenWidth;

            animationF.Commit(
            owner: this,
            name: "ScrollF",
            length: (uint)speed);

            avvisiScroll.ScrollToAsync(0, 0, false);
            avvisiLabel.TranslationX = App.ScreenWidth;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, (int)speed + 1000), () =>
            {
                animationF.Commit(
            owner: this,
            name: "ScrollF",
            length: (uint)speed);

                avvisiScroll.ScrollToAsync(0, 0, false);
                avvisiLabel.TranslationX = App.ScreenWidth;
                return true;
            });

        }
    }
}
