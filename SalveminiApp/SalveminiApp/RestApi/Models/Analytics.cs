using System;
namespace SalveminiApp.RestApi.Models
{
    public class Analytics
    {
        public string Tipo { get; set; }
        public DateTime Mese { get; set; }
        public int Valore { get; set; }

    }

    public class EventLog
    {
        public string Evento { get; set; }
        public DateTime Data { get; set; }
    }

}
