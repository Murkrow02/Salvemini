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
    
    public partial class CoinSpese
    {
        public int id { get; set; }
        public string Tipo { get; set; }
        public Nullable<int> idOggetto { get; set; }
        public System.DateTime Attivazione { get; set; }
        public int idUtente { get; set; }
        public int Quantità { get; set; }
        public string Descrizione { get; set; }
    
        public virtual Utenti Utenti { internal get; set; }
    }
}