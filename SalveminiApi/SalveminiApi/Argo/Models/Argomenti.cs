using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{
    public class Argomenti
    {
        public string datGiorno { get; set; }
        public string desMateria { get; set; }
        public string desArgomento { get; set; }
        public string docente { get; set; }
        //public string codMin { get; set; }
        //public int numAnno { get; set; }
        //public int prgMateria { get; set; }
        //public int prgClasse { get; set; }
        //public int prgScuola { get; set; }

      public string Materia
        {
            get
            {
                return Helpers.Utility.FirstCharToUpper(desMateria.ToLower());
            }
        }
        
    }
    public class argomentiList
    {
        public List<Argomenti> dati { get; set; }
    }
}
