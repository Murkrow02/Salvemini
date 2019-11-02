using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.Helpers
{
    public partial class PollOption : ContentView
    {
        //Title
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(PollOption), default(string), Xamarin.Forms.BindingMode.OneWay);
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

        //Title
        public static readonly BindableProperty optionIdProperty = BindableProperty.Create(nameof(optionId), typeof(int), typeof(PollOption), default(int), Xamarin.Forms.BindingMode.OneWay);
        public int optionId
        {
            get
            {
                return (int)GetValue(optionIdProperty);
            }

            set
            {
                SetValue(optionIdProperty, value);
            }
        }

        //Image
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(PollOption), default(string), Xamarin.Forms.BindingMode.OneWay);
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

        public PollOption()
        {
            InitializeComponent();
            view.WidthRequest = App.ScreenWidth / 2.5;

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
                    questionLbl.Text = Title;
                }

                //Image
                if (propertyName == ImageProperty.PropertyName)
                {
                    //Image found
                    if (Image != "no")
                    {
                        optionImage.Source = Image;
                        labelFrame.BackgroundColor = Color.Transparent;
                    }
                    else //No image
                    {
                        imageFrame.IsVisible = false;
                        labelFrame.VerticalOptions = LayoutOptions.CenterAndExpand;
                    }
                }
            }
            catch
            {
                //Boh per sicurezza a volte fa cose strane
                return;
            }
        }
    }
}
