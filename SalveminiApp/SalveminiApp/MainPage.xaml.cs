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
        public MainPage()
        {
            InitializeComponent();

            //Initialize interface
            adsFrame.HeightRequest = App.ScreenHeight / 5.4;
            todayFrame.WidthRequest = App.ScreenWidth / 2.6;
            todayFrame.HeightRequest = App.ScreenHeight / 8.5;
            calendarImage.WidthRequest = todayFrame.WidthRequest / 9;
            trainFrame.WidthRequest = App.ScreenWidth / 2.6;
            trainFrame.HeightRequest = App.ScreenHeight / 8.5;
            trainImage.WidthRequest = todayFrame.WidthRequest / 9;
            adviceImage.WidthRequest = App.ScreenWidth / 22;
            clockImage.WidthRequest = App.ScreenWidth / 22;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();


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
