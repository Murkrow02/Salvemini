using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace SalveminiApp
{
    public partial class ArgoWidget : ContentView
    {
        public static Page pushPage;

        //Title
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(WidgetGradient), default(string), Xamarin.Forms.BindingMode.OneWay);
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

        //Icon
        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(WidgetGradient), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string Icon
        {
            get
            {
                return (string)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        //StartColor
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create(nameof(StartColor), typeof(string), typeof(WidgetGradient), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string StartColor
        {
            get
            {
                return (string)GetValue(StartColorProperty);
            }

            set
            {
                SetValue(StartColorProperty, value);
            }
        }

        //EndColor
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create(nameof(EndColor), typeof(string), typeof(WidgetGradient), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string EndColor
        {
            get
            {
                return (string)GetValue(EndColorProperty);
            }

            set
            {
                SetValue(EndColorProperty, value);
            }
        }

        //PushTo
        public static readonly BindableProperty PushProperty = BindableProperty.Create(nameof(Push), typeof(Page), typeof(WidgetGradient), default(Page), Xamarin.Forms.BindingMode.OneWay);
        public Page Push
        {
            get
            {
                return (Page)GetValue(PushProperty);
            }

            set
            {
                SetValue(PushProperty, value);
            }
        }

        //Order
        public static readonly BindableProperty OrderProperty = BindableProperty.Create(nameof(Order), typeof(int), typeof(WidgetGradient), default(int), Xamarin.Forms.BindingMode.OneWay);
        public int Order
        {
            get
            {
                return (int)GetValue(OrderProperty);
            }

            set
            {
                SetValue(OrderProperty, value);
            }
        }


        public ArgoWidget()
        {
            InitializeComponent();

            //Set dimensions
            image.WidthRequest = App.ScreenWidth / 4;
            image.HeightRequest = App.ScreenWidth / 4;
            view.WidthRequest = App.ScreenWidth / 3.5;
            view.HeightRequest = App.ScreenWidth / 3.5;
            mainLayout.WidthRequest = App.ScreenWidth / 3.5;
            mainLayout.HeightRequest = App.ScreenWidth / 3.5;

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
                    TitleLbl.Text = Title;
                }

                //Icon
                if (propertyName == IconProperty.PropertyName)
                {
                    if (!string.IsNullOrEmpty(Icon))
                        image.Source = Icon;
                }

                //StartColor
                if (propertyName == StartColorProperty.PropertyName)
                {
                    if (!string.IsNullOrEmpty(StartColor))
                        view.BackgroundGradientStartColor = Color.FromHex(StartColor);
                }

                //EndColor
                if (propertyName == EndColorProperty.PropertyName)
                {
                    if (!string.IsNullOrEmpty(EndColor))
                        view.BackgroundGradientEndColor = Color.FromHex(EndColor);
                }

                //Push
                if (propertyName == PushProperty.PropertyName)
                {
                    if (Push != null)
                    pushPage = Push;
                }
            }
            catch (Exception ex)
            {
                //Boh per sicurezza a volte fa cose strane
                Debug.WriteLine(ex);
                return;
            }

        }
    }
}
