using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using MonkeyCache.SQLite;
using Xamarin.Essentials;

namespace SalveminiApp
{
    public class Costants
    {
        //Api Url
        public static string Uri(string next, bool api = true)
        {
        if(api)
            return "http://mysalvemini.me/api/" + next;
        else
                return "http://mysalvemini.me/" + next;

        }

        //Materie Dictionary
        public static List<string> Colors = new List<string>
        {
            "#5BB0E5", "#7D77FF", "#FFBB4E" ,"#FF7064",  "#EA5AEA","#48EB9A","#FF5A1D","#3F83E0","#A346E8","#C73636","#C73679","#36C7C7"
        };

        public static string SetColors(string materia)
        {
            try
            {
                //Index of colors
                var index = Preferences.Get("colorLoop", 0);

                if (Preferences.Get("mat" + materia, "#802891") == "#802891")
                {
                    //save new color
                    Preferences.Set("mat" + materia, Colors[index]);
                    //reset index to prevent crashes
                    if (index == Colors.Count())
                        Preferences.Set("colorLoop", 0);
                    else
                        Preferences.Set("colorLoop", index + 1);
                    return Preferences.Get("mat" + materia, "#802891");
                }
                else
                {
                    return Preferences.Get("mat" + materia, "#802891");
                }
            }
            catch
            {
                return Preferences.Get("mat" + materia, "#802891");
            }

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
            var Days = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList();
            //Remove sunday
            Days.RemoveAt(0);
            //First letter to upper
            Days = Days.ConvertAll(x => x.FirstCharToUpper());
            return Days;
        }

        public static void Logout(bool fromSuperVip = false)
        {
            //Clear preferences except firsttime
            Preferences.Clear();
            Preferences.Set("veryFirstTime",false); Preferences.Set("isFirstTime",false);

            //Clear cache
            Barrel.Current.EmptyAll();

            //Push to login only if triggered from super vip page
            if (!fromSuperVip)
                Xamarin.Forms.Application.Current.MainPage = new FirstAccess.Login();
        }

        public static void showToast(string message)
        {
            //Costants
            if (message == "connection") message = "Nessuna connessione rilevata";
#if __IOS__
            //Show toast
            BigTed2.BTProgressHUD2.ShowToast(message, BigTed2.ProgressHUD2.MaskType.None, false, 3000);

            //Vibrate
            iOS.AppDelegate.hapticVibration();
#endif

#if _ANDROID_
            //Show toast
            SalveminiApp.Droid.ShowToast.LongAlert(message);

            //Vibrate
            try
            {
                Vibration.Vibrate();

                var duration = TimeSpan.FromMilliseconds(200);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                Console.WriteLine("Not supported");
            }
#endif
        }
    }

    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }
}
