using System;
namespace SalveminiApp.RestApi.Models
{
    public class Sondaggi
    {
        public string Nome { get; set; }
        public int Creatore { get; set; }
        public DateTime Creazione { get; set; }
    }

    public class VotoSondaggio
    {
        public int idSondaggio { get; set; }
        public int Utente { get; set; }
        public int Voto { get; set; }
    }

    public class OggettiSondaggi
    {
        public int id { get; set; }
        public string Nome { get; set; }
        public string Immagine { get; set; }
    }

}
