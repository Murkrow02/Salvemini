using System;
using System.Collections.Generic;

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

        public bool ShowMoreVisibility
        {
            get
            {
                return Descrizione.Length > 100;
            }
        }

        public bool ListVisibility
        {
            get
            {
                return FullImmagini != null && FullImmagini.Count > 0;
            }
        }

        //Description to display
        public string CorrectDescription
        {
            get
            {
                return Descrizione.Length > 0 ? Descrizione.Remove(100) + "..." : Descrizione;
            }
        }
    }
}
