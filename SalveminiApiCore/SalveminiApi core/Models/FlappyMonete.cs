using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class FlappyMonete
    {
        public int Id { get; set; }
        public int ValoreMonete { get; set; }
        public string Descrizione { get; set; }
        public int Costo { get; set; }
    }
}
