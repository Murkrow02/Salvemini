using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.Controls
{
    public partial class PercentageBar : ContentView
    {
        public PercentageBar()
        {
            InitializeComponent();
        }

        //Title
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(PercentageBar), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }
        }

        //BgColor
        public static readonly BindableProperty BgColorProperty = BindableProperty.Create(nameof(BgColor), typeof(string), typeof(PercentageBar), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string BgColor
        {
            get
            {
                return (string)GetValue(BgColorProperty);
            }

            set
            {
                SetValue(BgColorProperty, value);
            }
        }

        //Percentage
        public static readonly BindableProperty PercentageProperty = BindableProperty.Create(nameof(Percentage), typeof(int), typeof(PercentageBar), default(int), Xamarin.Forms.BindingMode.OneWay);
        public int Percentage
        {
            get
            {
                return (int)GetValue(PercentageProperty);
            }

            set
            {
                SetValue(PercentageProperty, value);
            }
        }

        //Update values
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            try
            {

                //Title
                if (propertyName == TitleProperty.PropertyName)
                {
                    if (!string.IsNullOrEmpty(Title))
                        TitleLbl.HtmlText = Title;
                }

                //BgColor
                if (propertyName == BgColorProperty.PropertyName)
                {
                    if (!string.IsNullOrEmpty(BgColor))
                        percentageFrame.BackgroundColor = Color.FromHex(BgColor);
                }

                //Percentage
                if (propertyName == PercentageProperty.PropertyName)
                {
                    percentageNumber.Text = Percentage.ToString() + "%";

                    //Set frame width
                    parentFrame.WidthRequest = App.ScreenWidth * 0.8;
                    percentageFrame.WidthRequest = App.ScreenWidth * 0.8 / 100 * Percentage;
                }

            }
            catch (Exception ex)
            {
                //Boh per sicurezza a volte fa cose strane
                return;
            }

        }


    }
}
