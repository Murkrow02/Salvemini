using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class OggettiSondaggi
    {
        public OggettiSondaggi()
        {
            VotiSondaggi = new HashSet<VotiSondaggi>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Immagine { get; set; }
        public int IdSondaggio { get; set; }

        public virtual Sondaggi IdSondaggioNavigation { get; set; }
        public virtual ICollection<VotiSondaggi> VotiSondaggi { get; set; }
    }
}
