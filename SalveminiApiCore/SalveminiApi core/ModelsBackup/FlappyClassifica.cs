using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class FlappyClassifica
    {
        public int IdUtente { get; set; }
        public int Punteggio { get; set; }

        public virtual Utenti IdUtenteNavigation { get; set; }
    }
}
