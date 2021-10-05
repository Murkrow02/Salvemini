using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Avvisi
    {
        public int Id { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public string Immagini { get; set; }
        public int IdCreatore { get; set; }
        public DateTime Creazione { get; set; }
        public bool SendNotification { get; set; }

        public virtual Utenti IdCreatoreNavigation { get; set; }
    }
}
