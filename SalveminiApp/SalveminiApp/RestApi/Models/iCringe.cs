using System;
using System.Collections.Generic;

namespace SalveminiApp.RestApi.Models
{

    public class DomandeReturn
    {
        public int id { get; set; }
        public string Domanda { get; set; }
        public TimeSpan Data { get; set; }
        public int CommentiCount { get; set; }
        public Utenti Utente { get; set; }
        public List<Commenti> Commenti { get; set; }

        public string Elapsed
        {
            get
            {
                //Anni fa
                if (Data.Days > 365)
                    return (Data.Days / 365).ToString() + " anni fa";

                //Mesi
                if (Data.Days > 30)
                    return (Data.Days / 30).ToString() + " mesi fa";

                //Giorni
                if (Data.Days >= 7)
                    return (Data.Days / 7).ToString() + " settimane fa";

                //Giorni
                if (Data.Days > 1)
                    return (Data.Days).ToString() + " giorni fa";

                //Ore
                if (Data.Hours > 1)
                    return (Data.Hours).ToString() + " ore fa";

                //Minuti
                if (Data.Minutes > 1)
                    return (Data.Minutes).ToString() + " minuti fa";

                //Minuti
                if (Data.Seconds > 1)
                    return (Data.Seconds).ToString() + " secondi fa";

                return "";
            }
        }

        public string CommentiString
        {
            get
            {
                return CommentiCount.ToString() + " commenti";
            }
        }
        }

    public class Commenti
    {
        public int id { get; set; }
        public string Commento { get; set; }
        public System.DateTime Creazione { get; set; }
        public int idUtente { get; set; }
        public int idPost { get; set; }
        public bool Anonimo { get; set; }
    }

}
