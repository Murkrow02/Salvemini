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
    
    public partial class VotiSondaggi
    {
        public int id { get; set; }
        public int idSondaggio { get; set; }
        public int Utente { get; set; }
        public int Voto { get; set; }
    
        public virtual Sondaggi Sondaggi { internal get; set; }
        public virtual Utenti Utenti { internal get; set; }
        public virtual OggettiSondaggi OggettiSondaggi { internal get; set; }
    }
}
