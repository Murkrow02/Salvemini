using System;
namespace SalveminiApp.RestApi.Models
{
    public class Ad
    {
        public int id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string Immagine { get; set; }
        public string Url { get; set; }
        public int Tipo { get; set; }

        public string FullImmagine
        {
            get
            {
                return Costants.Uri("images/ads/") + Immagine;
            }
        }
    }
}
