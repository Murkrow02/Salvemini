using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Models
{
    public class FlappySkinReturn
    {
        public int id { get; set; }
        public string Nome { get; set; }
        public List<string> Immagini { get; set; }
        public int Costo { get; set; }
        public bool Comprata { get; set; }
    }
    public class FlappyMoneteReturn
    {
        public int id { get; set; }
        public string Descrizione { get; set; }
        public int Costo { get; set; }
        public int Valore { get; set; } 
        public bool Comprata { get; set; }
    }

}