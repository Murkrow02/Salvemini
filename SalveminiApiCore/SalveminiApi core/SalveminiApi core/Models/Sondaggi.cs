using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Sondaggi
    {
        public Sondaggi()
        {
            OggettiSondaggi = new HashSet<OggettiSondaggi>();
            VotiSondaggi = new HashSet<VotiSondaggi>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public int Creatore { get; set; }
        public DateTime Creazione { get; set; }
        public bool? Attivo { get; set; }

        public virtual ICollection<OggettiSondaggi> OggettiSondaggi { get; set; }
        public virtual ICollection<VotiSondaggi> VotiSondaggi { get; set; }
    }
}
