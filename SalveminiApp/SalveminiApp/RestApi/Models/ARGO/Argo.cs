using System;
using System.Linq;

namespace SalveminiApp.RestApi.Models
{
    public class WholeModel
    {
        public string desCommento { get; set; }
        public string giorno { get; set; }
        public string tipo { get; set; }
        public string titolo { get; set; }
        public string codMin { get; set; }
        public string datGiorno { get; set; }
        public string codEvento { get; set; }
        public string desMateria { get; set; }
        public string desArgomento { get; set; }
        public string desCompiti { get; set; }
        public string docente { get; set; }
        public string datGiustificazione { get; set; }
        public string giustificataDa { get; set; }
        public string registrataDa { get; set; }
        public string binUid { get; set; }
        public string desAssenza { get; set; }
        public string desAnnotazioni { get; set; }
        public string desMittente { get; set; }
        public string codVoto { get; set; }
        public int numAnno { get; set; }
        public int prgAlunno { get; set; }
        public int prgScheda { get; set; }
        public int prgScuola { get; set; }
        public int ordine { get; set; }
        public int prgMateria { get; set; }
        public int prgClasse { get; set; }
        public double decValore { get; set; }
        public bool flgDaGiustificare { get; set; }
        public string datAssenza { get; set; }
    }
}
