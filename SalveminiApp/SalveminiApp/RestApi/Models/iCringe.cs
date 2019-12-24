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
                    return (Data.Days / 365).ToString() + (Data.Days / 365 == 1 ? " anno fa" : " anni fa");

                //Mesi
                if (Data.Days > 30)
                    return (Data.Days / 30).ToString() + (Data.Days / 30 == 1 ? " mese fa" : " mesi fa");

                //Giorni
                if (Data.Days >= 7)
                    return (Data.Days / 7).ToString() + (Data.Days / 7 == 1 ? " settimana fa" : " settimane fa");

                //Giorni
                if (Data.Days > 1)
                    return (Data.Days).ToString() + (Data.Days == 1 ? " giorno fa" : " giorni fa");

                //Ore
                if (Data.Hours > 1)
                    return (Data.Hours).ToString() + (Data.Hours == 1 ? " ora fa" : " ore fa");

                //Minuti
                if (Data.Minutes > 1)
                    return (Data.Minutes).ToString() + (Data.Minutes == 1 ? " minuto fa" : " minuti fa");

                //Secondi
                if (Data.Seconds > 1)
                    return (Data.Seconds).ToString() + (Data.Seconds == 1 ? " secondo fa" : " secondi fa");

                return "";
            }
        }

        //Commenti totali
        public string CommentiString
        {
            get
            {
                var conto = CommentiCount.ToString();
                return CommentiCount == 1 ? conto + " commento" : conto + " commenti";
            }
        }

        //Quanti altri commenti?
        public string AltriCommentiString
        {
            get
            {
                //Solo un altro
                if (CommentiCount == 3)
                    return "Vedi un altro commento";
                //Più di 1
                else if (CommentiCount > 3)
                    return "Vedi altri " + CommentiCount.ToString() + " commenti";
                else //Niente
                    return "";
            }
        }

        //Altri commenti da mostrare?
        public bool AltriCommentiVisible
        {
            get
            {
                return CommentiCount > 2;
            }
        }

        //Mostra primo commento?
        public bool FirstCommentVisible
        {
            get
            {
                return CommentiCount > 0;
            }
        }

        //Mostra secondo commento?
        public bool SecondCommentVisible
        {
            get
            {
                return CommentiCount > 1;
            }
        }

        //Primo Commento
        public Commenti FirstComment
        {
            get
            {
                if (CommentiCount > 0)
                    return Commenti[0];
                else
                    return null;
            }
        }

        //Secondo Commento
        public Commenti SecondComment
        {
            get
            {
                if (CommentiCount > 1)
                    return Commenti[1];
                else
                    return null;
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
        public Utenti Utenti { get; set; }
        public bool Anonimo { get; set; }

        public string UserName
        {
            get
            {
                return Utenti != null ? Utenti.nomeCognome : "Anonimo";
            }
        }

        public string UserImage
        {
            get
            {
                return Utenti != null ? Utenti.fullImmagine : "Anonimo";
            }
        }
    }

    public class CommentiReturn
    {
        public string Domanda { get; set; }
        public List<Commenti> Commenti { get; set; }
    }

    public class Domande
    {
        public int id { get; set; }
        public string Domanda { get; set; }
        public bool Approvata { get; set; }
        public int idUtente { get; set; }
        public int Impressions { get; set; }
        public System.DateTime Creazione { get; set; }
        public bool Anonimo { get; set; }
    }

    public class Notifiche
    {
        public int id { get; set; }
        public string Descrizione { get; set; }
        public System.DateTime Creazione { get; set; }
        public int idPost { get; set; }
        public int idUtente { get; set; }
        public int Tipo { get; set; }

        public string Icon {
            get
            {
                switch (Tipo)
                {
                    case 0: //Respinta
                        return "fal-times";
                    case 1: //Approvata
                        return "fal-check";
                    case 2: //Commento
                        return "fal-comment";

                    default: //Boh
                        return "";
                }
                    
            }
        }
    }

}
