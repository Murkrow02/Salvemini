﻿using System;
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

            //Get day names list
            var giorni = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            //Remove sunday
            giorni.RemoveAt(0);
            //First letter to upper
            giorni = giorni.ConvertAll(x => x.FirstCharToUpper());
            giorniList.ItemsSource = giorni;


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
            MessagingCenter.Subscribe<App>(this, "RemoveAvvisiBadge", (sender) =>
            {
                RemoveBadge();
            });

            //Fill initial cache
            if (Barrel.Current.Exists("Index"))
            {
                var tempIndex = Barrel.Current.Get<RestApi.Models.Index>("Index");
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
            //Trasporti
            var registro = new WidgetGradient { Title = "Registro", SubTitle = "Qua ci va l'ultima cosa che hai preso oggi", Icon = "far-calendar-alt", StartColor = "FACA6F", EndColor = "FF7272", Push = new ArgoPages.Registro(), Order = Preferences.Get("OrderRegistro", 0) };
            registro.GestureRecognizers.Add(tapGestureRecognizer);
            //Trasporti
            var trasporti = new WidgetGradient { Title = "Trasporti", SubTitle = await getNextTrain(), Icon = "fas-subway", StartColor = "A872FF", EndColor = "6F8AFA", Push = new SecondaryViews.BusAndTrains(), Order = Preferences.Get("OrderTrasporti", 0) };
            trasporti.GestureRecognizers.Add(tapGestureRecognizer);
            //Card
            var card = new WidgetGradient { Title = "SalveminiCard", SubTitle = "Visualizza tutti i vantaggi esclusivi per gli studenti del Salvemini", Icon = "fas-credit-card", StartColor = "B487FD", EndColor = "FA6FFA", Push = new SecondaryViews.SalveminiCard(), Order = Preferences.Get("OrderCard", 0) };
            card.GestureRecognizers.Add(tapGestureRecognizer);
            //Extra
            var extra = new WidgetGradient { Title = "Extra", SubTitle = "Esplora funzioni aggiuntive", Icon = "fas-star", StartColor = "B487FD", EndColor = "FA6FFA", Order = Preferences.Get("OrderExtra", 0), Push = new SecondaryViews.Extra() };
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
                //Get updated orario
                var _orario = await App.Orari.GetOrario(classeCorso);

                //Check if success and if there are updates
                if (_orario != null && Orario != _orario)
                {
                    //Update with new orario
                    Orario = _orario;
                    changeDay(-1);
                }

                Index = await App.Index.GetIndex();

                //Checks
                //Authorized? todo
                if (Index != null)
                {
                    if (!Index.Authorized)
                    {
                        await DisplayAlert("Attenzione", "Accesso all'app non autorizzato, la sessione di argo è scaduta oppure il tuo account è stato disabilitato", "Ok");
                        Costants.Logout();
                    }
                }


                //Get last sondaggio
                if (Index.ultimoSondaggio != null)
                {
                    string nuovoSondaggio = "no";
                    int positionSondaggio = Preferences.Get("OrderSondaggio", 0);
                    if (Preferences.Get("LastSondaggio", 0) != Index.ultimoSondaggio.id) //New sondaggio detected
                    {
                        //Preferences.Set("LastSondaggio", Index.ultimoSondaggio.id);
                        nuovoSondaggio = "si";
                        positionSondaggio = -1;
                    }
                    //Create sondaggio widget
                    var sondaggi = new WidgetGradient { Order = positionSondaggio, Title = "Sondaggi", SubTitle = Index.ultimoSondaggio.Nome, Icon = "fas-people-poll", StartColor = "FD8787", EndColor = "F56FFA", Badge = nuovoSondaggio };
                    sondaggi.GestureRecognizers.Add(tapGestureRecognizer);
                    widgets.Add(sondaggi);

                }


                //Get last avviso
                if (Index.ultimoAvviso != null)
                {
                    string nuovoAvviso = "no";
                    int positionAvvisi = Preferences.Get("OrderAvvisi", 0);
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


        //Push to profile pageo
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
            //-1 notify the algorythm that today is selected
            bool today = day == -1;
            orarioList.IsVisible = true;

            //reset original day
            if (today)
                day = (int)DateTime.Now.DayOfWeek;

            try
            {
                //Orario loaded successfully
                if (Orario != null)
                {
                    //Detect sunday
                    if (day == 0) day++;
                    //Intelligent skip today
                    if(today && DateTime.Now.Hour > 14)
                        day++;

                    //Fill orario list
                    var orarioOggi = await App.Orari.GetOrarioDay(day, Orario);

                    //Skip freeday
                    if (orarioOggi[0].Materia == "Libero") 
                    {
                        day++;
                        orarioOggi = await App.Orari.GetOrarioDay(day, Orario);
                    }

                    //Set day label display text
                    if (day == (int)DateTime.Now.DayOfWeek)
                        orarioDay.Text = "Oggi"; //Lol it's today
                    else if (today && DateTime.Now.Hour > 14)
                    {
                        orarioDay.Text = "Domani"; //Auto select tomorrow
                    }
                    else
                    {
                        var giorni = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
                        orarioDay.Text = giorni[day].FirstCharToUpper(); //Other day

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

        public void RemoveBadge()
        {
            var widget = widgets.First(x => x.Title == "Avvisi");
            widgets.Remove(widget);
            widget.Badge = "no";
            widgets.Add(widget);
            OrderWidgets(true);
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
    }
}
