using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Ads
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string Url { get; set; }
        public string Immagine { get; set; }
        public int Tipo { get; set; }
        public int? Impressions { get; set; }
    }
}
