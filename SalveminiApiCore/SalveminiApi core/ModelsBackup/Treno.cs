using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Models
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

        public DateTime LeaveTime
        {
            get
            {
                var a = Partenza.Split(':');
                return new DateTime(1, 1, 1, Convert.ToInt32(a[0]), Convert.ToInt32(a[1]), 0);
            }
        }
    }
}