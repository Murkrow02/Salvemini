using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MonkeyCache.SQLite;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Voti : INotifyPropertyChanged
    {
        private bool nonFaMedia_;

        public event PropertyChangedEventHandler PropertyChanged;


        internal Voti(bool nonfamedia)
        {
            this.nonFaMedia_ = nonfamedia;
        }

        public Voti()
        {

        }
        public bool NonFaMedia
        {
            get { return nonFaMedia_; }
            set
            {
                nonFaMedia_ = value;
                OnPropertyChanged("NonFaMedia");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string datGiorno { get; set; }
        public string codVotoPratico { get; set; }
        public string desProva { get; set; }
        public string codVoto { get; set; }
        public string desCommento { get; set; }
        public string docente { get; set; }
        public int prgMateria { get; set; }
        public double? decValore { get; set; }
        public string Materia { get; set; }

        public bool Distructive
        {
            get
            {
                return !NonFaMedia;
            }
        }

        public string Voto
        {
            get
            {
                if (codVoto == "G")
                    return "Giustifica";
                else return codVoto;
            }
        }

        public string MenuItemText
        {
            get
            {
                return !NonFaMedia ? "Non fa media" : "Fa media";
            }
        }

        public string formattedTeacher
        {
            get
            {
                return docente.Substring(7).Replace(")", "");
            }
        }

        public string formattedSubject
        {
            get
            {
                return Materia.ToUpper()[0] + Materia.Substring(1).ToLower();
            }
        }

        public string Data
        {
            get
            {
                return Convert.ToDateTime(datGiorno).ToString("dddd, dd MMMM yyyy");
            }
        }

        public Color markColor
        {
            get
            {
                return decValore < 6 ? Color.FromHex("#E37070") : Styles.TextColor;
            }
        }

        public bool dotsVisibility
        {
            get
            {
                return !string.IsNullOrEmpty(desCommento);
            }
        }
        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);


        void calculateMedia(List<CachedVoto> cache, Voti arg)
        {
            var votiDellaMateria = ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.Materia).ToList();

            for (int i = votiDellaMateria.Count - 1; i >= 0; i--)
            {
                foreach (var cobj in cache)
                {
                    if (votiDellaMateria[i].decValore == cobj.decValore && votiDellaMateria[i].Materia == cobj.Materia && votiDellaMateria[i].datGiorno == cobj.datGiorno)
                    {
                        votiDellaMateria.RemoveAt(i);
                        break;
                    }
                }
            }

            var media = votiDellaMateria.Sum(x => x.decValore) / votiDellaMateria.Count();
            ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.Materia).Media = (double)media;
        }

        [JsonIgnore]
        public ICommand FaMedia => new Command<Voti>((arg) =>
        {
            if (!arg.NonFaMedia)
            {
                ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.Materia).ToList()[ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.Materia).ToList().IndexOf(arg)].NonFaMedia = true;

                if (Barrel.Current.Exists("NoCountVoti"))
                {
                    var cache = CacheHelper.GetCache<List<CachedVoto>>("NoCountVoti");
                    cache.Add(new CachedVoto { datGiorno = arg.datGiorno, decValore = arg.decValore, Materia = arg.Materia });
                    Barrel.Current.Add("NoCountVoti", cache, TimeSpan.FromDays(365));
                    calculateMedia(cache, arg);
                }
                else
                {
                    Barrel.Current.Add("NoCountVoti", new List<CachedVoto> { new CachedVoto { datGiorno = arg.datGiorno, decValore = arg.decValore, Materia = arg.Materia } }, TimeSpan.FromDays(365));
                    var cache = CacheHelper.GetCache<List<CachedVoto>>("NoCountVoti");
                    calculateMedia(cache, arg);
                }
            }
            else
            {
                ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.Materia).ToList()[ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.Materia).ToList().IndexOf(arg)].NonFaMedia = false;

                if (Barrel.Current.Exists("NoCountVoti"))
                {
                    var cache = CacheHelper.GetCache<List<CachedVoto>>("NoCountVoti");
                    cache.RemoveAll(x => x.datGiorno == arg.datGiorno && x.decValore == arg.decValore && x.Materia == arg.Materia);
                    Barrel.Current.Add("NoCountVoti", cache, TimeSpan.FromDays(365));
                    calculateMedia(cache, arg);
                }
            }
        });
    }

    public class GroupedVoti : ObservableCollection<Voti>, INotifyPropertyChanged
    {
        public string Materia { get; set; }
        public ObservableCollection<Voti> Voti => this;

        private double media_;


        public event PropertyChangedEventHandler PropertyChanged;

        //[JsonConstructor]
        internal GroupedVoti(double media)
        {
            this.media_ = media;
        }

        public GroupedVoti()
        {

        }


        public double Media
        {
            get { return media_; }
            set
            {
                media_ = value;
                OnPropertyChanged("Media");
                calcTotalMedia();
            }
        }

        public static void calcTotalMedia()
        {
            //Calculate total media
            double media = 0;
            int toSkip = 0;
            foreach (var a in ArgoPages.Voti.GroupedVoti)
            {
                if (a.Media.ToString() == "NaN")
                {
                    toSkip++;
                    continue;
                }
                media += a.Media;
            }

            MessagingCenter.Send((App)Application.Current, "TotalMediaChanged", media / (ArgoPages.Voti.GroupedVoti.Count() - toSkip));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool chartVisibility
        {
            get
            {
                return Voti.Count >= 2;
            }
        }

    }

    //Pentagono
    public class Pentagono
    {
        public string Materia { get; set; }
        public double Media { get; set; }
    }

    //Voti che non fanno media
    public class CachedVoto
    {
        public string datGiorno { get; set; }
        public string Materia { get; set; }
        public double? decValore { get; set; }
    }

}

