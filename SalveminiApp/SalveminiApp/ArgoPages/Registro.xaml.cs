using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Syncfusion.XForms.Expander;
using Forms9Patch;
using System.Linq;

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

            todayButton.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                getValues(DateTime.Today);
            }
        }

        async void getValues(DateTime date)
        {
            widgetsLayout.Children.Clear();
            var response = await App.Argo.GetOggi(date);
            if (string.IsNullOrEmpty(response.Message))
            {
                Oggi = response.Data as RestApi.Models.WholeModel;
                if (Oggi != null)
                {
                    if (Oggi.bacheca != null && Oggi.bacheca.Count > 0)
                    {
                        Bacheca = Oggi.bacheca;
                        setLayout("BAC");
                    }
                    if (Oggi.voti != null && Oggi.voti.Count > 0)
                    {
                        Voti = Oggi.voti;
                        setLayout("VOT");
                    }
                    if (Oggi.argomenti != null && Oggi.argomenti.Count > 0)
                    {
                        Argomenti = Oggi.argomenti;
                        setLayout("ARG");
                    }
                }
            }
        }

        void setLayout(string type)
        {
            //Static values for widgets
            string title = "";
            string color = "";
            bool IsExpanded = false;
            int rowHeight = 0;

            //Create List of contents of each widget
            Xamarin.Forms.ListView ExpanderContent = new Xamarin.Forms.ListView { HasUnevenRows = false, HorizontalScrollBarVisibility = ScrollBarVisibility.Never, VerticalScrollBarVisibility = ScrollBarVisibility.Never, VerticalOptions = LayoutOptions.Start, Footer = " " };
            switch (type)
            {
                case "BAC":
                    title = "Bacheca";
                    color = "FF8181";
                    rowHeight = 70;
                    ExpanderContent.ItemsSource = Bacheca;
                    IsExpanded = Bacheca.Count == 1;
                    ExpanderContent.ItemTemplate = new DataTemplate(() =>
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };

                        //Title Layout
                        var titleLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };

                        //Title Label
                        var titleLabel = new Xamarin.Forms.Label { FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Start };
                        titleLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "formattedTitle");
                        titleLayout.Children.Add(titleLabel);

                        //File icon
                        titleLayout.Children.Add(new Plugin.Iconize.IconLabel { FontSize = 20, Text = "far-file-alt", HorizontalOptions = LayoutOptions.Start });

                        //Pdf name
                        var descriptionLabel = new Forms9Patch.Label { Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
                        descriptionLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "desMessaggio");

                        //Add childrens to main layout
                        layout.Children.Add(titleLayout);
                        layout.Children.Add(descriptionLabel);
                        return new ViewCell { View = layout };
                    });
                    ExpanderContent.HeightRequest = Bacheca.Count * rowHeight;
                    break;
                case "VOT":
                    title = "Voti Giornalieri";
                    color = "7690FF";
                    rowHeight = 70;
                    ExpanderContent.ItemsSource = Voti;
                    IsExpanded = Voti.Count == 1;
                    ExpanderContent.ItemTemplate = new DataTemplate(() =>
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Padding = 10 };

                        //Subject and teacher layout
                        var titleLayout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.Start };

                        //Subject Label
                        var subjectLabel = new Forms9Patch.Label { FontSize = 20, FontAttributes = FontAttributes.Bold, Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
                        subjectLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "formattedSubject");
                        titleLayout.Children.Add(subjectLabel);

                        //Teacher Label
                        var teacherLabel = new Xamarin.Forms.Label { FontSize = 15, TextColor = Styles.TextGray, HorizontalOptions = LayoutOptions.Start };
                        teacherLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "docente");
                        titleLayout.Children.Add(teacherLabel);

                        //Mark Label
                        var markLabel = new Xamarin.Forms.Label { FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.EndAndExpand, FontSize = 30 };
                        markLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "codVoto");
                        markLabel.SetBinding(Xamarin.Forms.Label.TextColorProperty, "markColor");

                        //Add childrens to main layout
                        layout.Children.Add(titleLayout);
                        layout.Children.Add(markLabel);
                        return new ViewCell { View = layout };
                    });
                    ExpanderContent.HeightRequest = Bacheca.Count * rowHeight;
                    break;
                case "ARG":
                    title = "Argomenti Lezione";
                    color = "64EB7D";
                    rowHeight = 100;
                    ExpanderContent.ItemsSource = Argomenti;
                    IsExpanded = Argomenti.Count == 1;
                    ExpanderContent.ItemTemplate = new DataTemplate(() =>
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };

                        //Layout with subject and teacher
                        var titleLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };

                        //Subject Label
                        var subjectLabel = new Forms9Patch.Label { FontSize = 20, FontAttributes = FontAttributes.Bold, Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
                        subjectLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "formattedSubject");
                        titleLayout.Children.Add(subjectLabel);

                        //Teacher Label
                        var teacherLabel = new Xamarin.Forms.Label { FontSize = 15, TextColor = Styles.TextGray, HorizontalOptions = LayoutOptions.Start };
                        teacherLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "docente");
                        titleLayout.Children.Add(teacherLabel);

                        //Argomento lezione label
                        var argumentLabel = new Forms9Patch.Label { FontSize = 15, Lines = 3, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
                        argumentLabel.SetBinding(Xamarin.Forms.Label.TextProperty, "Contenuto");
                        
                        //Add childrens to main layout
                        layout.Children.Add(titleLayout);
                        layout.Children.Add(argumentLabel);
                        return new ViewCell { View = layout };
                    });
                    ExpanderContent.HeightRequest = Bacheca.Count * rowHeight;
                    break;

            }
            ExpanderContent.RowHeight = rowHeight;
            var mainFrame = new Xamarin.Forms.Frame { HasShadow = false, CornerRadius = 15, IsClippedToBounds = true, Padding = 0 };
            var expander = new SfExpander { IconColor = Color.White, HeaderBackgroundColor = Color.FromHex(color), Header = new Xamarin.Forms.Frame { Content = new Xamarin.Forms.Label { FontSize = 20, Text = title, TextColor = Color.White, HorizontalOptions = LayoutOptions.Start }, HasShadow = false, CornerRadius = 0, Padding = 10, BackgroundColor = Color.FromHex(color), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Start } };
            expander.Content = ExpanderContent;
            expander.IsExpanded = IsExpanded;
            mainFrame.Content = expander;
            widgetsLayout.Children.Add(mainFrame);
        }

        void ChangeDate_Clicked(object sender, EventArgs e)
        {
            datepicker.Focus();
        }

        private void Picker_SelectedDate(object sender, DateChangedEventArgs e)
        {
            todayButton.Text = e.NewDate.ToString("dd/MM/yyyy");
        }

        private void Picker_Unfocused(object sender, FocusEventArgs e)
        {
            getValues(datepicker.Date);
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