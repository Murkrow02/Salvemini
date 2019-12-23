using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Models
{
    public class DomandeReturn
    {
        public int id { get; set; }
        public string Domanda { get; set; }
        public DateTime Data { get; set; }
        public int CommentiCount { get; set; }
        public List<Commenti> Commenti {get;set;}
    }
}