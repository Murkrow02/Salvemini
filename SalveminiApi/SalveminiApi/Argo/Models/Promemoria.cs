using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{
    public class Promemoria
    {
        public string desAnnotazioni { get; set; }
        public string datGiorno { get; set; }
        public string desMittente { get; set; }
        //public string codMin { get; set; }
        //public int numAnno { get; set; }
        //public int prgProgressivo { get; set; }
        //public int prgClasse { get; set; }
        //public int prgAnagrafe { get; set; }
        //public int prgScuola { get; set; }
    }

    public class promemoriaList
    {
        public List<Promemoria> dati { get; set; }
    }
}