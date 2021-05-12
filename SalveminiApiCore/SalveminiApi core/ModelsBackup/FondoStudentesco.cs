using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class FondoStudentesco
    {
        public long Id { get; set; }
        public string Causa { get; set; }
        public DateTime Data { get; set; }
        public decimal Importo { get; set; }
    }
}
