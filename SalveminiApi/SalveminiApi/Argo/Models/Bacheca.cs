using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
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
        public List<Allegati> allegati { get; set; }
    }
}