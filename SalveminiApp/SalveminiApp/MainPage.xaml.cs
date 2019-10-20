using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using UserNotifications;
using Plugin.Toasts;
using Rg.Plugins.Popup.Extensions;

namespace SalveminiApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public List<RestApi.Models.Lezione> Orario = new List<RestApi.Models.Lezione>();
        public RestApi.Models.Index Index = new RestApi.Models.Index();
        public RestApi.Models.Treno NextTrain = new RestApi.Models.Treno();

        public MainPage()
        {
            InitializeComponent();

            //Initialize interface
            adsFrame.HeightRequest = App.ScreenHeight / 14;
            todayFrame.WidthRequest = App.ScreenWidth / 2.6;
            calendarImage.WidthRequest = todayFrame.WidthRequest / 9;
            trainFrame.WidthRequest = App.ScreenWidth / 2.6;
            trainImage.WidthRequest = todayFrame.WidthRequest / 9;
            adviceImage.WidthRequest = App.ScreenWidth / 22;
            clockImage.WidthRequest = App.ScreenWidth / 22;
            labelView.HeightRequest = App.ScreenHeight / 10;
            tipBottom.TranslationY = App.ScreenHeight / 3;

#if __IOS__
            if (iOS.AppDelegate.HasNotch)
            {
                mainLayout.Spacing = 20;
            }
#endif
        }

        public async void getNextTrain()
        {
            NextTrain = await App.Treni.GetNextTrain(Preferences.Get("savedStation", 0), Preferences.Get("savedDirection", true));

            var nextTrainText = new FormattedString();
            nextTrainText.Spans.Add(new Span { Text = "Il prossimo treno per ", FontAttributes = FontAttributes.None });
            nextTrainText.Spans.Add(new Span { Text = NextTrain.DirectionString, FontAttributes = FontAttributes.Bold });
            nextTrainText.Spans.Add(new Span { Text = " parte alle " + NextTrain.Partenza + " da ", FontAttributes = FontAttributes.None });
            nextTrainText.Spans.Add(new Span { Text = Costants.Stazioni[NextTrain.Stazione], FontAttributes = FontAttributes.Bold });

            nextTrainLabel.FormattedText = nextTrainText;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();


            var uno = new WidgetGradient { Title = "Prova 1", SubTitle = "asdansfoiasnioafssf", Icon = "calendar", StartColor = "FF7272", EndColor = "FACA6F" };
            var due = new WidgetGradient { Title = "Prova 2", SubTitle = "asdansfoiasnioafssf", Icon = "train", StartColor = "A872FF", EndColor = "6F8AFA" };

            var lista = new List<WidgetGradient>();
            lista.Add(uno);
            lista.Add(due);
            widgetCollection.ItemsSource = lista;




            var notificator = DependencyService.Get<IToastNotificator>();
            if (Preferences.Get("orariTreniVersion", 0) > 0)
            {
                getNextTrain();
            }

            //Get timetables
            Orario = await App.Orari.GetOrario("3FCAM", 1);

            if (Orario != null)
            {
                orarioList.ItemsSource = Orario;
                timeTablesDay.Text = Costants.Giorni[Enum.GetName(typeof(DayOfWeek), Orario[0].Giorno)];
            }

            //Check Internet
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Index = await App.Index.GetIndex();

                //Fill Alert
                if (Index.ultimoAvviso != null)
                {
                    //Get Alert Description
                    string stringToDisplay = Index.ultimoAvviso.Descrizione;

                    //Cut string if it is toooo long
                    if (Index.ultimoAvviso.Descrizione.Length > 200)
                    {
                        stringToDisplay = stringToDisplay.Remove(201) + "...";
                    }

                    //Display string
                    avvisiLabel.Text = stringToDisplay;
                }

                //Update orari
                if (Index.OrariTreniVersion > Preferences.Get("orariTreniVersion", 0))
                {
                    bool successTreni = await App.Treni.GetTrainJson();
                    if (successTreni)
                    {
                        Preferences.Set("orariTreniVersion", Index.OrariTreniVersion);
                        getNextTrain();
                    }
                }

                if (!Index.ArgoAuth)
                {
                    //UserDialogs.Instance.Toast(Costants.Toast("Le credenziali sono cambiate", Styles.PrimaryColor, Color.White, ToastPosition.Bottom));
                }
            }
            else
            {
                var options = new NotificationOptions()
                {
                    Description = "Nessuna connessione ad internet 🚀",
                };

                var result = await notificator.Notify(options);
            }

            if (!string.IsNullOrEmpty(avvisiLabel.Text))
            {
                //Alerts' Label starts to flow
                flowLabel();
            }

           
            //Show tip at bottom
            await tipBottom.TranslateTo(0, 0, 1000, Easing.SpringOut);

        }

        void flowLabel()
        {
            var speed = ((double)avvisiLabel.Text.Length / 10) * 1000 + 6000;
            Point pointF = avvisiScroll.GetScrollPositionForElement(avvisiLabel, ScrollToPosition.End);
            pointF.X += App.ScreenWidth * 2;
            var animationF = new Animation(
                callback: x => avvisiScroll.ScrollToAsync(x, pointF.Y, animated: false),
                start: avvisiScroll.ScrollX,
                end: pointF.X);
            avvisiLabel.TranslationX = App.ScreenWidth;

            animationF.Commit(
            owner: this,
            name: "ScrollF",
            length: (uint)speed);

            avvisiScroll.ScrollToAsync(0, 0, false);
            avvisiLabel.TranslationX = App.ScreenWidth;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, (int)speed + 1000), () =>
            {
                animationF.Commit(
            owner: this,
            name: "ScrollF",
            length: (uint)speed);

                avvisiScroll.ScrollToAsync(0, 0, false);
                avvisiLabel.TranslationX = App.ScreenWidth;
                return true;
            });

        }
        void PushToSettings(object sender, System.EventArgs e)
        {
            //Navigation.PushPopupAsync(new SecondaryViews.SettingsPanel());

        }

        void PushToVip(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AreaVip.VipMenu());
        }

        void busTrains_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SecondaryViews.BusAndTrains());
        }

        void Avvisi_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new SecondaryViews.Avvisi());
        }

    }
}
