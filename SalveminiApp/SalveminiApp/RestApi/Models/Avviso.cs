using System;
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
    }
}
