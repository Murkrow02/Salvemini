using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SalveminiApi.Argo.Models;

namespace SalveminiApi.Models
{
    public class IndexModel
    {
        public decimal AppVersion { get; set; }
       public int OrarioTrasportiVersion { get; set; }
        public Avvisi ultimoAvviso { get; set; }
        public Sondaggi ultimoSondaggio { get; set; }
        public List<Ads> Ads { get; set; }
        public int Authorized { get; set; }
        public bool VotedSondaggio { get; set; }
        public int sCoin { get; set; }
    }

    public class IndexArgo
    {
        public int NotizieCount { get; set; }
        public string UltimaNotizia { get; set; }
        public string TipoNotizia { get; set; }
    }
}