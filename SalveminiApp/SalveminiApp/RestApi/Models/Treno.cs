using System;
namespace SalveminiApp.RestApi.Models
{
    public class Treno
    {
        public string Partenza { get; set; }

        //(D = Diretto, DD = Direttissimo)
        public string Importanza { get; set; }

        //(FER = Feriale, FES = Festivo)
        public string Variazioni { get; set; }

        //(0 = Sorrento)
        public int Stazione { get; set; }

        public DateTime LeaveTime
        {
            get
            {
                var a = Partenza.Split(':');
                return new DateTime(0, 0, 0, Convert.ToInt32(a[0]), Convert.ToInt32(a[1], 0), 0);
            }
        }
    }
}
