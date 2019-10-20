using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp
{
    public partial class WidgetGradient : ContentView
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

        //Subtitle
        public static readonly BindableProperty SubTitleProperty = BindableProperty.Create(nameof(SubTitle), typeof(string), typeof(WidgetGradient), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string SubTitle
        {
            get
            {
                return (string)GetValue(SubTitleProperty);
            }

            set
            {
                SetValue(SubTitleProperty, value);
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


        public WidgetGradient()
        {
            InitializeComponent();

            //Set icon width
            iconImg.WidthRequest = App.ScreenWidth / 21;
        }

        //Update values
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            //Title
            if (propertyName == TitleProperty.PropertyName)
            {
                TitleLbl.Text = Title;
            }

            //SubTitle
            if (propertyName == SubTitleProperty.PropertyName)
            {
                SubTitleLbl.Text = SubTitle;
            }

            //Icon
            if (propertyName == IconProperty.PropertyName)
            {
                iconImg.Source = Icon;
            }

            //StartColor
            if (propertyName == StartColorProperty.PropertyName)
            {
                view.BackgroundGradientStartColor = Color.FromHex(StartColor);
            }

            //EndColor
            if (propertyName == EndColorProperty.PropertyName)
            {
                view.BackgroundGradientEndColor = Color.FromHex(EndColor);
            }

            //Push
            if (propertyName == PushProperty.PropertyName)
            {
                pushPage = Push;
            }
        }
    }
}
