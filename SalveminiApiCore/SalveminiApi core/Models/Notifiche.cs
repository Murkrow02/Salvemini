using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Notifiche
    {
        public int Id { get; set; }
        public string Descrizione { get; set; }
        public DateTime Creazione { get; set; }
        public int IdPost { get; set; }
        public int IdUtente { get; set; }
        public int Tipo { get; set; }

        public virtual Utenti IdUtenteNavigation { get; set; }
    }
}
