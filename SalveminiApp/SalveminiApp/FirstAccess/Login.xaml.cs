using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.Iconize;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.FirstAccess
{
    public partial class Login : ContentPage
    {
        public List<RestApi.Models.Utente> UtentiLogin = new List<RestApi.Models.Utente>();

        //First tip
        Forms9Patch.BubblePopup firstPopUp = new Helpers.PopOvers().defaultPopOver;


        public Login()
        {
            InitializeComponent();
            this.Opacity = 0;
            //Disable keyboard autocapitalize
            username.Keyboard = Keyboard.Create(KeyboardFlags.None);


            //Set Safe Area
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
#endif
            //Hide navigation bar
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

            //Initialize Interface
            argoLogo.HeightRequest = loginInfoLabel.FontSize;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await this.FadeTo(1, 300, Easing.CubicIn);

            //PopOvers
            firstPopUp.Content = new Xamarin.Forms.Label { Text = "Utilizza le stesse credenziali" + Environment.NewLine + "che usi nell'app DidUp Famiglia", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            firstPopUp.IsVisible = true;
            firstPopUp.PointerDirection = Forms9Patch.PointerDirection.Down;
            firstPopUp.PreferredPointerDirection = Forms9Patch.PointerDirection.Down;
            firstPopUp.Target = usernameEntry;
            firstPopUp.BackgroundColor = Styles.PrimaryColor;
            firstPopUp.BackgroundClicked += FirstPopUp_BackgroundClicked;
            firstPopUp.CloseWhenBackgroundIsClicked = true;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += FirstPopUp_BackgroundClicked;
            firstPopUp.PopAfter = TimeSpan.FromSeconds(3);
            firstPopUp.Content.GestureRecognizers.Add(tapGestureRecognizer);

        }

        private void FirstPopUp_BackgroundClicked(object sender, EventArgs e)
        {
            firstPopUp.IsVisible = false;
        }

        async void Continue_Clicked(object sender, System.EventArgs e)
        {
            //ShowLoading();
            //return;

            //Disable button to avoid reclick
            confirmButton.IsEnabled = false;

            //Username Check
            if (string.IsNullOrEmpty(username.Text))
            {
                usernameEntry.ErrorText = "Inserisci il tuo Username";
                usernameEntry.HasError = true;
                confirmButton.IsEnabled = true;
                entryStack.Spacing = 8;
                return;
            }

            //No username error
            usernameEntry.HasError = false;
            entryStack.Spacing = 0;

            //Password Check
            if (string.IsNullOrEmpty(password.Text))
            {
                passwordEntry.ErrorText = "Inserisci una Password";
                passwordEntry.HasError = true;
                confirmButton.IsEnabled = true;
                return;
            }

            //No password errror
            passwordEntry.HasError = false;

            //Start Loading
            loading.IsRunning = true;
            loading.IsVisible = true;


            //Create LoginForms
            var form = new RestApi.Models.LoginForm();
            form.Username = username.Text;
            form.Password = password.Text;

            //Check internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                //No connection
                Costants.showToast("connection");
                //End Loading
                loading.IsRunning = false;
                loading.IsVisible = false;

                //Re-enable button at end of task
                confirmButton.IsEnabled = true;
                return;
            }

            //Get response from api
            var Response = await App.Login.Login(form);

            //Error from response
            if (!string.IsNullOrEmpty(Response.Message))
            {
                usernameEntry.ErrorText = Response.Message;
                usernameEntry.HasError = true;
                entryStack.Spacing = 8;
            }
            else
            {
                //Success
                UtentiLogin = Response.Data as List<RestApi.Models.Utente>;
            }

            //Error getting users
            if (UtentiLogin == null)
            {
                Costants.showToast("Si è verificato un errore, riprova più tardi");
                //End Loading
                loading.IsRunning = false;
                loading.IsVisible = false;

                //Re-enable button at end of task
                confirmButton.IsEnabled = true;
                return;
            }

            if (UtentiLogin.Count > 0)
            {
                //Success
                if (UtentiLogin.Count > 1)
                {
                    //Pop up for more than one user
                    var popover = new Helpers.PopOvers().defaultPopOver;
                    var content = new StackLayout();

                    //Create Layout
                    content.Children.Add(new Label { Text = "Seleziona un account", HorizontalOptions = LayoutOptions.Center, TextColor = Styles.TextGray });
                    foreach (var utente in UtentiLogin)
                    {
                        var view = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                        view.Children.Add(new Label { Text = utente.Nome, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, HorizontalOptions = LayoutOptions.Start });
                        var GoButton = new IconButton { Text = "fas-arrow-circle-right", TextColor = Styles.PrimaryColor, FontSize = 20, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
                        GoButton.Clicked += (object sender2, EventArgs f) => { success(UtentiLogin.IndexOf(utente)); popover.IsVisible = false; };
                        view.Children.Add(GoButton);
                        content.Children.Add(view);
                    }

                    popover.Content = content;
                    popover.IsVisible = true;
                    popover.PointerDirection = Forms9Patch.PointerDirection.Down;
                    popover.PreferredPointerDirection = Forms9Patch.PointerDirection.Down;
                    popover.Target = sender as Button;
                    popover.BackgroundClicked += (object sender3, EventArgs g) => { popover.IsVisible = false; };
                    popover.CloseWhenBackgroundIsClicked = true;
                }
                else
                {
                    //Only one user
                    success(0);
                }
            }

            //End Loading
            loading.IsRunning = false;
            loading.IsVisible = false;

            //Re-enable button at end of task
            confirmButton.IsEnabled = true;

        }

        async void success(int id)
        {
            //Waat bannato?
            if (UtentiLogin[id].Stato < 0)
            {
                await DisplayAlert("Ueueueue", "Sembrerebbe proprio che il tuo account sia stato disabilitato! Contatta gli sviluppatori se ritieni si tratti di un errore", "Ok");
                return;
            }

            //Save user data
            Preferences.Set("UserId", UtentiLogin[id].id);
            Preferences.Set("Token", UtentiLogin[id].ArgoToken);
            Preferences.Set("Classe", UtentiLogin[id].Classe);
            Preferences.Set("Corso", UtentiLogin[id].Corso);


            if (Preferences.Get("isFirstTime", true))
            {
                Navigation.PushModalAsync(new OrariTrasporti());
            }
            else
            {
                Xamarin.Forms.Application.Current.MainPage = new TabPage();
            }

            App.refreshCalls();
        }
        void Username_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //Remove error when rewriting Username
            if (usernameEntry.HasError && !string.IsNullOrEmpty(e.NewTextValue))
            {
                usernameEntry.HasError = false;
            }
        }

        void Password_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //Remove error when rewriting Password
            if (passwordEntry.HasError && !string.IsNullOrEmpty(e.NewTextValue))
            {
                passwordEntry.HasError = false;
            }
        }

        void FirstUserButton_Clicked(object sender, EventArgs e)
        {
            success(0);
        }

        void SecondUserButton_Clicked(object sender, EventArgs e)
        {
            success(1);
        }

        void forgotPwd_clicked(object sender, System.EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri("https://www.portaleargo.it/argoweb/famiglia/common/login_form2.jsp"));

            }
            catch
            {
                DisplayAlert("Attenzione", "Non è stato possibile aprire il browser", "Ok");
            }
        }

        void help_clicked(object sender, System.EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri("mailto:support@codexdevelopment.net"));
            }
            catch
            {
                DisplayAlert("Attenzione", "Non è stato possibile inviare una mail, puoi inviarla manualmente a support@codexdevelopment.net", "Ok");
            }
        }


        public void ShowLoading()
        {
            confirmButton.Animate("TimeBarAnimation", new Animation(scaleXTo => confirmButton.WidthRequest = scaleXTo, 0, 124), 16, 1000);

        }

    }
}
