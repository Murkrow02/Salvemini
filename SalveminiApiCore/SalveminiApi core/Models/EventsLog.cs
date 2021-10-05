using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class EventsLog
    {
        public int Id { get; set; }
        public string Evento { get; set; }
        public DateTime Data { get; set; }
    }
}
