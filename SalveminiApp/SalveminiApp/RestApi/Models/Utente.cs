using System;
namespace SalveminiApp.RestApi.Models
{
    public class Utente
    {
        public int id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Sesso { get; set; }
        public int Classe { get; set; }
        public string Corso { get; set; }
        public int Stato { get; set; }
        public string Creazione { get; set; }
        public string ArgoToken { get; set; }
        public int sCoin { get; set; }
        public string Residenza { get; set; }
        public DateTime Compleanno { get; set; }
        public string Immagine { get; set; }

        public string nomeCognome
        {
            get
            {
                return Nome + " " + Cognome;
            }
        }

        public string cognomeNome
        {
            get
            {
                return Cognome + " " + Nome;
            }
        }

        public string classeCorso
        {
            get
            {
                return Classe + Corso;
            }
        }

        public string fullImmagine
        {
            get
            {
                return Costants.Uri("images/users/") + Immagine;
            }
        }

        public string StatoLabel
        {
            get
            {
                switch (Stato)
                {
                    case 0:
                        return "Plebeo";
                    case 1:
                        return "Rappr.";
                    case 2:
                        return "VIP";
                    case 3:
                        return "Super VIP";
                    case -1:
                        return "Disab.";
                    default:
                        return "null";
                }
            }
        }
    }
}
