using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Models
{
    public class DomandeReturn
    {
        public int id { get; set; }
        public string Domanda { get; set; }
        public string Data { get; set; }
        public int CommentiCount { get; set; }
        public Utenti Utente { get; set; }
        public List<Commenti> Commenti {get;set;}
    }

    public class CommentiReturn
    {
        public string Domanda { get; set; }
        public List<Commenti> Commenti { get; set; }
    }

    public class NewNotifiche
    {
        public int Tipo { get; set; }
        public string Count { get; set; }
    }
}