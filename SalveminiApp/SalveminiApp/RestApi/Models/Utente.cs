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

        public string nomeCognome
        {
            get
            {
                return Nome + " " + Cognome;
            }
        }
        public string classeCorso
        {
            get
            {
                return Classe + Corso;
            }
        }

        public string Immagine
        {
            get
            {
                return Costants.Uri("images/users/") + id;
            }
        }
    }
}
