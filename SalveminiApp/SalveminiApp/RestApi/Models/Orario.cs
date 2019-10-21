using System;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Lezione
    {
        public int Giorno { get; set; }
        public int Ora { get; set; }
        public string Docente { get; set; }
        public string Materia { get; set; }
        public string Sede { get; set; }

        //Internal Parameters
        public int numOre { get; set; }
        public bool toRemove { get; set; }
        public double OrarioFrameHeight { get; set; }
        public double OrarioFrameRadius { get; set; }

        public Thickness OrarioFrameMargin
        {
            get
            {
                var value = new Thickness(10, 2);

                return value;
            }
        }

        public string Colore
        {
            get
            {
                return Costants.Colore[Materia];
            }
        }

        public string oraLezione
        {
            get
            {
                return Costants.Ore[Ora];
            }
        }
    }
}
