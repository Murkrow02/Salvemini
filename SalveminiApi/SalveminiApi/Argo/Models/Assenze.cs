using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{
    public class Assenze
    {
        public string codEvento { get; set; }
        public string datGiustificazione { get; set; }
        public string binUid { get; set; }
        public string codMin { get; set; }
        public string datAssenza { get; set; }
        public string giustificataDa { get; set; }
        public string desAssenza { get; set; }
        public string registrataDa { get; set; }
        public string oraAssenza { get; set; }
        public int? numOra { get; set; }
        public int prgScuola { get; set; }
        public int prgScheda { get; set; }
        public int numAnno { get; set; }
        public int prgAlunno { get; set; }
        public bool flgDaGiustificare { get; set; }

        
    }

   public class AssenzeList
    {
        public List<Assenze> dati { get; set; }

    }
}
