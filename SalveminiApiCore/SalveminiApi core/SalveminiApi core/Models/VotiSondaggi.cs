using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class VotiSondaggi
    {
        public int Id { get; set; }
        public int IdSondaggio { get; set; }
        public int Utente { get; set; }
        public int Voto { get; set; }

        public virtual Sondaggi IdSondaggioNavigation { get; set; }
        public virtual Utenti UtenteNavigation { get; set; }
        public virtual OggettiSondaggi VotoNavigation { get; set; }
    }
}
