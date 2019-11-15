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
using System.Diagnostics;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
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
        //Do not update orario if already cached
        public bool orarioFromCached;
        //How many times the page loaded onAppearing
        public int appearedTimes;

        public MainPage()
        {
            InitializeComponent();

            //Set profile image size
            userImg.WidthRequest = App.ScreenWidth / 8.8;
            coinImage.WidthRequest = App.ScreenWidth / 13;

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
                try
                {
                    ImageService.Instance.InvalidateCacheEntryAsync(Costants.Uri("images/users/") + Preferences.Get("UserId", ""), CacheType.All, removeSimilar: true);
                    userImg.Source = "";
                    userImg.Source = Costants.Uri("images/users/") + Preferences.Get("UserId", "");
                    userImg.ReloadImage();
                    userImg.WidthRequest = App.ScreenWidth / 8.8;
                }
                catch
                {

                }
            });

            //Remove avvisi badge
            MessagingCenter.Subscribe<App, string>(this, "RemoveBadge", (sender, tipo) =>
            {
                RemoveBadge(tipo);
            });



            ////Get orario cached
            //var orarioCached = CacheHelper.GetCache<List<RestApi.Models.Lezione>>("orario" + Preferences.Get("Classe", 0) + Preferences.Get("Corso", ""));
            //if (orarioCached != null)
            //{
            //    Orario = orarioCached;
            //    changeDay(-1);
            //}



            //Get index cache
            var IndexCache = CacheHelper.GetCache<RestApi.Models.Index>("Index");

            //Failed to get
            if (IndexCache == null)
                return;

            //SalveminiCoin
            sCoinLbl.Text = IndexCache.sCoin.ToString();

            //Get banner cache
            if (IndexCache.Ads != null && IndexCache.Ads.Count > 0)
            {
                //Find a banner
                var banner = IndexCache.Ads.Where(x => x.Tipo == 0).ToList();
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



        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Increment number of appeared times
            appearedTimes++;

            //Show loading
            widgetLoading.IsRunning = true;
            widgetLoading.IsVisible = true;

            //Sempre meglio mettere il try lol
            try
            {

                //Set image profile url
                userImg.Source = Costants.Uri("images/users/") + Preferences.Get("UserId", 0).ToString();

                //Remove modals bug
                ModalPush_Disappearing(null, null);

                //Get classe e corso
                classeCorso = Preferences.Get("Classe", 0) + Preferences.Get("Corso", "");

                //Get cached orario
                Orario = CacheHelper.GetCache<List<RestApi.Models.Lezione>>("orario" + classeCorso);
                if (!orarioFromCached && Orario != null) //already got today from initialize component
                {
                    changeDay(-1);
                    orarioFromCached = false;
                }

                //Create static widgets
                widgets.Clear();
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += widget_Tapped;

                //Registro
                var registro = new WidgetGradient { Title = "Registro", SubTitle = lastArgoString(), Icon = "far-calendar-alt", StartColor = "FACA6F", EndColor = "FF7272", Push = new ArgoPages.Registro(), Order = 1 };
                registro.GestureRecognizers.Add(tapGestureRecognizer);

                //Trasporti
                if (Preferences.Get("OrarioTrasportiVersion", 0) > 0) //Add this only if orario is downloaded
                {
                    var trasporti = new WidgetGradient { Title = "Trasporti", SubTitle = await getNextTrain(), Icon = "fas-subway", StartColor = "A872FF", EndColor = "6F8AFA", Push = new SecondaryViews.BusAndTrains(), Order = 3 };
                    trasporti.GestureRecognizers.Add(tapGestureRecognizer);
                    widgets.Add(trasporti);
                }
                //Card
                var card = new WidgetGradient { Title = "SalveminiCard", SubTitle = "Visualizza tutti i vantaggi esclusivi per gli studenti del Salvemini", Icon = "fas-credit-card", StartColor = "B487FD", EndColor = "FA6FFA", Push = new SecondaryViews.SalveminiCard(), Order = 6 };
                card.GestureRecognizers.Add(tapGestureRecognizer);
                //Extra
                var extra = new WidgetGradient { Title = "Extra", SubTitle = "Esplora funzioni aggiuntive", Icon = "fas-star", StartColor = "B487FD", EndColor = "FA6FFA", Order = 5, Push = new SecondaryViews.Extra() };
                extra.GestureRecognizers.Add(tapGestureRecognizer);

                //Initialize list with first widgets
                widgets.Add(registro); widgets.Add(card); widgets.Add(extra);
                OrderWidgets(false); //uncomment to fast load initial widgets


                //Check Internet
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {

                    //Argo index in background
                    await Task.Run((Action)GetArgoIndex);

                    //Update orario in background
                    await Task.Run((Action)updateOrario);



                    var secondo = new Stopwatch();secondo.Start();
                    //Get index from api call
                    var tempIndex = await App.Index.GetIndex();

    

                    //Checks in downloaded index
                    if (tempIndex != null)
                    {
                        //Save new index
                        Index = tempIndex;

                        //Can use the app?
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
                        //Chiamata api fallita
                        Costants.showToast("Non è stato possibile connettersi al server");

                        //Anche la cache è nulla, stacca stacca
                        if (Index != null)
                        {
                            widgetLoading.IsRunning = false;
                            widgetLoading.IsVisible = false;
                            return;
                        }
                    }

                    //Save salveminiCoin value
                    sCoinLbl.Text = Index.sCoin.ToString();

                    //Get last sondaggio
                    if (Index.ultimoSondaggio != null) //New sondaggio detected
                    {
                        string nuovoSondaggio = "no";
                        int positionSondaggio = 4;
                        if (!Index.VotedSondaggio) //User did not vote
                        {
                            //Save that user has not voted
                            Preferences.Set("voted" + Index.ultimoSondaggio.id, false);

                            //Set badges on widget
                            nuovoSondaggio = "si";
                            positionSondaggio = -1;

                            //If user decided not to vote don't show popup
                            if (!Preferences.Get("skipPoll" + Index.ultimoSondaggio.id, false)) //Push to vote
                                await Navigation.PushModalAsync(new SecondaryViews.NewSondaggio(Index.ultimoSondaggio));
                        }
                        else
                        {
                            //save that he voted
                            Preferences.Set("voted" + Index.ultimoSondaggio.id, true);
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
                        int positionAvvisi = 2;
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
                    if (Index.OrarioTrasportiVersion > Preferences.Get("OrarioTrasportiVersion", 0))
                    {
                        bool successTreni = await App.Treni.GetTrainJson();
                        if (successTreni)
                        {
                            Preferences.Set("OrarioTrasportiVersion", Index.OrarioTrasportiVersion);
                            await getNextTrain();
                        }
                    }

                    //Check app version
                    var appversion = Convert.ToDecimal(VersionTracking.CurrentVersion);
                    if (Index.AppVersion > appversion)
                        newVersion(); //New version detected

                }
                else
                {
                    //Nessuna connessione
                    Costants.showToast("connection");
                }

                //Update widgets order
                OrderWidgets(false);
                widgetLoading.IsRunning = false;
                widgetLoading.IsVisible = false;
            }
            catch (Exception ex) //Errore sconosciuto :0
            {
                widgetLoading.IsRunning = false;
                widgetLoading.IsVisible = false;
                Costants.showToast("Si è verificato un errore fatale, contatta gli sviluppatori se il problema persiste");
            }

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
                        widget.Push.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
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
                    var dataOggi = DateTime.Now.ToString("dd/MM/yyyy");

                    //same day, same notizie count
                    if (dataUltimoControllo == dataOggi && ArgoIndex.NotizieCount <= notizieUltimoControllo)
                    {
                        return;
                    }

                    //New day
                    if (dataUltimoControllo != dataOggi)
                    {
                        //Save new date
                        Preferences.Remove("lastTipoNotizia"); //Cached last title
                        Preferences.Remove("lastNotizia"); //Cached last subtitle
                        Preferences.Set("lastDate", DateTime.Now.ToString("dd/MM/yyyy"));
                    }

                    //Notizie > notizie saved
                    Preferences.Set("lastNotizie", ArgoIndex.NotizieCount); //Notizie count
                    Preferences.Set("lastTipoNotizia", ArgoIndex.TipoNotizia); //Cached last title
                    Preferences.Set("lastNotizia", ArgoIndex.UltimaNotizia); //Cached last subtitle
                    widget.Badge = "si";
                    widget.Order = -2;
                    widget.SubTitle = ("<strong>" + ArgoIndex.TipoNotizia + "</strong>" + ArgoIndex.UltimaNotizia).Truncate(70);
                    widgets.Remove(widget);
                    widgets.Add(widget);
                    //Order widgets on main thread to prevent crashes
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        OrderWidgets(true);
                    });
                }
                catch
                {
                    //Non è stato possibile connettersi al registro
                    Costants.showToast("Impossibile connettersi ad ARGO");
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
            var giorni = Costants.getDays();
            giorni = giorni.ConvertAll(x => x.ToLower());
            var intGiorno = giorni.IndexOf(giorno.ToLower()) + 1;

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
            modalPush.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
            modalPush.BarTextColor = Styles.TextColor;
            Navigation.PushModalAsync(modalPush);
        }

        //Fix bug on ios 13 that doesen t close modal automatically
        private void ModalPush_Disappearing(object sender, EventArgs e)
        {
            try
            {
#if __IOS__
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
#endif
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

                    //Save freeday
                    Preferences.Set("FreedayInt", freedayInt);



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
                    if (day == freedayInt + 1)
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

                    showOrario(orarioOggi, freedayInt);
                }
                else
                {
                    await DisplayAlert("Errore", "Non è stato possibile recuperare l'orario, contattaci se il problema persiste", "Ok");
                    materieLayout.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                materieLayout.IsVisible = false;
                await DisplayAlert("Errore", "Non è stato possibile recuperare l'orario, contattaci se il problema persiste", "Ok");
            }

        }


        public void showOrario(List<RestApi.Models.Lezione> orario, int freeday)
        {
            //Orario is not empty
            emptyLayout.IsVisible = false;

            try
            {
                //Remove freeday from list
                var allDays = Costants.getDays();
                allDays.RemoveAt(freeday - 1);
                giorniList.ItemsSource = allDays;

                //Fill list with lezioni
                materieLayout.Children.Clear();
                foreach (var lezione in orario)
                {
                    //Horizontal stack layout
                    var stack = new Xamarin.Forms.StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0 };
                    //Ora
                    var oraLbl = new Xamarin.Forms.Label { FontSize = 10, HorizontalOptions = LayoutOptions.Start, Text = lezione.oraLezione, TextColor = Styles.TextGray, VerticalOptions = LayoutOptions.Start };
                    //Materia
                    var materiaFrame = new Xamarin.Forms.Frame { HasShadow = false, BackgroundColor = Color.FromHex(lezione.Colore), CornerRadius = lezione.OrarioFrameRadius, Margin = lezione.OrarioFrameMargin, Padding = new Thickness(10, 0), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = lezione.OrarioFrameHeight };
                    materiaFrame.Content = new Xamarin.Forms.Label { TextColor = Color.White, Text = lezione.Materia, VerticalOptions = LayoutOptions.Center };

                    //Add to final layout
                    stack.Children.Add(oraLbl); stack.Children.Add(materiaFrame);
                    materieLayout.Children.Add(stack);
                }



                //Show orario
                orarioFrame.IsVisible = true;
            }
            catch
            {
                //From constructor, get full orario to fix this
                orarioFromCached = false;
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




        //returns the last news from argo or the default if there are not
        public string lastArgoString()
        {
            var dataUltimoControllo = Preferences.Get("lastDate", "02/05/2002");
            var dataOggi = DateTime.Now.ToString("dd/MM/yyyy");

            //He saved a string today
            if (dataUltimoControllo == dataOggi)
                return ("<strong>" + Preferences.Get("lastTipoNotizia", "") + "</strong>" + Preferences.Get("lastNotizia", "Cosa è successo oggi in classe?")).Truncate(70);

            return "Cosa è successo oggi in classe?";
        }

        public async void newVersion()
        {
            //Check App Version
            var appversion = Convert.ToDecimal(VersionTracking.CurrentVersion);
            if (Index.AppVersion > appversion && appearedTimes % 2 == 0) //Show this only 1/2
            {
                bool choice = await DisplayAlert("Buone notizie!", "È disponibile un aggiornamento dell'app, recati sullo store per effettuarlo!", "Aggiorna", "Chiudi");
                if (choice)
                {
                    //Redirect to store
#if __ANDROID__

#endif
#if __IOS__

#endif


                }
            }
        }


        public async void updateOrario()
        {
            //Download orario
            var data = await App.Orari.GetOrario(classeCorso);

            //Failed to get
            if (data.Message != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Costants.showToast(data.Message);

                    if (data.Message == "L'orario della classe non è stato trovato")
                        emptyLayout.IsVisible = true;
                });
            }
            else
            {
                //Delete last color palette
                Costants.ClearColors();

                var _orario = data.Data as List<RestApi.Models.Lezione>;

                //Check if success and if there are updates
                if (_orario != null && Orario != _orario && !orarioFromCached)
                {
                    //Update with new orario
                    Orario = _orario;

                    //Use main thread to prevent crashes
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        changeDay(-1);
                    });
                }
            }
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
                foreach (var item in items.ToList())
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
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

    }
}
