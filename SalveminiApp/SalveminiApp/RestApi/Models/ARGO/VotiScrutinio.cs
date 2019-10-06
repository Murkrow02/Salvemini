using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Scrutinio
    {
        public int ordineMateria { get; set; }
        public string desMateria { get; set; }

        public string Voto { get; set; }
        public int prgMateria { get; set; }
        public int prgScuola { get; set; }
        public int prgScheda { get; set; }
        public bool votoUnico { get; set; }
        public int prgPeriodo { get; set; }
        public int? assenze { get; set; }
        public string codMin { get; set; }
        public string suddivisione { get; set; }
        public int numAnno { get; set; }
        public int prgAlunno { get; set; }
        public string giudizioSintetico { get; set; }
        public int prgClasse { get; set; }
        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);

        public string FullAssenze
        {
            get
            {
                return assenze > 0 ? "Assenze: " + assenze : "Nessuna assenza";
            }
        }
    }

    public class ScrutinioGrouped
    {
        public List<Scrutinio> Primo { get; set; }
        public List<Scrutinio> Secondo { get; set; }
    }
}
