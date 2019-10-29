using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Syncfusion.XForms.Expander;
using Forms9Patch;

namespace SalveminiApp.ArgoPages
{
    public partial class Registro : ContentPage
    {
        public RestApi.Models.WholeModel Oggi = new RestApi.Models.WholeModel();
        public List<RestApi.Models.Assenza> Assenze = new List<RestApi.Models.Assenza>();
        public List<RestApi.Models.Bacheca> Bacheca = new List<RestApi.Models.Bacheca>();
        public List<RestApi.Models.Voti> Voti = new List<RestApi.Models.Voti>();
        public List<RestApi.Models.Argomenti> Argomenti = new List<RestApi.Models.Argomenti>();
        public List<RestApi.Models.Compiti> Compiti = new List<RestApi.Models.Compiti>();
        public List<RestApi.Models.Promemoria> Promemoria = new List<RestApi.Models.Promemoria>();

        public Registro()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var response = await App.Argo.GetOggi(DateTime.Today);
                if (string.IsNullOrEmpty(response.Message))
                {
                    Oggi = response.Data as RestApi.Models.WholeModel;
                    if (Oggi != null)
                    {
                        if (Oggi.bacheca.Count > 0)
                        {
                            Bacheca = Oggi.bacheca;
                            setLayout("BAC");
                        }
                    }
                }
            }
        }

        void setLayout(string type)
        {
            string title = "";
            Xamarin.Forms.ListView ExpanderContent = new Xamarin.Forms.ListView { HasUnevenRows = false, RowHeight = 70, VerticalOptions = LayoutOptions.Start, Footer = " " };
            switch (type)
            {
                case "BAC":
                    title = "Bacheca";
                    ExpanderContent.ItemsSource = Bacheca;
                    ExpanderContent.ItemTemplate = new DataTemplate(() =>
                    {
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };
                        var titleLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };
                        var titleLabel = new Xamarin.Forms.Label { FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Start };
                        titleLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "formattedTitle");
                        titleLayout.Children.Add(titleLabel);
                        titleLayout.Children.Add(new Plugin.Iconize.IconLabel { FontSize = 20, Text = "far-file-alt", HorizontalOptions = LayoutOptions.Start });
                        layout.Children.Add(titleLayout);
                        var descriptionLabel = new Forms9Patch.Label { Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
                        descriptionLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "desMessaggio");
                        layout.Children.Add(descriptionLabel);
                        return new ViewCell { View = layout };
                    });
                    ExpanderContent.HeightRequest = Bacheca.Count * 70;
                    break;
            }

            var mainFrame = new Xamarin.Forms.Frame { HasShadow = false, CornerRadius = 15, IsClippedToBounds = true, Padding = 0 };
            var expander = new SfExpander { IconColor = Color.White, HeaderBackgroundColor = Color.FromHex("FF8181"), Header = new Xamarin.Forms.Frame { Content = new Xamarin.Forms.Label { FontSize = 20, Text = title, TextColor = Color.White, HorizontalOptions = LayoutOptions.Start }, HasShadow = false, CornerRadius = 0, Padding = 10, BackgroundColor = Color.FromHex("FF8181"), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Start } };
            expander.Content = ExpanderContent;
            mainFrame.Content = expander;
            widgetsLayout.Children.Add(mainFrame);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //Ios 13 bug
            try
            {
                Navigation.PopModalAsync();
            }
            catch
            {
                //fa nient
            }
        }
    }
}