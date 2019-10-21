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
using Forms9Patch;
using System.Globalization;

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

        //Giorni popup
        BubblePopup dayPopOver = new Helpers.PopOvers().defaultPopOver;
        Xamarin.Forms.ListView giorniList = new Xamarin.Forms.ListView { VerticalScrollBarVisibility = ScrollBarVisibility.Never, Footer = "", BackgroundColor = Color.Transparent, SeparatorColor = Color.Gray, WidthRequest = App.ScreenWidth / 4, HeightRequest = App.ScreenHeight / 5 };


        public MainPage()
        {
            InitializeComponent();

            //Initialize interface
            var uno = new WidgetGradient { Title = "Prova 1", SubTitle = "<p>Il prossimo treno per <strong>Sorrento&nbsp;</strong>partirà alle <strong>10:50</strong> da <strong>Napoli</strong></p>", Icon = "calendar", StartColor = "FF7272", EndColor = "FACA6F" };
            var due = new WidgetGradient { Title = "Prova 2", SubTitle = "asnaosfnoinOI", Icon = "train", StartColor = "A872FF", EndColor = "6F8AFA" };
            var tre = new WidgetGradient { Title = "Prova 2", SubTitle = "asnaosfnoinOI", Icon = "train", StartColor = "000000", EndColor = "ffffff" };

            var lista = new List<WidgetGradient>();
            lista.Add(uno);
            lista.Add(due);
            lista.Add(tre);
            widgetCollection.ItemsSource = lista;
        }

        public async void getNextTrain()
        {
            NextTrain = await App.Treni.GetNextTrain(Preferences.Get("savedStation", 0), Preferences.Get("savedDirection", true));

            var nextTrainText = new FormattedString();
            nextTrainText.Spans.Add(new Span { Text = "Il prossimo treno per ", FontAttributes = FontAttributes.None });
            nextTrainText.Spans.Add(new Span { Text = NextTrain.DirectionString, FontAttributes = FontAttributes.Bold });
            nextTrainText.Spans.Add(new Span { Text = " parte alle " + NextTrain.Partenza + " da ", FontAttributes = FontAttributes.None });
            nextTrainText.Spans.Add(new Span { Text = Costants.Stazioni[NextTrain.Stazione], FontAttributes = FontAttributes.Bold });

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();


           




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
                //timeTablesDay.Text = Costants.Giorni[Enum.GetName(typeof(DayOfWeek), Orario[0].Giorno)];
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
                    //avvisiLabel.Text = stringToDisplay;
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

        void widget_Tapped(object sender, System.EventArgs e)
        {
            try
            {
                //Get push page from model
                var widget = sender as WidgetGradient;

                //Push to selected page
                if(widget.Push != null)
                Navigation.PushAsync(widget.Push);
            }
            catch(Exception ex)
            {
                //Page not set or some random error, sticazzi
                return;
            }
            
        }

        void ChangeDay_Clicked(object sender, System.EventArgs e)
        {
            //Create layout
            var stack = new Xamarin.Forms.StackLayout();
            giorniList.ItemSelected += GiorniList_ItemSelected;
            giorniList.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new ViewCell();
                var giorno = new Forms9Patch.Label { TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, MaxLines=1,AutoFit = AutoFit.Width };
                cell.View = giorno;
                giorno.SetBinding(Xamarin.Forms.Label.TextProperty, ".");
                return cell;
            });

            var giorni = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            giorni.RemoveAt(0);
            giorniList.ItemsSource = giorni;
            stack.Children.Add(giorniList);


            //Create popover
            dayPopOver.Content = stack;
            dayPopOver.PointerDirection = PointerDirection.Up;
            dayPopOver.PreferredPointerDirection = PointerDirection.Up;
            dayPopOver.Target = arrowDown;
            dayPopOver.BackgroundColor = Color.FromHex("202020");
            dayPopOver.IsVisible = true;
        }

        private async void GiorniList_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect Animation
            if (e.SelectedItem == null)
                return;

            giorniList.SelectedItem = null;
            var giorno = e.SelectedItem as string;

            var giorni = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            var intGiorno = giorni.IndexOf(giorno);

            //Get timetables
            Orario = await App.Orari.GetOrario("3FCAM", intGiorno);

            if (Orario != null)
            {
                orarioList.ItemsSource = Orario;
            }

            dayPopOver.IsVisible = false;
        }

    }
}
