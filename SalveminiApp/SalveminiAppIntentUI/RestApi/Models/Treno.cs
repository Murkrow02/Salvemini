using System;
namespace SalveminiAppIntentUI.RestApi.Models
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

        public DateTime LeaveTime
        {
            get
            {
                var a = Partenza.Split(':');
                return new DateTime(1, 1, 1, Convert.ToInt32(a[0]), Convert.ToInt32(a[1]), 0);
            }
        }

        public string FullVariazioni
        {
            get
            {
                if (!string.IsNullOrEmpty(Variazioni))
                {
                    return Variazioni == "FER" ? "Feriale" : "Festivo";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
