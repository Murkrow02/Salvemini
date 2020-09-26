﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using MonkeyCache.SQLite;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace SalveminiApp
{
    public class Costants
    {
        //Api Url
        public static string Uri(string next, bool api = true)
        {
            if (api)
                return "https://www.mysalvemini.me/api/" + next;
            else
                return "https://www.mysalvemini.me/" + next;

        }

        //Materie Dictionary
        public static List<string> Colors = new List<string>
        {
            "#5BB0E5", "#7D77FF", "#FFBB4E" ,"#FF7064",  "#EA5AEA","#48EB9A","#FF5A1D","#3F83E0","#A346E8","#C73636","#C73679","#36C7C7"  ,"#47C736","#C77E36","#195AD5","#3FA398","#B4708D","#3FA398"
        };

        public static string SetColors(int idMateria)
        {
            try
            {
                //Index of colors
                var index = Preferences.Get("colorLoop", 0);

                if (Preferences.Get("mat" + idMateria, "#802891") == "#802891") //Empty color
                {
                    //save new color
                    Preferences.Set("mat" + idMateria, Colors[index]);
                    Preferences.Set("matlist", Preferences.Get("matlist", "") + " mat" + idMateria + ",");
                    //reset index to prevent crashes
                    if (index == Colors.Count())
                        Preferences.Set("colorLoop", 0);
                    else
                        Preferences.Set("colorLoop", index + 1);

                    //Return generated color
                    return Preferences.Get("mat" + idMateria, "#802891");
                }
                else
                {
                    return Preferences.Get("mat" + idMateria, "#802891");
                }
            }
            catch
            {
                return Preferences.Get("mat" + idMateria, "#802891");
            }
        }

        public static void ClearColors()
        {
            try
            {
                var colori = Preferences.Get("matlist", "");
                var listaColori = colori.Split(',').ToList();
                foreach (var colore in listaColori)
                {
                    Preferences.Remove(colore);
                }
                Preferences.Set("colorLoop", 0);
            }
            catch
            {

            }
        }

        public static Xamarin.Forms.Color ColorLoop(int index)
        {
            try
            {
                return Xamarin.Forms.Color.FromHex(Colors[index - Convert.ToInt32((index / Colors.Count) * Colors.Count)]);
            }
            catch
            {
                return Xamarin.Forms.Color.FromHex(Colors[0]);
            }
        }

        public static Xamarin.Forms.Color RandomColor()
        {
            Random rnd = new Random();
            int colorIndex = rnd.Next(0, Colors.Count - 1);
            return Xamarin.Forms.Color.FromHex(Colors[colorIndex]);
        }

        public static Dictionary<int, string> Ore = new Dictionary<int, string>
        {
            {1, " 8:00" },
            {2, " 9:00"  },
            {3, "10:00"  },
            {4, "11:00" },
            {5, "11:55" },
            {6, "12:50" },
           {7, "13:45" }

        };

        public static Dictionary<string, string[]> Rotte = new Dictionary<string, string[]>
        {
            {"Sorrento", new string[] { "Capri", "Amalfi", "Ischia", "Napoli", "Positano", "Procida"} },
            {"Capri", new string[] { "Sorrento", "Amalfi", "Castellammare", "Ischia", "Napoli", "Piano di Sorrento", "Positano", "Salerno", "Torre del Greco"} },
            {"Piano di Sorrento", new string[] {"Capri"} },
            {"Castellammare", new string[] {"Capri"} },
            {"Torre del Greco", new string[] {"Capri"} },
            {"Napoli", new string[] {"Capri", "Ischia", "Procida", "Positano", "Sorrento"} },
            {"Positano", new string[] {"Capri", "Amalfi", "Ischia", "Napoli", "Salerno", "Sorrento"} },
        };

        public static Dictionary<int, string> Stazioni = new Dictionary<int, string>
        {
            {0, "Sorrento"},
            {1, "S. Agnello"},
            {2, "Piano"},
            {3, "Meta"},
            {4, "Seiano"},
            {5, "Vico Equense"},
            {6, "Castellammare di Stabia"},
            {7, "Via Nocera"},
            {8, "Pioppaino"},
            {9, "Moregine"},
            {10, "Pompei Scavi"},
            {11, "Villa Regina"},
            {12, "Torre Annunziata"},
            {13, "Trecase"},
            {14, "Via Viuli"},
            {15, "Leopardi"},
            {16, "Villa delle Ginestre"},
            {17, "Via dei Monaci"},
            {18, "Via del Monte"},
            {19, "Via S'Antonio"},
            {20, "Torre del Greco"},
            {21, "Ercolano Miglio d'Oro"},
            {22, "Ercolano Scavi"},
            {23, "Portici Via Libertà"},
            {24, "Portici Bellavista"},
            {25, "S'Giorgio Cavalli di Bronzo"},
            {26, "S'Giorgio a Cremano"},
            {27, "S'Maria del Pozzo"},
            {28, "Barra"},
            {29, "S'Giovanni a Teduccio"},
            {30, "Via Gianturco"},
            {31, "Napoli Piazza Garibaldi"},
            {32, "Napoli Porta Nolana"},
        };



        public static Dictionary<string, string> Giorni = new Dictionary<string, string>
        {
            {"Sunday", "Domenica"},
            {"Monday", "Lunedì"},
            {"Tuesday", "Martedì"},
            {"Wednesday", "Mercoledì"},
            {"Thursday", "Giovedì"},
            {"Friday", "Venerdì"},
            {"Saturday", "Sabato"},
        };

        public static List<string> getDays()
        {
            //Get day names list
            var culture = new CultureInfo("it-IT");
            var Days = culture.DateTimeFormat.DayNames.ToList();
            //Remove sunday
            Days.RemoveAt(0);
            //First letter to upper
            Days = Days.ConvertAll(x => x.FirstCharToUpper());
            return Days;
        }

        public static async void Logout(bool fromSuperVip = false)
        {
            //Clear preferences except firsttime
            Preferences.Clear();
            Preferences.Set("veryFirstTime", false); Preferences.Set("isFirstTime", false);

            //Clear cache
            Barrel.Current.EmptyAll();
            //Helpers.GetStorageInfo.storageInfo(true);
            FFImageLoading.ImageService.Instance.InvalidateCacheAsync(FFImageLoading.Cache.CacheType.All);

            //Push to login only if triggered from super vip page
            if (!fromSuperVip)
                Xamarin.Forms.Application.Current.MainPage = new FirstAccess.Login();
        }

        public static void showToast(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {

                //Costants
                if (message == "connection") message = "Nessuna connessione rilevata";
                DependencyService.Get<IPlatformSpecific>().SendToast(message);
            });
        }





        public static DateTime GetNextWeekday(DayOfWeek day)
        {
            var start = DateTime.Now;
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static async void OpenPdf(string url, string title)
        {
            try
            {
if(Device.RuntimePlatform == Device.Android)
                {
                    await Launcher.OpenAsync(new Uri("http://drive.google.com/viewer?url=" + url));
                    return;
                }
                

                //Create page with webview
                var webView = new WebView() { Source = url }; webView.Navigating += WebView_Navigating; webView.Navigated += WebView_Navigated;
                var contentPage = new ContentPage { Title = title };
                contentPage.Content = webView;
                //Add toolbaritems to the page
                var barItem = new ToolbarItem { Text = "Chiudi", };
                barItem.Clicked += (object mandatore, EventArgs f) =>
                {
                    Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
                };
                contentPage.ToolbarItems.Add(barItem);

                //Make it navigable
                var webPage = new Xamarin.Forms.NavigationPage(contentPage) { BarTextColor = Styles.TextColor };

                //Show or hide loading page
                void WebView_Navigating(object sender, WebNavigatingEventArgs e)
                {
                  UserDialogs.Instance.ShowLoading("Caricamento...", MaskType.Black);
                
                }
                void WebView_Navigated(object sender, WebNavigatedEventArgs e)
                {
                    UserDialogs.Instance.HideLoading();
                }


                Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(webPage);
            }
            catch
            {
                showToast("Impossibile aprire il file");
            }

        }

        public static bool DownloadedOrariTrasporti()
        {
            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OrariTreni.txt");
            if (File.Exists(filename)) return true; return false;
        }

        public static string SpanString(TimeSpan Data)
        {
            //Anni fa
            if (Data.Days >= 365)
                return (Data.Days / 365).ToString() + (Data.Days / 365 == 1 ? " anno fa" : " anni fa");

            //Mesi
            if (Data.Days >= 30)
                return (Data.Days / 30).ToString() + (Data.Days / 30 == 1 ? " mese fa" : " mesi fa");

            //Giorni
            if (Data.Days >= 7)
                return (Data.Days / 7).ToString() + (Data.Days / 7 == 1 ? " settimana fa" : " settimane fa");

            //Giorni
            if (Data.Days >= 1)
                return (Data.Days).ToString() + (Data.Days == 1 ? " giorno fa" : " giorni fa");

            //Ore
            if (Data.Hours >= 1)
                return (Data.Hours).ToString() + (Data.Hours == 1 ? " ora fa" : " ore fa");

            //Minuti
            if (Data.Minutes >= 1)
                return (Data.Minutes).ToString() + (Data.Minutes == 1 ? " minuto fa" : " minuti fa");

            //Secondi
            if (Data.Seconds >= 1)
                return (Data.Seconds).ToString() + (Data.Seconds == 1 ? " secondo fa" : " secondi fa");

            return "Ora";
        }
    }

    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }

        /// <summary>
        /// Gets the page to which an element belongs
        /// </summary>
        /// <returns>The page.</returns>
        /// <param name="element">Element.</param>
        public static Xamarin.Forms.Page GetParentPage(this Xamarin.Forms.VisualElement element)
        {
            if (element != null)
            {
                var parent = element.Parent;
                while (parent != null)
                {
                    if (parent is Xamarin.Forms.Page)
                    {
                        return parent as Xamarin.Forms.Page;
                    }
                    parent = parent.Parent;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the page to which an element belongs
        /// </summary>
        /// <returns>The page.</returns>
        /// <param name="element">Element.</param>
        public static Xamarin.Forms.ListView GetParentList(this Xamarin.Forms.VisualElement element)
        {
            if (element != null)
            {
                var parent = element.Parent;
                while (parent != null)
                {
                    if (parent is Xamarin.Forms.ListView)
                    {
                        return parent as Xamarin.Forms.ListView;
                    }
                    parent = parent.Parent;
                }
            }
            return null;
        }

        public static Task<bool> ColorTo(this Xamarin.Forms.VisualElement self, Xamarin.Forms.Color fromColor, Xamarin.Forms.Color toColor, Action<Xamarin.Forms.Color> callback, uint length = 250, Easing easing = null)
        {
            Func<double, Xamarin.Forms.Color> transform = (t) =>
              Xamarin.Forms.Color.FromRgba(fromColor.R + t * (toColor.R - fromColor.R),
                             fromColor.G + t * (toColor.G - fromColor.G),
                             fromColor.B + t * (toColor.B - fromColor.B),
                             fromColor.A + t * (toColor.A - fromColor.A));

            return ColorAnimation(self, "ColorTo", transform, callback, length, easing);
        }

        static Task<bool> ColorAnimation(Xamarin.Forms.VisualElement element, string name, Func<double, Xamarin.Forms.Color> transform, Action<Xamarin.Forms.Color> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate<Xamarin.Forms.Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }

       

        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }



        public static string GetHexString(this Xamarin.Forms.Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        public static string SerializeObject(this object item)
        {
            try
            {
                return JsonConvert.SerializeObject(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> col, IEnumerable<T> content)
        {
            content.ForEach(x => col.Add(x));

            return col;
        }

        public static ObservableCollection<T> InsertRange<T>(this ObservableCollection<T> col, int index, IEnumerable<T> content)
        {
            content = content.Reverse();
            content.ForEach(x => col.Insert(index, x));

            return col;
        }

        public static ObservableCollection<T> RemoveRange<T>(this ObservableCollection<T> col, IEnumerable<T> content)
        {
            for (int i = col.Count - 1; i >= 0; i--)
            {
                foreach (var item in content)
                {
                    if (item.SerializeObject() == col[i].SerializeObject())
                    {
                        col.RemoveAt(i);
                        break;
                    }
                }
            }

            return col;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> col, Action<T> action)
        {
            foreach (var item in col)
            {
                action.Invoke(item);
            }
            return col;
        }

        public static List<T> MoveIndex<T>(this List<T> col, int oldIndex, int newIndex)
        {
            T item = col[oldIndex];
            col.Remove(item);
            col.Insert(newIndex, item);
            return col;
        }


        public static Task ScaleHeightTo(this Xamarin.Forms.VisualElement self, double value, uint duration, Easing easing = null)
        {
            var animate = new Animation(d => self.HeightRequest = d, self.Height, value);
            animate.Commit(self, "ScaleHeightTo", 16, duration, easing);

            return Task.CompletedTask;
        }

       

    }

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
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

    }
}
