using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{
    public class Compiti
    {
        public string datGiorno { get; set; }
        public string desMateria { get; set; }
        public string desCompiti { get; set; }
        public string docente { get; set; }
        //public int numAnno { get; set; }
        //public string codMin { get; set; }
        //public int prgMateria { get; set; }
        //public int prgClasse { get; set; }
        //public int prgScuola { get; set; }
    }
    public class compitiList
        {
            public List<Compiti> dati { get; set; }
        }
    }
