using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class CoinSpese
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public int? IdOggetto { get; set; }
        public DateTime Attivazione { get; set; }
        public int IdUtente { get; set; }
        public int Quantità { get; set; }
        public string Descrizione { get; set; }

        public virtual Utenti IdUtenteNavigation { get; set; }
    }
}
