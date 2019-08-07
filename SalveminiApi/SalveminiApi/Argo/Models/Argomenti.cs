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
        public string codMin { get; set; }
        public int numAnno { get; set; }
        public int prgMateria { get; set; }
        public int prgClasse { get; set; }
        public int prgScuola { get; set; }

        public string Data
        {
            get
            {
                return Convert.ToDateTime(datGiorno).ToString("dd/MM/yyyy");
            }
        }

        public string Materia
        {
            get
            {
                if (desMateria == "LINGUA E CULTURA STRANIERA(INGLESE)")
                {
                    return "Inglese";
                }
                else if (desMateria == "LINGUA e LETTERATURA ITALIANA")
                {
                    return "Letteratura Italiana";
                }
                else if (desMateria == "LINGUA e CULTURA LATINA")
                {
                    return "Latino";
                }
                else if (desMateria == "SCIENZE NATURALI")
                {
                    return "Scienze Naturali";
                }
                else if (desMateria == "LINGUA E CULTURA STRANIERA(TEDESCO)")
                {
                    return "Tedesco";
                }
                else if (desMateria == "DISEGNO E STORIA DELL'ARTE")
                {
                    return "Arte";
                }
                else
                {
                    return desMateria.First().ToString().ToUpper() + desMateria.Substring(1).ToLower();
                }

            }
        }
    }
    public class argomentiList
    {
        public List<Argomenti> dati { get; set; }
    }
}
