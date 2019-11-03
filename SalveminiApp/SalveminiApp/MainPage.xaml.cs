using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Forms9Patch;
using System.Globalization;
using P42.Utils;
using MonkeyCache.SQLite;
using FFImageLoading;
using FFImageLoading.Cache;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
namespace SalveminiApp
{
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        //Full orario list, all days
        public List<RestApi.Models.Lezione> Orario = new List<RestApi.Models.Lezione>();
        //Index informations, such as app version, avvisi, sondaggi
        public RestApi.Models.Index Index = new RestApi.Models.Index();
        //Argo index, how many notizie etc.
        public RestApi.Models.IndexArgo ArgoIndex = null;
        //Ad to be shown
        public RestApi.Models.Ad Ad = new RestApi.Models.Ad();
        //List of widgets to be shown
        List<WidgetGradient> widgets = new List<WidgetGradient>();
        //User's class and course e.g. 4FCAM
        string classeCorso;
        //Giorni popover
        BubblePopup dayPopOver = new Helpers.PopOvers().defaultPopOver;
        Xamarin.Forms.ListView giorniList = new Xamarin.Forms.ListView { VerticalScrollBarVisibility = ScrollBarVisibility.Never, Footer = "", BackgroundColor = Color.Transparent, SeparatorColor = Color.Gray, WidthRequest = App.ScreenWidth / 4, HeightRequest = App.ScreenHeight / 5 };
        //Fix close modal bug
        public static bool isSelectingImage;

        public MainPage()
        {
            InitializeComponent();

            //Set profile image size
            userImg.WidthRequest = App.ScreenWidth / 8.8;

            //Set navigation view
            todayLbl.Text = DateTime.Now.ToString("dddd").FirstCharToUpper();

            //Create daylist for orario popover
            giorniList.ItemSelected += GiorniList_ItemSelected;
            giorniList.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new ViewCell();
                var giorno = new Forms9Patch.Label { TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, MaxLines = 1, AutoFit = AutoFit.Width };
                cell.View = giorno;
                giorno.SetBinding(Xamarin.Forms.Label.TextProperty, ".");
                return cell;
            });



            //Subscribe to messaging center
            //Refresh image cache
            MessagingCenter.Subscribe<App>(this, "ReloadUserPic", (sender) =>
            {
                ImageService.Instance.InvalidateCacheEntryAsync(Costants.Uri("images/users/") + Preferences.Get("UserId", ""), CacheType.All, removeSimilar: true);
                userImg.Source = "";
                userImg.Source = Costants.Uri("images/users/") + Preferences.Get("UserId", "");
                userImg.ReloadImage();
                userImg.WidthRequest = App.ScreenWidth / 8.8;
            });

            //Remove avvisi badge
            MessagingCenter.Subscribe<App, string>(this, "RemoveBadge", (sender, tipo) =>
            {
                RemoveBadge(tipo);
            });

            //Fill initial cache
            if (Barrel.Current.Exists("Index"))
            {
                try
                {
                    var tempIndex = Barrel.Current.Get<RestApi.Models.Index>("Index");
                    Index = tempIndex;
                    //Get banner ad
                    if (tempIndex != null && tempIndex.Ads != null && tempIndex.Ads.Count > 0)
                    {
                        //Find a banner
                        var banner = tempIndex.Ads.Where(x => x.Tipo == 0).ToList();
                        if (banner.Count > 0)
                        {
                            //Found
                            Ad = banner[0];
                            adTitle.Text = Ad.Nome;
                            adImage.Source = Ad.FullImmagine;
                            adLayout.Opacity = 1;
                        }
                    }
                }
                catch
                {
                    //Error getting cache, delete it
                    Barrel.Current.Empty("Index");
                }

            }
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Security checks todo

            //Set image profile url
            userImg.Source = Costants.Uri("images/users/") + Preferences.Get("UserId", "");

            //Remove modals bug
            ModalPush_Disappearing(null, null);

            //Get cached orario
            classeCorso = Preferences.Get("Classe", 0) + Preferences.Get("Corso", "");
            if (Barrel.Current.Exists("orario" + classeCorso))
            {
                Orario = Barrel.Current.Get<List<RestApi.Models.Lezione>>("orario" + classeCorso);
                changeDay(-1);
            }

            //Create static widgets
            widgets.Clear();
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += widget_Tapped;
            //Registro
            string notizia = "Cosa è successo oggi in classe?";
            if (ArgoIndex != null)
                notizia = "<strong>" + ArgoIndex.TipoNotizia + "</strong>" + ArgoIndex.UltimaNotizia;
            var registro = new WidgetGradient { Title = "Registro", SubTitle = notizia, Icon = "far-calendar-alt", StartColor = "FACA6F", EndColor = "FF7272", Push = new ArgoPages.Registro(), Order = 1 };
            registro.GestureRecognizers.Add(tapGestureRecognizer);
            //Trasporti
            var trasporti = new WidgetGradient { Title = "Trasporti", SubTitle = await getNextTrain(), Icon = "fas-subway", StartColor = "A872FF", EndColor = "6F8AFA", Push = new SecondaryViews.BusAndTrains(), Order = 2 };
            trasporti.GestureRecognizers.Add(tapGestureRecognizer);
            //Card
            var card = new WidgetGradient { Title = "SalveminiCard", SubTitle = "Visualizza tutti i vantaggi esclusivi per gli studenti del Salvemini", Icon = "fas-credit-card", StartColor = "B487FD", EndColor = "FA6FFA", Push = new SecondaryViews.SalveminiCard(), Order = 6 };
            card.GestureRecognizers.Add(tapGestureRecognizer);
            //Extra
            var extra = new WidgetGradient { Title = "Extra", SubTitle = "Esplora funzioni aggiuntive", Icon = "fas-star", StartColor = "B487FD", EndColor = "FA6FFA", Order = 5, Push = new SecondaryViews.Extra() };
            extra.GestureRecognizers.Add(tapGestureRecognizer);

            //Initialize list with first widgets
            widgets.Add(registro); widgets.Add(trasporti); widgets.Add(card); widgets.Add(extra);
            OrderWidgets(false);

            //Check train version
            if (Preferences.Get("orariTreniVersion", 0) > 0)
            {
                getNextTrain();
            }

            //Check Internet
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Argo index
                GetArgoIndex();

                //Get updated orario
                var _orario = await App.Orari.GetOrario(classeCorso);

                //Check if success and if there are updates
                if (_orario != null && Orario != _orario)
                {
                    //Update with new orario
                    Orario = _orario;
                    changeDay(-1);
                }

                var tempIndex = await App.Index.GetIndex();

                //Checks
                //Authorized? todo
                if (tempIndex != null)
                {
                    //Save new index
                    Index = tempIndex;

                    switch (Index.Authorized)
                    {
                        case -1: //Banned
                            await DisplayAlert("Accesso all'app non autorizzato", "Ooops, tuo account è stato disabilitato! Contatta gli sviluppatori se ritieni si tratti di un errore!", "Ok");
                            Costants.Logout();
                            break;
                        case -2: //Argo unauthorized
                            await DisplayAlert("Accesso all'app non autorizzato", "La sessione di ARGO è scaduta, probabilmente la password è stata cambiata, rieffettua l'accesso per continuare", "Ok");
                            Costants.Logout();
                            break;
                    }
                }
                else
                {
                    //todo 
                    await DisplayAlert("Attenzione", "Non è stato possibile connettersi al server", "Ok");

                    //Anche la cache è nulla, stacca stacca
                    if (Index != null)
                        return;
                }


                //Get last sondaggio
                if (Index.ultimoSondaggio != null)
                {
                    string nuovoSondaggio = "no";
                    int positionSondaggio = 4;
                    if (Preferences.Get("LastSondaggio", 0) != Index.ultimoSondaggio.id && !Index.VotedSondaggio) //New sondaggio detected
                    {
                        //Preferences.Set("LastSondaggio", Index.ultimoSondaggio.id);
                        nuovoSondaggio = "si";
                        positionSondaggio = -1;


                        if (!Preferences.Get("voted" + Index.ultimoSondaggio.id, false)) //Push to vote
                            await Navigation.PushModalAsync(new SecondaryViews.NewSondaggio(Index.ultimoSondaggio));
                    }



                    //Create sondaggio widget
                    var sondaggi = new WidgetGradient { Order = positionSondaggio, Title = "Sondaggi", SubTitle = Index.ultimoSondaggio.Nome.Truncate(50), Icon = "fas-poll", StartColor = "FD8787", EndColor = "F56FFA", Badge = nuovoSondaggio, Push = new SecondaryViews.NewSondaggio(Index.ultimoSondaggio) };
                    sondaggi.GestureRecognizers.Add(tapGestureRecognizer);
                    widgets.Add(sondaggi);

                }


                //Get last avviso
                if (Index.ultimoAvviso != null)
                {
                    string nuovoAvviso = "no";
                    int positionAvvisi = 3;
                    if (Preferences.Get("LastAvviso", 0) != Index.ultimoAvviso.id) //New avviso detected
                    {
                        nuovoAvviso = "si";
                        positionAvvisi = -2;
                    }
                    //Create avviso widget
                    var avvisi = new WidgetGradient { Order = positionAvvisi, Title = "Avvisi", SubTitle = Index.ultimoAvviso.Titolo, Icon = "fas-exclamation-triangle", StartColor = "FDD487", EndColor = "FA6F6F", Push = new SecondaryViews.Avvisi(), Badge = nuovoAvviso };
                    avvisi.GestureRecognizers.Add(tapGestureRecognizer);
                    widgets.Add(avvisi);
                }

                //Get banner ad
                if (Index.Ads != null && Index.Ads.Count > 0)
                {
                    //Find a banner
                    var banner = Index.Ads.Where(x => x.Tipo == 0).ToList();
                    if (banner.Count > 0)
                    {
                        //Found
                        Ad = banner[0];
                        adTitle.Text = Ad.Nome;
                        adImage.Source = Ad.FullImmagine;
                        await adLayout.FadeTo(1, 300, Easing.CubicInOut);
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

            }
            else
            {
                //todo notify no internet
            }

            //Update widgets order
            OrderWidgets(true);

        }

        //Handle widget redirections
        void widget_Tapped(object sender, System.EventArgs e)
        {
            try
            {
                //Get push page from model
                var widget = sender as WidgetGradient;

                //Push to selected page
                if (widget.Push != null)
                {
                    if (widget.Title == "Trasporti")
                    {
                        Navigation.PushAsync(widget.Push); //Push
                    }
                    else
                    {
                        //Modal figo
#if __IOS__
                        widget.Push.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
#endif
                        Navigation.PushModalAsync(widget.Push); //Modal
                    }

                }
            }
            catch (Exception ex)
            {
                //Page not set or some random error, sticazzi
                return;
            }
        }


        //Get string for train widget
        public async Task<string> getNextTrain()
        {
            var NextTrain = await App.Treni.GetNextTrain(Preferences.Get("savedStation", 0), Preferences.Get("savedDirection", true));
            string result = "<p>Il prossimo treno per <strong>" + NextTrain.DirectionString + "</strong> partirà alle <strong>" + NextTrain.Partenza + "</strong> da <strong>" + Costants.Stazioni[NextTrain.Stazione] + "</strong></p>";
            return result;
        }

        //Show popover with daylist
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

        //Get argo index
        async void GetArgoIndex()
        {
            //Get argo index
            var ArgoIndex_ = await App.Index.GetIndexArgo();


            if (ArgoIndex_ != null && ArgoIndex_.NotizieCount > 0)
            {
                ArgoIndex = ArgoIndex_;
                try
                {
                    //Get registro widget
                    var widget = widgets.First(x => x.Title == "Registro");
                    var dataUltimoControllo = Preferences.Get("lastDate", "02/05/2002");
                    var notizieUltimoControllo = Preferences.Get("lastNotizie", 0);
                    var dataOggi = DateTime.Now.ToString("dd/MM-yyyy");

                    //same day, same notizie count
                    if (dataUltimoControllo == dataOggi && ArgoIndex.NotizieCount <= notizieUltimoControllo)
                    {
                        return;
                    }

                    //New day
                    if (dataUltimoControllo != dataOggi)
                    {
                        //Save new date
                        Preferences.Set("lastDate", DateTime.Now.ToString("dd/MM/yyyy"));
                    }

                    //Notizie > notizie saved
                    Preferences.Set("lastNotizie", ArgoIndex.NotizieCount);
                    widget.Badge = "si";
                    widget.SubTitle = "<strong>" + ArgoIndex.TipoNotizia + "</strong>" + ArgoIndex.UltimaNotizia;
                    widgets.Remove(widget);
                    widgets.Add(widget);
                    OrderWidgets(true);
                }
                catch
                {
                    //Fa nient
                }
            }
        }

        //Selected day from popover
        private void GiorniList_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Deselect Animation
            if (e.SelectedItem == null)
                return;
            giorniList.SelectedItem = null;

            //Get current day name
            var giorno = e.SelectedItem as string;

            //Find index from that name
            var giorni = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            giorni = giorni.ConvertAll(x => x.ToLower());
            var intGiorno = giorni.IndexOf(giorno.ToLower());

            //Get timetables
            changeDay(intGiorno);

            //Hide popover
            dayPopOver.IsVisible = false;
        }


        //Push to profile page
        void profilePush(object sender, System.EventArgs e)
        {
            //Create new navigation page
            var modalPush = new Xamarin.Forms.NavigationPage(new SecondaryViews.Profile());

            //Add disappearing event
            modalPush.Disappearing += ModalPush_Disappearing;

            //Add toolbaritem to close page
            var close = new ToolbarItem { Text = "Annulla" };
            close.Clicked += ModalPush_Disappearing;
            modalPush.ToolbarItems.Add(close);

            //Modal figo
#if __IOS__
            modalPush.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
#endif
            modalPush.BarTextColor = Styles.TextColor;
            Navigation.PushModalAsync(modalPush);
        }

        //Fix bug on ios 13 that doesen t close modal automatically
        private void ModalPush_Disappearing(object sender, EventArgs e)
        {
            try
            {
                if (!isSelectingImage)
                    Navigation.PopModalAsync();
                else
                    isSelectingImage = false;
            }
            catch
            {
                //fa nient
            }
        }

        //todo edit widget order
        void editWidget_Tapped(object sender, System.EventArgs e)
        {
        }

        //Update orario list (-1 = today)
        public async void changeDay(int day)
        {
            bool today = day == -1; //Today is selected

            if (today)
                day = (int)DateTime.Now.DayOfWeek; //Reset current day

            int daySkipped = 0;

            try
            {
                if (Orario != null)
                {
                    //Detect freeday
                    var freedayInt = Orario.FirstOrDefault(x => x.Materia == "Libero").Giorno;
                    var allDays = Costants.getDays();

                    //Remove freeday from list
                    allDays.RemoveAt(freedayInt - 1);
                    giorniList.ItemsSource = allDays;

                    //intelligent auto skip if dopo le 2
                    if (today && DateTime.Now.Hour > 14)
                    {
                        daySkipped++;
                        day = SkipDay(day);
                    }

                    //Detect Sunday
                    if (day == 0)
                    {
                        day++;
                        daySkipped++;
                    }

                    //Skip freeday
                    if (day == freedayInt)
                    {
                        daySkipped++;
                        day = SkipDay(day);
                    }

                    //get only today lessons
                    var orarioOggi = await App.Orari.GetOrarioDay(day, Orario);

                    //Set day label
                    if (day == (int)DateTime.Now.DayOfWeek)
                    {
                        orarioDay.Text = "Oggi";
                    }
                    else if (daySkipped == 1 && today)
                    {
                        orarioDay.Text = "Domani";
                    }
                    else
                    {
                        orarioDay.Text = Costants.getDays()[day - 1].FirstCharToUpper(); //Other day
                    }

                    //Set sede label
                    try
                    {
                        sedeLbl.Text = orarioOggi[0].Sede;
                    }
                    catch
                    {
                        //Fa niente
                    }


                    //Fill list with lezioni
                    orarioList.ItemsSource = orarioOggi;

                    //Calc orario dimensions
                    orarioList.HeightRequest = orarioOggi[0].OrarioFrameHeight * orarioOggi.Sum(x => x.numOre) + (4 * orarioOggi.Count);
                    orarioFrame.HeightRequest = orarioList.HeightRequest + orarioHeader.Height;

                    //Set max orario height
                    if (orarioFrame.HeightRequest > fullLayout.Height * 0.4)
                    {
                        orarioFrame.HeightRequest = fullLayout.Height * 0.4;
                    }

                    //Show orario
                    orarioFrame.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Errore", "Non è stato possibile recuperare l'orario, contattaci se il problema persiste", "Ok");
                    orarioList.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                orarioList.IsVisible = false;
                await DisplayAlert("Errore", "Non è stato possibile recuperare l'orario, contattaci se il problema persiste", "Ok");
            }

        }

        //Update widget list
        public async void OrderWidgets(bool animate)
        {
            //Refresh with new widgets
            //Fade animation
            if (animate)
                await widgetCollection.FadeTo(0, 300, Easing.CubicInOut);

            //Remove previous widgets
            widgetsLayout.Children.Clear();
            //Add initial space
            widgetsLayout.Children.Add(new Xamarin.Forms.ContentView { WidthRequest = 5 });
            //Order by user order
            widgets = widgets.OrderBy(x => x.Order).ToList();
            //Add widgets to list
            widgetsLayout.Children.AddRange(widgets);
            //Add final space
            widgetsLayout.Children.Add(new Xamarin.Forms.ContentView { WidthRequest = 5 });
            //Fade animation
            if (animate)
                await widgetCollection.FadeTo(1, 300, Easing.CubicInOut);
        }

        //Handle ad tapped to show info
        void ad_Tapped(object sender, System.EventArgs e)
        {
            if (Ad != null)
            {
                //URL
                if (string.IsNullOrEmpty(Ad.Descrizione) && !string.IsNullOrEmpty(Ad.Url))
                {
                    try
                    {
                        //Apri browser
                        Device.OpenUri(new Uri(Ad.Url));
                    }
                    catch
                    {
                        //Fa niente
                    }
                }

                //DESCRIZIONE
                if (!string.IsNullOrEmpty(Ad.Descrizione) && string.IsNullOrEmpty(Ad.Url))
                {
                    DisplayAlert(Ad.Nome, Ad.Descrizione, "Chiudi");
                }
            }
        }

        public void RemoveBadge(string tipo)
        {
            try
            {
                var widget = widgets.First(x => x.Title == tipo);
                widgets.Remove(widget);
                widget.Badge = "no";
                widgets.Add(widget);
                OrderWidgets(true);
            }
            catch
            {
                //Fa nient
            }

        }

        public int SkipDay(int day)
        {
            int newDay = day;
            newDay++;
            if (newDay == 7)
                return 1;
            else
                return newDay;
        }

    }

    //Add range to iList
    public static class IListExtension
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException("list");
            if (items == null) throw new ArgumentNullException("items");

            if (list is List<T>)
            {
                ((List<T>)list).AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }
    }

    //Upper only first letter
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

    }
}
