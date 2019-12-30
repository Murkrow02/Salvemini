using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.Controls
{
    public partial class TeamPersonFrame : ContentView
    {
        public TeamPersonFrame()
        {
            InitializeComponent();

            //Set image sizes
            //image.WidthRequest = App.ScreenWidth / 7;
            //image.HeightRequest = App.ScreenWidth / 7;

            //iconImg.WidthRequest = App.ScreenWidth / 12;
            //iconImg.HeightRequest = App.ScreenWidth / 12;
        }

        //Title
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(TeamPersonFrame), default(string), Xamarin.Forms.BindingMode.OneWay);
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

        //SubTitle
        public static readonly BindableProperty SubTitleProperty = BindableProperty.Create(nameof(SubTitle), typeof(string), typeof(TeamPersonFrame), default(string), Xamarin.Forms.BindingMode.OneWay);
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

        //Image
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(TeamPersonFrame), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string Image
        {
            get
            {
                return (string)GetValue(ImageProperty);
            }

            set
            {
                SetValue(ImageProperty, value);
            }
        }

        //Icon
        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(TeamPersonFrame), default(string), Xamarin.Forms.BindingMode.OneWay);
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

        //IconUrl
        public static readonly BindableProperty IconUrlProperty = BindableProperty.Create(nameof(IconUrl), typeof(string), typeof(TeamPersonFrame), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string IconUrl
        {
            get
            {
                return (string)GetValue(IconUrlProperty);
            }

            set
            {
                SetValue(IconUrlProperty, value);
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
                    titleLbl.Text = Title;
                }

                //SubTitle
                if (propertyName == SubTitleProperty.PropertyName)
                {
                    subTitleLbl.Text = SubTitle;
                }

                //Image
                if (propertyName == ImageProperty.PropertyName)
                {
                    image.Source = Image;
                }

                //Icon
                if (propertyName == ImageProperty.PropertyName)
                {
                    iconImg.Source = Icon;
                }

                
            }
            catch
            {
                //Boh per sicurezza a volte fa cose strane
                return;
            }
        }

        void openUrl_Clicked(object sender, EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri(IconUrl));
            }
            catch
            {
                //Fa nient
            }
        }
    }
}
