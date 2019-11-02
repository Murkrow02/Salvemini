using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace SalveminiApp.RestApi.Models
{
    public class Bacheca
    {
        public string prgMessaggio { get; set; }
        public string desOggetto { get; set; }
        public bool adesione { get; set; }
        public bool richiediAd { get; set; }
        public bool presaVisione { get; set; }
        public bool richiediPv { get; set; }
        public string desUrl { get; set; }
        public string desMessaggio { get; set; }
        public List<Allegati> Allegati { get; set; }

        public string formattedTitle
        {
            get
            {
                return desOggetto.ToUpper()[0] + desOggetto.Substring(1).ToLower();
            }
        }
    }

    public class Allegati
    {
        public int prgAllegato { get; set; }
        public int prgMessaggio { get; set; }
        public string desFile { get; set; }
        public int numAnno { get; set; }
        public string codMin { get; set; }
        public string urlMessaggio { get; set; }
        public string fullUrl
        {
            get
            {
                return urlMessaggio.Replace("{token}", Preferences.Get("Token", "").Replace("-",""));
            }
        }
    }

    public class VisualizzaBacheca
    {
        public int prgMessaggio { get; set; }
        public bool presaVisione { get; set; }
    }
}
