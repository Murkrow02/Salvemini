using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
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
}