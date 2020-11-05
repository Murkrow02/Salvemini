using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Argo.Models
{
    public class Utente
    {
        public string desScuola { get; set; }

        public string desDenominazione { get; set; }

        public string desCorso { get; set; }

        public AlunnoInfo alunno { get; set; }

        public int prgAlunno { get; set; }

        public int prgScheda { get; set; }

        public int prgScuola { get; set; }

    }

    public class AlunnoInfo
    {
        public string desCf { get; set; }

        public string desCognome { get; set; }

        public string desVia { get; set; }

        public string desCap { get; set; }

        public string desNome { get; set; }

        public string desCellulare { get; set; }

        public string desComuneNascita { get; set; }

        public string flgSesso { get; set; }

        public string datNascita { get; set; }

        public string desIndirizzoRecapito { get; set; }

        public string desComuneRecapito { get; set; }

        public string desCapResidenza { get; set; }

        public string desComuneResidenza { get; set; }

        public string desTelefono { get; set; }

        public string desCittadinanza { get; set; }
    }
}