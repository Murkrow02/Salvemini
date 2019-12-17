using System;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Evento
    {
        public int Codice { get; set; }
        public decimal xAttivazione { get; set; }
        public decimal yAttivazione { get; set; }
        public decimal Raggio { get; set; }
        public int idCreatore { get; set; }
        public int Valore { get; set; }
        public DateTime Creazione { get; set; }
        public string Nome { get; set; }
        public bool Attivo { get; set; }

        public string AttivoString { get { return Attivo ? "Attivo" : "Disattivo"; } }
    }

    public class PostCode
    {
        public decimal xPosition { get; set; }
        public decimal yPosition { get; set; }
        public int Codice { get; set; }
    }

    public class Transaction
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public Color color
        {
            get
            {
                //Return red if negative or green if positive
                return Amount >= 0 ? Color.FromHex("3ae040") : Color.FromHex("f22e2e");
            }
        }

        public string formattedDate
        {
            get
            {
                return Date.ToString("dddd, dd MMMM yyyy");
            }
        }
    }
}
