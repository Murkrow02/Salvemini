using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Argomenti
    {
        public string datGiorno { get; set; }
        public string desArgomento { get; set; }
        public string docente { get; set; }
        public string codMin { get; set; }
        public int numAnno { get; set; }
        public int prgMateria { get; set; }
        public string desMateria { get; set; }
        public int prgClasse { get; set; }
        public int prgScuola { get; set; }
        public string Data { get; set; }
        public string Materia { get; set; }



        public string formattedTeacher
        {
            get
            {
                return docente.Substring(7).Replace(")", "");
            }
        }

        public string Contenuto
        {
            get
            {
                return desArgomento;
            }
        }


        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);
    }
}
