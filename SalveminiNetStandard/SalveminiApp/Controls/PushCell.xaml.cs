using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.Helpers
{
    public partial class PushCell : ContentView
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
        //Separator
        public static readonly BindableProperty SeparatorProperty = BindableProperty.Create(nameof(Separator), typeof(string), typeof(WidgetGradient), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string Separator
        {
            get
            {
                return (string)GetValue(SeparatorProperty);
            }

            set
            {
                SetValue(SeparatorProperty, value);
            }
        }

        //IsLoading
        public static readonly BindableProperty LoadingProperty = BindableProperty.Create(nameof(Loading), typeof(bool), typeof(WidgetGradient), default(bool), Xamarin.Forms.BindingMode.OneWay);
        public bool Loading
        {
            get
            {
                return (bool)GetValue(LoadingProperty);
            }

            set
            {
                SetValue(LoadingProperty, value);
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

        async void animateBg(object sender, System.EventArgs e)
        {
            await cell.FadeTo(0.5, 100);
            await cell.FadeTo(1, 100);
        }

        public PushCell()
        {
            InitializeComponent();
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
                    TitleLbl.Text = Title;
                }

                //Separator
                if (propertyName == SeparatorProperty.PropertyName)
                {
                    if (Separator == "si")
                        separator.IsVisible = true;
                    else
                        separator.IsVisible = false;
                }

                //Push
                if (propertyName == PushProperty.PropertyName)
                {
                    pushPage = Push;
                }

                //Loading
                if (propertyName == LoadingProperty.PropertyName)
                {
                    if (Loading)
                    {
                        loading.HeightRequest = TitleLbl.Height;
                        loading.IsRunning = true;
                        loading.IsVisible = true;
                        arrow.IsVisible = false;
                        TitleLbl.Opacity = 0.7;
                    }
                    else
                    {
                        loading.IsRunning = false;
                        loading.IsVisible = false;
                        arrow.IsVisible = true;
                        TitleLbl.Opacity = 1;
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


