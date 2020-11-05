using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class SalveminiCard
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string Immagine { get; set; }
    }
}
