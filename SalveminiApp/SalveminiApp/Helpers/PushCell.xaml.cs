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
        //Badge
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
            }
            catch
            {
                //Boh per sicurezza a volte fa cose strane
                return;
            }

        }
    }
}


