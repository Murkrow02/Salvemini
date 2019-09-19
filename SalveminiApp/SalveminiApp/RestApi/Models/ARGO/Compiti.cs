using System;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Compiti
    {
        public string datGiorno { get; set; }
        public string desMateria { get; set; }
        public string Materia { get; set; }
        public string Data { get; set; }
        public string desCompiti { get; set; }
        public string docente { get; set; }
        public string codMin { get; set; }
        public int numAnno { get; set; }
        public int prgMateria { get; set; }
        public int prgClasse { get; set; }
        public int prgScuola { get; set; }

        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);

    
    }
}
