using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Essentials;

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

        public string UserImage
        {
            get
            {
                return Utente != null ? Utente.fullImmagine : "Anonimo";
            }
        }

        public bool canDelete
        {
            get
            {
                if (SecondaryViews.Profile.superVip_)
                    return true;
                if (Utente == null)
                    return false;
                return Utente.id == Preferences.Get("UserId", 0);
            }
        }

        public string UserName
        {
            get
            {
                return Utente != null ? Utente.nomeCognome : "Anonimo";
            }
        }

        public string Elapsed
        {
            get
            {
                return Costants.SpanString(Data);
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
                    return "Vedi altri " + (CommentiCount - 2).ToString() + " commenti";
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
        //public int idUtente { get; set; }
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

        public string Elapsed
        {
            get
            {
                return Costants.SpanString(DateTime.Now - Creazione);
            }
        }

        public bool canDelete
        {
            get
            {
                if (SecondaryViews.Profile.superVip_)
                    return true;
                if (Utenti == null)
                    return false;
                return Utenti.id == Preferences.Get("UserId", 0);
            }
        }
    }

    public class CommentiReturn
    {
        public string Domanda { get; set; }
        //public List<Commenti> Commenti { get; set; }
        public ObservableCollection<Commenti> Commenti { get; set; }
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

        public string Icon
        {
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

        public string Elapsed
        {
            get
            {
                return Costants.SpanString(DateTime.Now - Creazione);
            }
        }
    }

}
