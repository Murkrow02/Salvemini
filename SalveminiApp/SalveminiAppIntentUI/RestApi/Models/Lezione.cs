using System;
namespace SalveminiAppIntentUI.RestApi.Models
{
    public class Lezione
    {
        public int Giorno { get; set; }
        public int Ora { get; set; }
        public string Materia { get; set; }
        public string Sede { get; set; }

        //Internal Parameters
        public int numOre { get; set; }
        public bool toRemove { get; set; }
    }
}
