using System;
using System.Collections.Generic;

namespace SalveminiApp.RestApi.Models
{
    public class Index
    {
        public decimal AppVersion { get; set; }
        public Avvisi ultimoAvviso { get; set; }
        public List<WholeModel> Oggi { get; set; }
        public List<Ad> Ads { get; set; }
        public bool ArgoAuth { get; set; }
    }
}
