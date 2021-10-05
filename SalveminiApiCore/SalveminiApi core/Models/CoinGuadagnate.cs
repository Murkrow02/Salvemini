using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class CoinGuadagnate
    {
        public int Id { get; set; }
        public string Descrizione { get; set; }
        public int Quantità { get; set; }
        public int IdUtente { get; set; }
        public DateTime Attivazione { get; set; }

        public virtual Utenti IdUtenteNavigation { get; set; }
    }
}
