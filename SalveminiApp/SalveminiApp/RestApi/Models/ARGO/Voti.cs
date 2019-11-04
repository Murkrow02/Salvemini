using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MonkeyCache.SQLite;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Voti : INotifyPropertyChanged
    {
        private bool nonFaMedia_;

        public event PropertyChangedEventHandler PropertyChanged;

        public Voti(bool nonfamedia)
        {
            this.nonFaMedia_ = nonfamedia;
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
        public string desMateria { get; set; }
        public string codVotoPratico { get; set; }
        public string desProva { get; set; }
        public string codVoto { get; set; }
        public string desCommento { get; set; }
        public string docente { get; set; }
        public int prgMateria { get; set; }
        public double? decValore { get; set; }
        public string Materia { get; set; }


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
                return desMateria.ToUpper()[0] + desMateria.Substring(1).ToLower();
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

        public ICommand FaMedia => new Command<Voti>((arg) =>
        {
            if (!arg.NonFaMedia)
            {
                ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.desMateria).ToList()[ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.desMateria).ToList().IndexOf(arg)].NonFaMedia = true;

                if (Barrel.Current.Exists("NoCountVoti"))
                {
                    var cache = Barrel.Current.Get<List<CachedVoto>>("NoCountVoti");
                    cache.Add(new CachedVoto { datGiorno = arg.datGiorno, decValore = arg.decValore, desMateria = arg.desMateria });
                    Barrel.Current.Add("NoCountVoti", cache, TimeSpan.FromDays(365));
                }
                else
                {
                    Barrel.Current.Add("NoCountVoti", new List<CachedVoto> { new CachedVoto { datGiorno = arg.datGiorno, decValore = arg.decValore, desMateria = arg.desMateria } }, TimeSpan.FromDays(365));
                }
            }
            else
            {
                ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.desMateria).ToList()[ArgoPages.Voti.GroupedVoti.FirstOrDefault(x => x.Materia == arg.desMateria).ToList().IndexOf(arg)].NonFaMedia = false;

                if (Barrel.Current.Exists("NoCountVoti"))
                {
                    var cache = Barrel.Current.Get<List<CachedVoto>>("NoCountVoti");
                    cache.RemoveAll(x => x.datGiorno == arg.datGiorno && x.decValore == arg.decValore && x.desMateria == arg.desMateria);
                    Barrel.Current.Add("NoCountVoti", cache, TimeSpan.FromDays(365));
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

        public GroupedVoti(double media)
        {
            this.media_ = media;
        }


        public double Media
        {
            get { return media_; }
            set
            {
                media_ = value;
                OnPropertyChanged("Media");
            }
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
        public string desMateria { get; set; }
        public double? decValore { get; set; }
    }
}

