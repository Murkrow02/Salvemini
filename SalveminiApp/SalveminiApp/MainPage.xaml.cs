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
        public RestApi.Models.Ad Ad = new RestApi.Models.Ad();
        //Giorni popup
        BubblePopup dayPopOver = new Helpers.PopOvers().defaultPopOver;
        Xamarin.Forms.ListView giorniList = new Xamarin.Forms.ListView { VerticalScrollBarVisibility = ScrollBarVisibility.Never, Footer = "", BackgroundColor = Color.Transparent, SeparatorColor = Color.Gray, WidthRequest = App.ScreenWidth / 4, HeightRequest = App.ScreenHeight / 5 };


        public MainPage()
        {
            InitializeComponent();

            //Initialize interface
            todayLbl.Text = DateTime.Now.ToString("dddd");

            //Create daylist for orario
            giorniList.ItemSelected += GiorniList_ItemSelected;
            giorniList.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new ViewCell();
                var giorno = new Forms9Patch.Label { TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, MaxLines = 1, AutoFit = AutoFit.Width };
                cell.View = giorno;
                giorno.SetBinding(Xamarin.Forms.Label.TextProperty, ".");
                return cell;
            });

            //Get day names list
            var giorni = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            giorni.RemoveAt(0);
            giorniList.ItemsSource = giorni;

        }

        public async Task<string> getNextTrain()
        {
            NextTrain = await App.Treni.GetNextTrain(Preferences.Get("savedStation", 0), Preferences.Get("savedDirection", true));
            string result = "<p>Il prossimo treno per <strong>" + NextTrain.DirectionString + "</strong> partirà alle <strong>"+ NextTrain.Partenza +"</strong> da <strong>"+ NextTrain.Stazione +"</strong></p>";
            return result;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Create static widgets
            var widgets = new List<WidgetGradient>();
            //Trasporti
            widgets.Add(new WidgetGradient { Title = "Trasporti", SubTitle = await getNextTrain(), Icon = "train", StartColor = "A872FF", EndColor = "6F8AFA", Push = new SecondaryViews.BusAndTrains(), Order= Preferences.Get("OrderTrasporti", 0) });
            //Card
            widgets.Add(new WidgetGradient { Title = "SalveminiCard", SubTitle = "Visualizza tutti i vantaggi esclusivi per gli studenti del Salvemini", Icon = "train", StartColor = "B487FD", EndColor = "FA6FFA", Push = new SecondaryViews.SalveminiCard(), Order= Preferences.Get("OrderCard", 0) });
            //Extra
            widgets.Add(new WidgetGradient { Title = "Extra", SubTitle = "Esplora funzioni aggiuntive", Icon = "train", StartColor = "B487FD", EndColor = "FA6FFA",Order = Preferences.Get("OrderExtra", 0) });
            widgetCollection.ItemsSource = widgets.OrderBy(x => x.Order).ToList();


            var notificator = DependencyService.Get<IToastNotificator>();
            if (Preferences.Get("orariTreniVersion", 0) > 0)
            {
                getNextTrain();
            }

            //Get timetables 
            changeDay((int)DateTime.Now.DayOfWeek);

            //Check Internet
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Index = await App.Index.GetIndex();

                //Get last sondaggio
                if(Index.ultimoSondaggio != null)
                {
                    string nuovoSondaggio = "no";
                    int positionSondaggio = Preferences.Get("OrderSondaggio", 0);
                    if (Preferences.Get("LastSondaggio", 0) != Index.ultimoSondaggio.id) //New sondaggio detected
                    {
                        Preferences.Set("LastSondaggio", Index.ultimoSondaggio.id);
                        nuovoSondaggio = "si";
                        positionSondaggio = 0;
                    }
                    //Create sondaggio widget
                    widgets.Add(new WidgetGradient { Title = "Sondaggi", SubTitle = Index.ultimoSondaggio.Nome, Icon = "train", StartColor = "FD8787", EndColor = "F56FFA", Badge = nuovoSondaggio });
                }


                //Get last avviso
                if (Index.ultimoAvviso != null)
                {
                    string nuovoAvviso = "no";
                    int positionAvvisi = Preferences.Get("OrderAvvisi", 0);
                    if (Preferences.Get("LastAvviso", 0) != Index.ultimoAvviso.id) //New avviso detected
                    {
                        Preferences.Set("LastAvviso", Index.ultimoAvviso.id);
                        nuovoAvviso = "si";
                        positionAvvisi = 0;
                    }
                    //Create avviso widget
                    widgets.Add(new WidgetGradient { Title = "Avvisi", SubTitle = Index.ultimoAvviso.Titolo, Icon = "train", StartColor = "FDD487", EndColor = "FA6F6F", Push = new SecondaryViews.Avvisi(), Badge = nuovoAvviso });
                }

                //Get banner ad
                if(Index.Ads.Count > 0)
                {
                    //Find a banner
                    var banner = Index.Ads.Where(x => x.Tipo == 0).ToList();
                    if(banner.Count > 0)
                    {
                        //Found
                        Ad = banner[0];
                        adTitle.Text = Ad.Nome;
                        adImage.Source = Ad.Immagine;
                        adImage.Source = Ad.Immagine;
                        await adBanner.FadeTo(1, 300, Easing.CubicInOut);
                    }
                }

                //Update orari if new version detected
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
                    //Log out and notify argo problem
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


            //Refresh with new widgets
            await widgetCollection.FadeTo(0, 300, Easing.CubicInOut);
            widgetCollection.ItemsSource = null;
            widgetCollection.ItemsSource = widgets.OrderBy(x => x.Order).ToList();
            await widgetCollection.FadeTo(1, 300, Easing.CubicInOut);

        }

        void ad_Tapped(object sender, System.EventArgs e)
        {
           //todo 
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
                if (widget.Push != null)
                    Navigation.PushModalAsync(widget.Push);
            }
            catch (Exception ex)
            {
                //Page not set or some random error, sticazzi
                return;
            }

        }

        void ChangeDay_Clicked(object sender, System.EventArgs e)
        {
            //Create new popover
            dayPopOver = new Helpers.PopOvers().defaultPopOver;
            dayPopOver.Content = giorniList;
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

            //Get current day name
            var giorno = e.SelectedItem as string;

            //Find index from that name
            var giorni = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            var intGiorno = giorni.IndexOf(giorno);

            //Get timetables
            changeDay(intGiorno);


            //Display dayname
            if (intGiorno == (int)DateTime.Now.DayOfWeek)
                orarioDay.Text = "Oggi"; //Lol it's today
            else
                orarioDay.Text = giorno; //Other day

            //Hide popover
            dayPopOver.IsVisible = false;

           
        }



        void editWidget_Tapped(object sender, System.EventArgs e)
        {


        }

        public async void changeDay (int day){
            Orario = await App.Orari.GetOrario("3FCAM", day);
            if (Orario != null)
            {
                orarioList.ItemsSource = Orario;
                double height = 0;
                height = Orario[0].OrarioFrameHeight * Orario.Count + (4 * Orario.Count) ;
                orarioFrame.HeightRequest = height + orarioHeader.Height;

            }

        }

    }
}
