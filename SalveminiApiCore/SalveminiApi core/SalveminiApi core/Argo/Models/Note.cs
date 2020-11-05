using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Argo.Models
{
    public class Note
    {
       
       public string flgVisualizzata { get; set; }
        public string datNota { get; set; }
        public int prgAnagrafe { get; set; }
        public int prgNota { get; set; }
        public DateTime oraNota { get; set; }
        public string desNota { get; set; }
        public string docente { get; set; }
    }

    public class noteList
    {
        public List<Note> dati { get; set; }
    }
}