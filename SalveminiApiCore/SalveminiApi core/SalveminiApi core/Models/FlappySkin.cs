using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class FlappySkin
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Immagini { get; set; }
        public int Costo { get; set; }
    }
}
