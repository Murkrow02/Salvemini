using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{
    public class Voti
    {
        public string datGiorno { get; set; }
        public string desMateria { get; set; }
        public string codVotoPratico { get; set; }
        public string desProva { get; set; }
        public string codVoto { get; set; }
        public string desCommento { get; set; }
        public string docente { get; set; }
        public int prgMateria { get; set; }
        public double? decValore { get; set; }

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

    public class GroupedVoti: ObservableCollection<Voti>
    {
        public string Materia { get; set; }
        public double Media { get; set; }

    }

    public class VotiList
    {
        public List<Voti> dati { get; set; }
    }


    //Pentagono
    public class Pentagono {
        public string Materia { get; set; }
        public double Media { get; set; }
    }
}