using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Syncfusion.XForms.Expander;
using Forms9Patch;
using System.Linq;
using Newtonsoft.Json;
using MonkeyCache.SQLite;
#if __IOS__
using UIKit;
#endif

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


        bool gotByCache = false;

        public Registro()
        {
            InitializeComponent();

            todayButton.Text = DateTime.Today.ToString("dd/MM/yyyy");

            getDayCache(DateTime.Today.ToString("yyyy-MM-dd"));

            //Set max and min date
            datepicker.MaximumDate = DateTime.Now;
            datepicker.MinimumDate = DateTime.Now.AddYears(-1);

#if __IOS__
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 35, 0, 0);
            }
#endif
        }

        void getDayCache(string date)
        {
            if (Barrel.Current.Exists("Oggi" + date))
            {
                try
                {
                    widgetsLayout.Children.Clear();
                    Oggi = CacheHelper.GetCache<RestApi.Models.WholeModel>("Oggi" + date);
                    if (Oggi != null)
                    {
                        gotByCache = true;
                        var data = Convert.ToDateTime(date);
                        callSetLayout(data.Day == DateTime.Today.Day && data.Month == DateTime.Today.Month && data.Year == DateTime.Today.Year);

                    }
                }
                catch (Exception ex)
                {
                    //Failed stacca stacca
                    Barrel.Current.Empty("Oggi" + date);
                }

            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Status bar color
#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            }
#endif
            if (Oggi.assenze != null && Oggi.compiti != null && Oggi.argomenti != null && Oggi.promemoria != null && Oggi.bacheca != null && Oggi.voti != null)
            {
                getDayCache(DateTime.Today.ToString("yyyy-MM-dd"));
            }

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                getValues(DateTime.Today);
            }
            else
            {
                //No internet
                Costants.showToast("connection");
            }

            //Remove badge
            MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "RemoveBadge", "Registro");

        }

        public bool ObjectsDiffer(RestApi.Models.WholeModel callObj, RestApi.Models.WholeModel cachedObj)
        {
            if (callObj == null || cachedObj == null)
            {
                return true;
            }

            var firstString = JsonConvert.SerializeObject(callObj);
            var secondString = JsonConvert.SerializeObject(cachedObj);

            if (firstString == secondString)
                return false;
            else
                return true;
        }


        async void getValues(DateTime date)
        {
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            var response = await App.Argo.GetOggi(date);
            if (string.IsNullOrEmpty(response.Message))
            {
                var calledOggi = response.Data as RestApi.Models.WholeModel;
                if (calledOggi != null && ObjectsDiffer(calledOggi, Oggi))
                {
                    widgetsLayout.Children.Clear();
                    Oggi = calledOggi;
                    callSetLayout(date.Day == DateTime.Today.Day && date.Month == DateTime.Today.Month && date.Year == DateTime.Today.Year);
                }
            }
            else
            {
                await DisplayAlert("Errore", response.Message, "Ok");
            }
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
        }

        void callSetLayout(bool isToday)
        {
            nothingLayout.IsVisible = false;
            if (Oggi.bacheca.Count == 0 && Oggi.voti.Count == 0 && Oggi.argomenti.Count == 0 && Oggi.compiti.Count == 0 && Oggi.promemoria.Count == 0 && Oggi.assenze.Count == 0)
            {
                placeholderLabel.Text = isToday ? "Oggi non è successo niente" : "Questo giorno non è successo niente";
                nothingLayout.IsVisible = true;
            }

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
            if (Oggi.compiti != null && Oggi.compiti.Count > 0)
            {
                Compiti = Oggi.compiti;
                setLayout("COM");
            }
            if (Oggi.promemoria != null && Oggi.promemoria.Count > 0)
            {
                Promemoria = Oggi.promemoria;
                setLayout("PRO");
            }
            if (Oggi.assenze != null && Oggi.assenze.Count > 0)
            {
                Assenze = Oggi.assenze;
                setLayout("ASS");
            }
        }

        void setLayout(string type)
        {
            //Static values for widgets
            string title = "";
            string color = "";
            bool IsExpanded = false;
            List<View> Content = new List<View>();

            //Create List of contents of each widget
            switch (type)
            {
                case "BAC":
                    title = "Bacheca";
                    color = "FF8181";
                    IsExpanded = Bacheca.Count == 1;
                    foreach (var bacheca in Bacheca)
                    {
                        var gesture = new TapGestureRecognizer();
                        gesture.Tapped += Gesture_Tapped;

                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };
                        layout.GestureRecognizers.Add(gesture);
                        //Title Layout
                        var titleLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };

                        //Title Label
                        var titleLabel = new Xamarin.Forms.Label { FontSize = 20, Text = bacheca.formattedTitle, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Start };
                        titleLayout.Children.Add(titleLabel);

                        //File icon
                        titleLayout.Children.Add(new Plugin.Iconize.IconLabel { FontSize = 20, Text = "far-file-alt", HorizontalOptions = LayoutOptions.Start });

                        //Pdf name
                        var descriptionLabel = new Forms9Patch.Label { Lines = 1, Text = bacheca.desMessaggio, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };

                        //Add childrens to main layout
                        layout.Children.Add(titleLayout);
                        layout.Children.Add(descriptionLabel);
                        Content.Add(layout);

                        async void Gesture_Tapped(object sender, EventArgs e)
                        {
                            try
                            {
                                Device.OpenUri(new Uri(bacheca.Allegati[0].fullUrl));

                                //Post visualizzazione
                                var response = await App.Argo.VisualizzaBacheca(new RestApi.Models.VisualizzaBacheca { presaVisione = true, prgMessaggio = bacheca.Allegati[0].prgMessaggio });
                            }
                            catch
                            {
                                DisplayAlert("Errore", "Non è stato possibile scaricare l'allegato", "Ok");
                            }
                        }
                    }

                    break;
                case "VOT":
                    title = "Voti Giornalieri";
                    color = "7690FF";
                    IsExpanded = Voti.Count == 1;
                    foreach (var voto in Voti)
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Padding = 10 };

                        //Subject and teacher layout
                        var titleLayout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.Start };

                        //Subject Label
                        var subjectLabel = new Forms9Patch.Label { FontSize = 20, Text = voto.formattedSubject, FontAttributes = FontAttributes.Bold, Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.StartAndExpand };
                        titleLayout.Children.Add(subjectLabel);

                        //Teacher Label
                        var teacherLabel = new Xamarin.Forms.Label { FontSize = 15, Text = voto.formattedTeacher, TextColor = Styles.TextGray, HorizontalOptions = LayoutOptions.Start };
                        titleLayout.Children.Add(teacherLabel);

                        //Mark Label
                        var markLabel = new Forms9Patch.Label { FontAttributes = FontAttributes.Bold, TextColor = voto.markColor, Fit = LabelFit.Width, MaxLines= 1, Text = voto.codVoto, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 30 };

                        //Add childrens to main layout
                        layout.Children.Add(titleLayout);
                        layout.Children.Add(markLabel);
                        Content.Add(layout);
                    }
                    break;
                case "ARG":
                    title = "Argomenti Lezione";
                    color = "64EB7D";
                    IsExpanded = Argomenti.Count == 1;
                    foreach (var argomento in Argomenti)
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };

                        //Layout with subject and teacher
                        var titleLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

                        //Subject Label
                        var subjectLabel = new Forms9Patch.Label { FontSize = 20, VerticalOptions = LayoutOptions.Center, Text = argomento.Materia, FontAttributes = FontAttributes.Bold, Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.StartAndExpand };
                        titleLayout.Children.Add(subjectLabel);

                        //Teacher Label
                        var teacherLabel = new Forms9Patch.Label { FontSize = 10, Lines = 1, AutoFit = AutoFit.Width, Text = argomento.formattedTeacher, TextColor = Styles.TextGray, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End };
                        titleLayout.Children.Add(teacherLabel);

                        //Argomento lezione label
                        var argumentLabel = new Forms9Patch.Label { FontSize = 15, Lines = 8, Text = argomento.Contenuto, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };

                        //Add childrens to main layout
                        layout.Children.Add(titleLayout);
                        layout.Children.Add(argumentLabel);
                        Content.Add(layout);
                    }
                    break;
                case "COM":
                    title = "Compiti Assegnati";
                    color = "FFAF52";
                    IsExpanded = Compiti.Count == 1;
                    foreach (var compito in Compiti)
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };

                        //Layout with subject and teacher
                        var titleLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

                        //Subject Label
                        var subjectLabel = new Forms9Patch.Label { FontSize = 20, VerticalOptions = LayoutOptions.Center, Text = compito.Materia, FontAttributes = FontAttributes.Bold, Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.StartAndExpand };
                        titleLayout.Children.Add(subjectLabel);

                        //Teacher Label
                        var teacherLabel = new Forms9Patch.Label { FontSize = 10, Lines = 1, AutoFit = AutoFit.Width, Text = compito.formattedTeacher, TextColor = Styles.TextGray, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End };
                        titleLayout.Children.Add(teacherLabel);

                        //Argomento lezione label
                        var argumentLabel = new Forms9Patch.Label { FontSize = 15, Lines = 8, Text = compito.Contenuto, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };

                        //Add childrens to main layout
                        layout.Children.Add(titleLayout);
                        layout.Children.Add(argumentLabel);
                        Content.Add(layout);
                    }
                    break;
                case "PRO":
                    title = "Promemoria";
                    color = "FF5FFF";
                    IsExpanded = Promemoria.Count == 1;
                    foreach (var promemoria in Promemoria)
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };

                        //Promemoria Label
                        var reminderLabel = new Forms9Patch.Label { FontSize = 20, VerticalOptions = LayoutOptions.Center, Text = promemoria.desAnnotazioni, FontAttributes = FontAttributes.Bold, Lines = 3, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.StartAndExpand };

                        //Teacher Label
                        var teacherLabel = new Forms9Patch.Label { FontSize = 10, Lines = 1, AutoFit = AutoFit.Width, Text = promemoria.desMittente, TextColor = Styles.TextGray, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End };

                        //Add childrens to main layout
                        layout.Children.Add(reminderLabel);
                        layout.Children.Add(teacherLabel);
                        Content.Add(layout);
                    }
                    break;
                case "ASS":
                    title = "Assenza";
                    color = "FFD800";
                    IsExpanded = Assenze.Count == 1;
                    foreach (var assenza in Assenze)
                    {
                        //Main Layout
                        var layout = new Xamarin.Forms.StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10), Orientation = StackOrientation.Horizontal, Spacing = 5, VerticalOptions = LayoutOptions.FillAndExpand };

                        //Assenza Label
                        var assenzaLabel = new Forms9Patch.Label { FontSize = 20, VerticalOptions = LayoutOptions.Center, Text = assenza.FormattedInfo, FontAttributes = FontAttributes.Bold, Lines = 1, AutoFit = AutoFit.Width, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.StartAndExpand };

                        //Layout with status and teacher
                        var statusLayout = new Xamarin.Forms.StackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End };

                        //Status Label
                        var statusLabel = new Plugin.Iconize.IconLabel { Text = "fas-circle", TextColor = Color.FromHex(assenza.StatusColor), FontSize = 20, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End };
                        statusLayout.Children.Add(statusLabel);

                        //Teacher Label
                        var teacherLabel = new Forms9Patch.Label { FontSize = 10, Lines = 1, AutoFit = AutoFit.Width, Text = assenza.Professore, TextColor = Styles.TextGray, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End };
                        statusLayout.Children.Add(teacherLabel);

                        //Add childrens to main layout
                        layout.Children.Add(assenzaLabel);
                        layout.Children.Add(statusLayout);
                        Content.Add(layout);
                    }
                    break;


            }
            var mainFrame = new Xamarin.Forms.Frame { HasShadow = false, CornerRadius = 15, IsClippedToBounds = true, Padding = 0 };
            var expander = new SfExpander { IconColor = Color.White, HeaderBackgroundColor = Color.FromHex(color), Header = new Xamarin.Forms.Frame { Content = new Xamarin.Forms.Label { FontSize = 20, Text = title, TextColor = Color.White, HorizontalOptions = LayoutOptions.Start }, HasShadow = false, CornerRadius = 0, Padding = 10, BackgroundColor = Color.FromHex(color), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Start } };
            var contentLayout = new Xamarin.Forms.StackLayout();
            contentLayout.Children.AddRange(Content);
            expander.Content = contentLayout;
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
            if (datepicker.Date.ToString("dd/MM/yyyy") != savedDate)
            {
                getDayCache(datepicker.Date.ToString("yyyy-MM-dd"));
                getValues(datepicker.Date);
            }
        }
        public static string savedDate = "";
        private void Picker_Focused(object sender, FocusEventArgs e)
        {
            savedDate = todayButton.Text;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

#if __IOS__
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
#endif
            //Ios 13 bug
            try
            {
                todayButton.Text = DateTime.Today.ToString("dd/MM/yyyy");
                widgetsLayout.Children.Clear();
                Navigation.PopModalAsync();
            }
            catch
            {
                //fa nient
            }
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            todayButton.Text = DateTime.Today.ToString("dd/MM/yyyy");
            widgetsLayout.Children.Clear();
            Navigation.PopModalAsync();
        }
    }
}