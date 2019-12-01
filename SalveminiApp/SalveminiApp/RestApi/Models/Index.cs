using System;
using System.Collections.Generic;

namespace SalveminiApp.RestApi.Models
{
    public class Index
    {
        public decimal AppVersion { get; set; }
        public int OrarioTrasportiVersion { get; set; }
        public Avvisi ultimoAvviso { get; set; }
        public Sondaggi ultimoSondaggio { get; set; }
        public List<WholeModel> Oggi { get; set; }
        public List<Ad> Ads { get; set; }
        public Giornalino Giornalino { get; set; }
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
