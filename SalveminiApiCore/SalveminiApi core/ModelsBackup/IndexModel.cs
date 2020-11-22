using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Models
{
    public class IndexModel
    {
        public decimal AppVersion { get; set; }
       public int OrarioTrasportiVersion { get; set; }
        public Avvisi ultimoAvviso { get; set; }
        public Sondaggi ultimoSondaggio { get; set; }
        public List<Ads> Ads { get; set; }
        public Giornalino Giornalino { get; set; }
        public int Authorized { get; set; }
        public bool VotedSondaggio { get; set; }
        public int sCoin { get; set; }
        public string Classe { get; set; }
        public string Corso { get; set; }
    }

    public class IndexArgo
    {
        public int NotizieCount { get; set; }
        public string UltimaNotizia { get; set; }
        public string TipoNotizia { get; set; }
    }
}