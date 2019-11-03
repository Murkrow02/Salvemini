using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Models
{
    public class SondaggiResult
    {
        public string NomeOpzione { get; set; }
        public int Voti { get; set; }
        public int Percentuale { get; set; }
    }
}