using System;
using System.Collections.Generic;
using Forms9Patch;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using MonkeyCache.SQLite;

namespace SalveminiApp.Controls
{
    public partial class Orario : Xamarin.Forms.ContentView
    {
        //Giorni popover
        BubblePopup dayPopOver = new Helpers.PopOvers().defaultPopOver;
        Xamarin.Forms.ListView giorniList = new Xamarin.Forms.ListView { VerticalScrollBarVisibility = ScrollBarVisibility.Never, Footer = "", BackgroundColor = Color.Transparent, SeparatorColor = Color.Gray, WidthRequest = App.ScreenWidth / 3.8, HeightRequest = App.ScreenHeight / 3.8 };
        //Full orario list, all days
        public List<RestApi.Models.Lezione> LezioniList = new List<RestApi.Models.Lezione>();

        public Orario()
        {
            InitializeComponent();
            //Hide or show hints
            if (!Preferences.Get("showHintOrario", true))
                hintOrario.IsVisible = false;

            //hintOrario.WidthRequest = App.ScreenWidth / 14;
            //hintOrario.HeightRequest = App.ScreenWidth / 14;

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



        }


        //Show popover with daylist
        void ChangeDay_Clicked(object sender, System.EventArgs e)
        {
            //Hide initial hint
            Preferences.Set("showHintOrario", false);
            hintOrario.IsVisible = false;

            //Create new popover
            dayPopOver = new Helpers.PopOvers().defaultPopOver;
            dayPopOver.Content = giorniList;
            dayPopOver.PointerDirection = PointerDirection.Up;
            dayPopOver.PreferredPointerDirection = PointerDirection.Up;
            dayPopOver.Target = arrowDown;
            dayPopOver.BackgroundColor = Color.FromHex("202020");
            dayPopOver.IsVisible = true;
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
                if (Lezioni != null)
                {
                    materieLayout.IsVisible = true;

                    //Detect freeday
                    var freedayInt = Lezioni.FirstOrDefault(x => x.Materia == "Libero").Giorno;

                    //Save freeday
                    Preferences.Set("FreedayInt", freedayInt);



                    //intelligent auto skip if dopo le 2
                    if (today && DateTime.Now.Hour > 14 && day != 6 && day != freedayInt)
                    {
                        daySkipped++;
                        day = SkipDay(day);
                    }

                    //Skip freeday (if saturday)
                    if (day == freedayInt)
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

                    //Skip freeday (if monday)
                    if (day == freedayInt)
                    {
                        daySkipped++;
                        day = SkipDay(day);
                    }

                    //get only today lessons
                    var orarioOggi = GetOrarioDay(day, Lezioni);

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
                    Costants.showToast("Non è stato possibile recuperare l'orario, contattaci se il problema persiste");
                    materieLayout.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                materieLayout.IsVisible = false;
                Costants.showToast("Non è stato possibile recuperare l'orario, contattaci se il problema persiste");
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
                //  orarioFromCached = false;
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

        public int SkipDay(int day)
        {
            int newDay = day;
            newDay++;
            if (newDay == 7)
                return 0;
            else
                return newDay;
        }


        public static readonly BindableProperty LezioniProperty = BindableProperty.Create(nameof(Lezioni), typeof(List<RestApi.Models.Lezione>), typeof(Orario), default(List<RestApi.Models.Lezione>), Xamarin.Forms.BindingMode.OneWay);
        public List<RestApi.Models.Lezione> Lezioni
        {
            get
            {
                return (List<RestApi.Models.Lezione>)GetValue(LezioniProperty);
            }

            set
            {
                SetValue(LezioniProperty, value);
            }
        }

        //Orario not found
        public static readonly BindableProperty ShowPlaceholderProperty = BindableProperty.Create(nameof(ShowPlaceholder), typeof(bool), typeof(Orario), default(bool), Xamarin.Forms.BindingMode.OneWay);
        public bool ShowPlaceholder
        {
            get
            {
                return (bool)GetValue(ShowPlaceholderProperty);
            }

            set
            {
                SetValue(ShowPlaceholderProperty, value);
            }
        }

        //Get from cache fast
        public static readonly BindableProperty ClasseCorsoProperty = BindableProperty.Create(nameof(ClasseCorso), typeof(string), typeof(Orario), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string ClasseCorso
        {
            get
            {
                return (string)GetValue(ClasseCorsoProperty);
            }

            set
            {
                SetValue(ClasseCorsoProperty, value);
            }
        }



        //Update values
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            try
            {
                if (propertyName == LezioniProperty.PropertyName)
                {
                    //Hide if no lessons found
                    if (Lezioni == null || Lezioni.Count < 1)
                    {
                        this.IsVisible = false;
                    }

                    //Delete last color palette
                    Costants.ClearColors();
                    //Update lessons list
                    LezioniList = Lezioni;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        changeDay(-1);
                        this.IsVisible = true;
                    });
                }

                //Show hidden placeholder
                if (propertyName == ShowPlaceholderProperty.PropertyName)
                {

                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (ShowPlaceholder)
                        {
                            emptyLayout.IsVisible = true;
                            orarioFrame.IsVisible = false;
                        }
                        else
                        {
                            emptyLayout.IsVisible = false;
                            orarioFrame.IsVisible = true;
                        }
                    });
                }

                //Get from cache
                if (propertyName == ClasseCorsoProperty.PropertyName)
                {
                    if (!string.IsNullOrEmpty(ClasseCorso))
                    {
                        var orarioCached = CacheHelper.GetCache<List<RestApi.Models.Lezione>>("orario" + ClasseCorso);
                        if (orarioCached != null)
                        {
                            Lezioni = orarioCached;
                            changeDay(-1);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Boh per sicurezza a volte fa cose strane
                return;
            }

        }

        public List<RestApi.Models.Lezione> GetOrarioDay(int day, List<RestApi.Models.Lezione> orario)
        {
            //Filter list per day and order by hour
            orario = orario.Where(x => x.Giorno == day).OrderBy(x => x.Ora).ToList();


            //Group hours by duration
            for (int i = 0; i < orario.Count; i++)
            {
                //Skip hour if is equal to the previous
                if (orario.ElementAtOrDefault(i - 1) != null && orario[i].Materia == orario[i - 1].Materia || string.IsNullOrEmpty(orario[i].Materia))
                {
                    orario[i].toRemove = true;
                    continue;
                }
                int next = 1;
                RestApi.Models.Lezione lezione()
                {
                    //Check if next hour is the same of this hour
                    if (orario.ElementAtOrDefault(i + next) != null && orario[i].Materia == orario[i + next].Materia)
                    {
                        //Increment hours number
                        orario[i].numOre = next + 1;
                        //Increment
                        next++;
                        lezione();
                    }


                    return orario[i];
                }

                orario[i] = lezione();
            }

            foreach (var lezione in orario)
            {
                if (lezione.numOre == 0)
                {
                    //Set Number of hours to default
                    lezione.numOre = 1;
                }

                //Setup Height Of Hour
                lezione.OrarioFrameHeight = (App.ScreenHeight / 24) * lezione.numOre;

                //Setup Corner Radius
                lezione.OrarioFrameRadius = lezione.numOre > 1 ? 10 : 8;
            }

            //Remove to remove items
            orario = orario.Where(x => x.toRemove == false).ToList();


            Barrel.Current.Add("orarioday" + day, orario, TimeSpan.FromDays(90));

            return orario;
        }


    }

}


