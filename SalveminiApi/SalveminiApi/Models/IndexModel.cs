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
        public int OrariTreniVersion { get; set; }
        public int OrariBusVersion { get; set; }
        public int OrariAliVersion { get; set; }
        public int OrariScuolaVersion { get; set; }
        public Avvisi ultimoAvviso { get; set; }
        public Sondaggi ultimoSondaggio { get; set; }
        //public List<WholeModel> Oggi { get; set; }
        public List<Ads> Ads { get; set; }
        public bool Authorized { get; set; }
    }
}