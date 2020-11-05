using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Commenti
    {
        public int Id { get; set; }
        public string Commento { get; set; }
        public DateTime Creazione { get; set; }
        public int IdUtente { get; set; }
        public int IdPost { get; set; }
        public bool Anonimo { get; set; }

        public virtual Domande IdPostNavigation {internal get; set; }
        public virtual Utenti IdUtenteNavigation { get; set; }
    }
}
