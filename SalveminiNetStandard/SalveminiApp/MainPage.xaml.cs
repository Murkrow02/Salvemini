﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace SalveminiApp
{
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {

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
        //Fix close modal bug
        public static bool isSelectingImage;
        //Do not update orario if already cached
        public bool orarioFromCached;
        //How many times the page loaded onAppearing
        public static int appearedTimes;
        //Page loaded from notification tapped
        public static Xamarin.Forms.Page NotificationPage = null;
        //Load navigation pages to be faster
        public Xamarin.Forms.NavigationPage profilePage = null;
        public Helpers.CustomNavigationPage scoinPage = null;
        public Xamarin.Forms.NavigationPage extraPage = null;

        public MainPage()
        {
            InitializeComponent();


            //Set sizes
            //userImg.WidthRequest = App.ScreenWidth / 8.8;
            //coinImage.WidthRequest = App.ScreenWidth / 13;

            //Subscribe to messaging center
            //Refresh image cache
            MessagingCenter.Subscribe<App, string>(this, "ReloadUserPic", (sender, arg) =>
             {
                 userImg.Source = "";
                 userImg.Source = arg;
                 userImg.ReloadImage();
                 userImg.WidthRequest = App.ScreenWidth / 8.8;
             });

            //Remove avvisi badge
            MessagingCenter.Subscribe<App, string>(this, "RemoveBadge", (sender, tipo) =>
            {
                RemoveBadge(tipo);
            });

            //Remove avvisi badge
            MessagingCenter.Subscribe<App, int?>(this, "UpdateCoins", (sender, tipo) =>
            {
                sCoinLbl.Text = tipo.ToString();
            });

            //Get orario cached
            //classeCorso = Preferences.Get("Classe", 0) + Preferences.Get("Corso", "");

            loadIndexCache();

        }

        //Detect if onappearing must be triggered
        public static bool forceAppearing; bool userRefreshed = true;
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Push on notification page clicked
            if (NotificationPage != null)
            {
                Navigation.PushModalAsync(NotificationPage);
                NotificationPage = null;
            }

            //Return if triggered from error
            if (((Parent as Helpers.CustomNavigationPage).Parent as TabbedPage).CurrentPage != this.Parent as Helpers.CustomNavigationPage)
                return;

            //Load navigation pages to be faster
          //  await Task.Run((Action)loadNavigationPages);

            if (appearedTimes == 0)
                (TabPage.Argo.RootPage as ArgoPage).initializeInterface();

            //Set navigation view
            todayLbl.Text = DateTime.Now.ToString("dddd").FirstCharToUpper();

            //Do Appearing only every 5 times or from pull to refresh or connection lost or first
            var lastDigit = appearedTimes % 10;
            if (lastDigit != 0 && lastDigit != 5 && !forceAppearing && appearedTimes != 1)
                return;

            //Increment number of appeared times
            appearedTimes++;

            //Sempre meglio mettere il try lol
            try
            {
                //Set image profile url
                userImg.Source = Costants.Uri("images/users/") + Preferences.Get("UserImage", "");

                //Remove modals bug
                ModalPush_Disappearing(null, null);

                //Create static widgets
                widgets.Clear();
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += widget_Tapped;

                //Registro
                var registro = new WidgetGradient { Title = "Registro", SubTitle = lastArgoString(), Icon = "far-calendar-alt", StartColor = "FACA6F", EndColor = "FF7272", Push = new ArgoPages.Registro(), Order = 1 };
                registro.GestureRecognizers.Add(tapGestureRecognizer);

                //Trasporti
                //var trasporti = new WidgetGradient { Title = "Trasporti", SubTitle = await getNextTrain(), Icon = "fas-subway", StartColor = "A872FF", EndColor = "6F8AFA", Push = new SecondaryViews.BusAndTrains(), Order = 3 };
                //trasporti.GestureRecognizers.Add(tapGestureRecognizer);
                //widgets.Add(trasporti);


                //Card
                //var card = new WidgetGradient { Title = "SalveminiCard", SubTitle = "Visualizza tutti i vantaggi esclusivi per gli studenti del Salvemini", Icon = "fas-credit-card", StartColor = "B487FD", EndColor = "FA6FFA", Push = new SecondaryViews.SalveminiCard(), Order = 7 };
                //card.GestureRecognizers.Add(tapGestureRecognizer);

                //Extra
                //var extra = new WidgetGradient { Title = "Extra", SubTitle = "Esplora funzioni aggiuntive", Icon = "fas-star", StartColor = "B487FD", EndColor = "FA6FFA", Order = 6 };
                //extra.GestureRecognizers.Add(tapGestureRecognizer);


                //Initialize list with first widgets
                widgets.Add(registro); 
                OrderWidgets(false); //uncomment to fast load initial widgets

                //Check Internet
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {

                    //Nessuna connessione
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        if (appearedTimes == 0)
                            loadIndexCache();
                    }
                    Costants.showToast("connection");
                    appearedTimes = 5; //Repeat this every time
                    return;
                }

                //Show loading
                userRefreshed = false; homeLoading.IsRefreshing = true; userRefreshed = true;

                //Argo index in background
                await Task.Run((Action)GetArgoIndex);

                //Update orario in background
                //await Task.Run((Action)updateOrario);

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
                        homeLoading.IsRefreshing = false;
                        forceAppearing = false;
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
                    var avvisi = new WidgetGradient { Order = positionAvvisi, Title = "Avvisi", SubTitle = Index.ultimoAvviso.Titolo, Icon = "fas-exclamation-triangle", StartColor = "FACA6F", EndColor = "FA6F6F", Push = new SecondaryViews.Avvisi(), Badge = nuovoAvviso };
                    avvisi.GestureRecognizers.Add(tapGestureRecognizer);
                    widgets.Add(avvisi);
                }

                //Get giornalino
                if (Index.Giornalino != null)
                {
                    //Add giornalino widget
                    string nuovoGiornalino = "no";
                    int positionGiornalino = 5;
                    if (Preferences.Get("LastGiornalino", 0) != Index.Giornalino.id) //New giornalino detected
                    {
                        nuovoGiornalino = "si";
                        positionGiornalino = 0;
                    }

                    var giornalino = new WidgetGradient { Title = "Giornalino", SubTitle = "Edizione di " + Index.Giornalino.Data.ToString("MMMM"), Icon = "fas-book-open", StartColor = "B487FD", EndColor = "6F8AFA", Order = positionGiornalino, Badge = nuovoGiornalino };
                    giornalino.GestureRecognizers.Add(tapGestureRecognizer);
                    widgets.Add(giornalino);
                }

                //Update widgets order
                OrderWidgets(false);
                homeLoading.IsRefreshing = false;

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
                //if (Index.OrarioTrasportiVersion > Preferences.Get("OrarioTrasportiVersion", 0))
                //{
                //    bool successTreni = await App.Treni.GetTrainJson();
                //    if (successTreni)
                //    {
                //        Preferences.Set("OrarioTrasportiVersion", Index.OrarioTrasportiVersion);
                //        //await getNextTrain();
                //    }
                //}

                //Check app version
                var appversion = Convert.ToDecimal(VersionTracking.CurrentVersion);
                if (Index.AppVersion > appversion)
                    newVersion(); //New version detected

                forceAppearing = false;
            }
            catch (Exception ex) //Errore sconosciuto :0
            {
                homeLoading.IsRefreshing = false;
                forceAppearing = true;
                Costants.showToast("Si è verificato un errore fatale, contatta gli sviluppatori se il problema persiste");
            }

        }

        public void refreshHome(object sender, EventArgs e)
        {
            //Fix bcause this event is triggered even if is refreshed by code
            if (!userRefreshed)
                return;

            forceAppearing = true;
            OnAppearing();
        }


        //Handle widget redirections
        async void widget_Tapped(object sender, System.EventArgs e)
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
                        if (!Costants.DownloadedOrariTrasporti() || Preferences.Get("firstTimeTrasporti", true))
                            Navigation.PushModalAsync(new FirstAccess.OrariTrasporti());

                        else
                            Navigation.PushAsync(widget.Push); //Push to pullman, treni, ali
                    }
                    else
                    {
                        Navigation.PushModalAsync(widget.Push); //Modal
                    }
                }

                //Giornalino
                if (widget.Title == "Giornalino")
                {
                    //Last or other editions?
                    string decision = await DisplayActionSheet("Che edizione del giornalino vuoi leggere?", "Chiudi", null, Index.Giornalino.Data.ToString("MMMM").FirstCharToUpper() + " (ultima)", "Vedi altre");
                    if (decision == "Chiudi")
                        return;
                    if (decision != "Vedi altre")
                    {
                        //Last
                        Preferences.Set("LastGiornalino", Index.Giornalino.id);
                        Costants.OpenPdf(Index.Giornalino.Url, "Giornalino"); //Show webpage
                        RemoveBadge("Giornalino");
                    }
                    else
                    {
                        //Download others
                        Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Attendi", Acr.UserDialogs.MaskType.Black);
                        var versioni = await App.Index.GetGiornalini();
                        if (versioni == null || versioni.Count <= 1)
                        {
                            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                            Costants.showToast("Non ci sono altre edizioni disponibili del giornalino!");
                            return;
                        }
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();

                        //Create new list of editions
                        var versioni_ = new List<string>();
                        foreach (var versione in versioni) { versioni_.Add(versione.Data.ToString("MMMM").FirstCharToUpper()); }
                        string decision_ = await DisplayActionSheet("Che edizione del giornalino vuoi leggere?", "Chiudi", null, versioni_.ToArray());
                        if (versioni_.Contains(decision_))
                            Costants.OpenPdf(versioni[versioni_.IndexOf(decision_)].Url, "Giornalino"); //Show webpage
                    }

                }
                //Extra
                if (widget.Title == "Extra")
                {
                    Navigation.PushModalAsync(extraPage);
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
            if (NextTrain == null)
                return "Visualizza gli orari di treni, bus e aliscafi";
            string result = "<p>Il prossimo treno per <strong>" + NextTrain.DirectionString + "</strong> partirà alle <strong>" + NextTrain.Partenza + "</strong> da <strong>" + Costants.Stazioni[NextTrain.Stazione] + "</strong></p>";
            return result;
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




        //Push to profile page
        void profilePush(object sender, System.EventArgs e)
        {
            //PROFILE
            //Create new navigation page
            profilePage = new Xamarin.Forms.NavigationPage(new SecondaryViews.Profile());
            profilePage.BarTextColor = Styles.TextColor;
            profilePage.BarBackgroundColor = Styles.BGColor;

            //Add disappearing event
            profilePage.Disappearing += ModalPush_Disappearing;

            //Add toolbaritem to close page
            var close = new ToolbarItem { Text = "Annulla" };
            close.Clicked += ModalPush_Disappearing;
            profilePage.ToolbarItems.Add(close);

            profilePage.BarTextColor = Styles.TextColor;
            Navigation.PushModalAsync(profilePage);
        }

        //Fix bug on ios 13 that doesen t close modal automatically
        private void ModalPush_Disappearing(object sender, EventArgs e)
        {
            if (!isSelectingImage)
                Navigation.PopModalAsync();
            else
                isSelectingImage = false;
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
            widgetsLayout.Children.AddRange(widgets.ToList());
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


        //public async void updateOrario()
        //{
        //    //Download orario
        //    var data = await App.Orari.GetOrario(classeCorso);

        //    //Failed to get
        //    if (data.Message != null)
        //    {
        //        if (data.Message == "L'orario della classe non è stato trovato")
        //        {
        //            orario.ShowPlaceholder = true;
        //            return;
        //        }
        //        Costants.showToast(data.Message);
        //        orario.ShowPlaceholder = false;
        //        orario.Lezioni = new List<RestApi.Models.Lezione>(); return;
        //    }

        //    //Set null if error occourred
        //    if (data.Data == null) { orario.Lezioni = new List<RestApi.Models.Lezione>(); return; }

        //    //Update lezioni
        //    Preferences.Set("OrarioSaved", true);
        //    orario.Lezioni = data.Data as List<RestApi.Models.Lezione>;

        //    //Show agenda if hidden bug (usually on first time)
        //    MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "showAgenda");


        //    ////Add siri shortcut
        //    //if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
        //    //    return; //Pre ios 13 can't use this :(

        //    ////Check if intent is already added or user doesen't want to add it
        //    //if (Preferences.Get("OrarioSiriSet", false) || Preferences.Get("DontShowSiriWidget", false))
        //    //    return;

        //    ////Save values for siri intent
        //    //var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
        //    //defaults.AddSuite("group.com.codex.SalveminiApp");
        //    //defaults.SetString(classeCorso, new NSString("SiriClass"));

        //    ////Create intent
        //    //var orarioIntent = new OrarioIntent();
        //    //orarioIntent.SuggestedInvocationPhrase = "Orario di domani";
        //    //INShortcut shortcut = new INShortcut(orarioIntent);

        //    //Device.BeginInvokeOnMainThread(() =>
        //    //{
        //    //    var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
        //    //    var popup = new iOS.SiriShortcutPopup(shortcut, "Orario", false);
        //    //    vc.PresentViewController(popup, true, null);

        //    //    //vc.View.AddSubview(popup.View);
        //    //    popup.DidMoveToParentViewController(vc);

        //    //    var height = vc.View.Frame.Height;
        //    //    var width = vc.View.Frame.Width;
        //    //    popup.View.Frame = new CoreGraphics.CGRect(0, vc.View.Frame.Y, width, height);

        //    //});

        //}



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
            if (Index.AppVersion > appversion && appearedTimes % 2 != 0) //Show this only 1/2
            {
                bool choice = await DisplayAlert("Buone notizie!", "È disponibile un aggiornamento dell'app, recati sullo store per effettuarlo!", "Aggiorna", "Chiudi");
                if (choice)
                {
                    //Redirect to store
                    if (Device.RuntimePlatform == Device.iOS)
                       await Launcher.OpenAsync(new Uri("https://apps.apple.com/it/app/salvemini/id1438855517"));
                    else
                        await Launcher.OpenAsync(new Uri("https://play.google.com/store/apps/details?id=com.codex.salveminiapp&hl=it"));
                }
            }
        }

        public void sCoin_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(scoinPage);
        }

        async void loadNavigationPages()
        {

            if (extraPage != null && profilePage != null && scoinPage != null) //All loaded, nothing to do
                return;

            //PROFILE
            //Create new navigation page
            profilePage = new Xamarin.Forms.NavigationPage(new SecondaryViews.Profile());
            profilePage.BarTextColor = Styles.TextColor;
            profilePage.BarBackgroundColor = Styles.BGColor;

            //Add disappearing event
            profilePage.Disappearing += ModalPush_Disappearing;

            //Add toolbaritem to close page
            var close = new ToolbarItem { Text = "Annulla" };
            close.Clicked += ModalPush_Disappearing;
            profilePage.ToolbarItems.Add(close);

            profilePage.BarTextColor = Styles.TextColor;

            //COIN
            //Create new navigation page
            scoinPage = new Helpers.CustomNavigationPage(new SalveminiCoin.CoinMenu());
            scoinPage.BarTextColor = Styles.TextColor;
            scoinPage.BarBackgroundColor = Styles.BGColor;

            //Add disappearing event
            scoinPage.Disappearing += ModalPush_Disappearing;

            //Add toolbaritem to close page
            scoinPage.ToolbarItems.Add(close);

            scoinPage.BarTextColor = Styles.TextColor;

            //EXTRA
            //Create new navigation page
            extraPage = new Xamarin.Forms.NavigationPage(new SecondaryViews.Extra());
            extraPage.BarTextColor = Styles.TextColor;
            extraPage.BarBackgroundColor = Styles.BGColor;
            //Add disappearing event
            extraPage.Disappearing += ModalPush_Disappearing;

            //Add toolbaritem to close page
            extraPage.ToolbarItems.Add(close);

            extraPage.BarTextColor = Styles.TextColor;

        }

        public void loadIndexCache()
        {
            //orario.ClasseCorso = classeCorso;

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

        void AdInfo_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Info Sponsor", "Il liceo Salvemini non lucra in alcun modo da questo progetto, tutto il guadagno è indirizzato agli sviluppatori dell'app", "Ok");
        }

    }


}
