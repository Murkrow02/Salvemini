using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using Syncfusion.XForms.PopupLayout;
using Plugin.Iconize;
using Forms9Patch;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.FirstAccess
{
    public partial class Login : ContentPage
    {
        public List<RestApi.Models.Utente> UtentiLogin = new List<RestApi.Models.Utente>();
        

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
            await this.FadeTo(1,300, Easing.CubicIn);

            //PopOvers
            var firstPopUp = new Helpers.PopOvers().defaultPopOver;
            firstPopUp.Content = new Xamarin.Forms.Label { Text = "Utilizza le stesse credenziali" + Environment.NewLine + "che usi nell'app DidUp Famiglia", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center };
            firstPopUp.IsVisible = true;
            firstPopUp.PointerDirection = PointerDirection.Down;
            firstPopUp.PreferredPointerDirection = PointerDirection.Down;
            firstPopUp.Target = usernameEntry;
            firstPopUp.BackgroundColor = Styles.PrimaryColor;
        }

        async void Continue_Clicked(object sender, System.EventArgs e)
        {
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
            passwordEntry.HasError = false;

            //Start Loading
            loading.IsRunning = true;
            loading.IsVisible = true;

            //Create LoginForms
            var form = new RestApi.Models.LoginForm();
            form.Username = username.Text;
            form.Password = password.Text;

            //Api Call
            var Response = await App.Login.Login(form);

            if (!string.IsNullOrEmpty(Response.Message))
            {
                usernameEntry.ErrorText = Response.Message;
                usernameEntry.HasError = true;
                entryStack.Spacing = 8;
            }
            else
            {
                UtentiLogin = Response.Data as List<RestApi.Models.Utente>;
            }

            if (UtentiLogin != null && UtentiLogin.Count > 0)
            {
                //Success
                if (UtentiLogin.Count > 1)
                {
                    popupLayout.PopupView.ContentTemplate = new DataTemplate(() =>

                    {
                        var mainLayout = new Xamarin.Forms.StackLayout { Padding = new Thickness(20, 10) };
                        var firstUserLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal };
                        var firstUserName = new Xamarin.Forms.Label { Text = UtentiLogin[0].Nome, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, HorizontalOptions = LayoutOptions.Start };
                        var firstUserButton = new IconButton { Text = "fas-arrow-right", TextColor = Styles.PrimaryColor, FontSize = 20, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
                        firstUserButton.Clicked += FirstUserButton_Clicked;
                        firstUserLayout.Children.Add(firstUserName);
                        firstUserLayout.Children.Add(firstUserButton);
                        var secondUserLayout = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal };
                        var secondUserName = new Xamarin.Forms.Label { Text = UtentiLogin[1].Nome, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, HorizontalOptions = LayoutOptions.Start };
                        var secondUserButton = new IconButton { Text = "fas-arrow-right", TextColor = Styles.PrimaryColor, FontSize = 20, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
                        secondUserButton.Clicked += SecondUserButton_Clicked;
                        secondUserLayout.Children.Add(firstUserName);
                        secondUserLayout.Children.Add(firstUserButton);
                        var titleLabel = new Xamarin.Forms.Label { Text = "Seleziona un account", TextColor = Styles.TextGray, HorizontalOptions = LayoutOptions.Center };
                        mainLayout.Children.Add(titleLabel);
                        mainLayout.Children.Add(firstUserLayout);
                        mainLayout.Children.Add(secondUserLayout);
                        return mainLayout;
                    });
                    popupLayout.ShowRelativeToView(confirmButton, RelativePosition.AlignTopLeft, App.ScreenWidth / 2, -15);
                }
                else
                {
                    success1();
                }
            }


            //End Loading
            loading.IsRunning = false;
            loading.IsVisible = false;

            //Re-enable button at end of task
            confirmButton.IsEnabled = true;

        }
        void success1()
        {
            Preferences.Set("UserId", UtentiLogin[0].id);
            Preferences.Set("Token", UtentiLogin[0].ArgoToken);
            Preferences.Set("Classe", UtentiLogin[0].Classe);
            Preferences.Set("Corso", UtentiLogin[0].Corso);

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
            success1();
        }

        void SecondUserButton_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("UserId", UtentiLogin[1].id);
            Preferences.Set("Token", UtentiLogin[1].ArgoToken);
            App.refreshCalls();
        }

         void forgotPwd_clicked(object sender, System.EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri("https://www.portaleargo.it/argoweb/famiglia/common/login_form2.jsp"));

            }
            catch
            {
                DisplayAlert("Attenzione", "Non è stato possibile aprire il browser","Ok");
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

    }
}
