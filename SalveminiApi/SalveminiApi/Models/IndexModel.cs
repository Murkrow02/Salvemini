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
        public Avvisi ultimoAvviso { get; set; }
        public List<WholeModel> Oggi { get; set; }
        public List<Ads> Ads { get; set; }
        public bool ArgoAuth { get; set; }
    }
}