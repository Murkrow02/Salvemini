using System;
using System.Collections.Generic;
using System.Linq;

namespace SalveminiApp.RestApi.Models
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
        public string Materia { get; set; }
    }

    public class GroupedVoti
    {
        public string Materia { get; set; }
        public double Media { get; set; }
        public List<Voti> Voti { get; set; }
    }

    public class VotiList
    {
        public List<Voti> dati { get; set; }
    }


    //Pentagono
    public class Pentagono
    {
        public string Materia { get; set; }
        public double Media { get; set; }
    }
}

