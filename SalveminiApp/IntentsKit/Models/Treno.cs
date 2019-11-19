using System;
namespace IntentsKit.Models
{
    public class Treno
    {
        public string Partenza { get; set; }

        //(D = Diretto, DD = Direttissimo)
        public string Importanza { get; set; }

        //(FER = Feriale, FES = Festivo)
        public string Variazioni { get; set; } = "";

        //(0 = Sorrento, 1 = Sant'Agnello)
        public int Stazione { get; set; }

        //(true = Napoli, false = Sorrento)
        public bool Direzione { get; set; }

        public string DirectionString
        {
            get
            {
                return Direzione ? "Napoli" : "Sorrento";
            }
        }
    }
}
