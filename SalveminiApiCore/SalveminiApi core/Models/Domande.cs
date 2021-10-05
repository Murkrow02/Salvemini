using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Domande
    {
        public Domande()
        {
            Commenti = new HashSet<Commenti>();
        }

        public int Id { get; set; }
        public string Domanda { get; set; }
        public bool Approvata { get; set; }
        public int IdUtente { get; set; }
        public int Impressions { get; set; }
        public DateTime Creazione { get; set; }
        public bool Anonimo { get; set; }

        public virtual ICollection<Commenti> Commenti {internal get; set; }
    }
}
