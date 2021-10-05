using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Argo.Models
{
    public class AuthBlock
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class AuthResponse
    {
        public string token { get; set; }
        public string disclaimer { get; set; }
        public string tipoUtente { get; set; }
    }

    public class changeBlock
    {
        public string vecchiaPassword { get; set; }
        public string nuovaPassword { get; set; }
    }
}