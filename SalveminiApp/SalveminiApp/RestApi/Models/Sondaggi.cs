using System;
using System.Collections.Generic;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Sondaggi
    {
        public int id { get; set; }
        public string Nome { get; set; }
        public int Creatore { get; set; }
        public DateTime Creazione { get; set; }
        public List<OggettiSondaggi> OggettiSondaggi { get; set; }
        public List<VotoSondaggio> VotiSondaggi { get; set; }
        public Utenti Utenti { get; set; }
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
        public string FullImmagine
        {
            get
            {
                return Costants.Uri("images/sondaggi/") + Immagine;
            }
        }
    }

    public class OggettiToUpload
    {
        public ImageSource imageSource { get; set; }
        public MediaFile mediaFile { get; set; }
        public string Nome { get; set; }
    }

    public class SondaggiResult
    {
        public string NomeOpzione { get; set; }
        public int Voti { get; set; }
        public int Percentuale { get; set; }
    }

}
