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
    
    public partial class OggettiSondaggi
    {
        public int id { get; set; }
        public string Nome { get; set; }
        public string Immagine { get; set; }
        public int idSondaggio { get; set; }
    
        public virtual Sondaggi Sondaggi { internal get; set; }
    }
}
