using System;
using System.Collections.Generic;

namespace SalveminiApp.RestApi.Models
{
    public class WholeModel
    {
        public List<Argomenti> argomenti { get; set; }
        public List<Voti> voti { get; set; }
        public List<Compiti> compiti { get; set; }
        public List<Assenza> assenze { get; set; }
        public List<Promemoria> promemoria { get; set; }
        public List<Bacheca> bacheca { get; set; }

    }
}