using System;
namespace SalveminiAppIntentUI.RestApi.Models
{
    public class Treno
    {
        public string Partenza { get; set; }

        //(D = Diretto, DD = Direttissimo)
        public string Importanza { get; set; }

        public string convertPriority
        {
            get
            {
                var value = "DIRETTO";
                switch (Importanza)
                {
                    case "D":
                        value = "DIRETTO";
                        break;

                    case "DD":
                        value = "DIRETTISSIMO";
                        break;
                }
                return value;
            }
        }
    }
}
