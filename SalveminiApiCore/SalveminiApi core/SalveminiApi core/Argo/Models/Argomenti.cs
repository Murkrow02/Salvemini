using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Argo.Models
{
    public class Argomenti
    {
        public string datGiorno { get; set; }
        public string desMateria { get; set; }
        public string desArgomento { get; set; }
        public string docente { get; set; }

      public string Materia
        {
            get
            {
                return Costants.ShortSubject(desMateria);
            }
        }
        
    }
    public class argomentiList
    {
        public List<Argomenti> dati { get; set; }
    }
}
