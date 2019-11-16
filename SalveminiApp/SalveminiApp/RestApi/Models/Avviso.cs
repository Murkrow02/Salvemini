using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SalveminiApp.RestApi.Models
{
    public class Avvisi
    {
        public int id { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public string Immagini { get; set; }
        public int idCreatore { get; set; }
        public DateTime Creazione { get; set; }
        public bool SendNotification { get; set; }
        public Utente Utenti { get; set; } 

        public List<string> FullImmagini
        {
            get
            {
                var lista = new List<string>();

                if (!string.IsNullOrEmpty(Immagini))
                {
                    foreach (string image in Immagini.Split(","))
                    {
                        lista.Add(Costants.Uri("images/avvisi/").ToString() + image);
                    }
                }
                return lista;
            }
        }

        [JsonIgnore]
        public bool ShowMoreVisibility
        {
            get
            {
                try { return Descrizione.Length > 100; } catch { return false; }
            }
        }

        [JsonIgnore]
        public bool ListVisibility
        {
            get
            {
                try { return FullImmagini != null && FullImmagini.Count > 0; } catch { return false; }             
            }
        }

        //Description to display
        [JsonIgnore]
        public string CorrectDescription
        {
            get
            {
                int cutAt = 100; //Start cutting string at 100 char
                if (string.IsNullOrEmpty(Immagini)) cutAt = 500; //More characters if no images
                try { return Descrizione.Length > cutAt ? Descrizione.Remove(cutAt) + "..." : Descrizione; } catch { return ""; }
                
            }
        }

        [JsonIgnore]
        public string nomeCreatore
        {
            get
            {
                try { return "Creato da " + Utenti.nomeCognome; } catch { return ""; }
            }
        }
    }
}
