//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SalveminiApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Domande
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Domande()
        {
            this.Commenti = new HashSet<Commenti>();
        }
    
        public int id { get; set; }
        public string Domanda { get; set; }
        public bool Approvata { get; set; }
        public int idUtente { get; set; }
        public int Impressions { get; set; }
        public System.DateTime Creazione { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Commenti> Commenti { internal get; set; }
    }
}
