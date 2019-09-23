using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Scrutinio
    {
        public int ordineMateria { get; set; }
        public string desMateria { get; set; }

        public string Voto { internal get; set; }
        public int prgMateria { internal get; set; }
        public int prgScuola { internal get; set; }
        public int prgScheda { internal get; set; }
        public bool votoUnico { get; set; }
        public int prgPeriodo { internal get; set; }
        public int? assenze { get; set; }
        public string codMin { internal get; set; }
        public string suddivisione { get; set; }
        public int numAnno { internal get; set; }
        public int prgAlunno { internal get; set; }
        public string giudizioSintetico { get; set; }
        public int prgClasse { get; set; }
        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);


    }

    public class ScrutinioGrouped
    {
        public List<Scrutinio> Primo { get; set; }
        public List<Scrutinio> Secondo { get; set; }
        

    }
}
