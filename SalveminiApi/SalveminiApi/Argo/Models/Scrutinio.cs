using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{
    public class Scrutinio
    {
        public int ordineMateria { get; set; }
        public string desMateria { get; set; }

        public votoGropued votoOrale {internal get; set; }
        public int prgMateria {internal get; set; }
        public int prgScuola {internal get; set; }
        public int prgScheda {internal get; set; }
        public bool votoUnico { get; set; }
        public int prgPeriodo {internal get; set; }
        public int? assenze { get; set; }
        public string codMin {internal get; set; }
        public string suddivisione { get; set; }
        public int numAnno {internal get; set; }
        public int prgAlunno {internal get; set; }
        public string giudizioSintetico { get; set; }
        public int prgClasse { get; set; }


        public string Materia
        {
            get
            {
                try { return Helpers.Utility.FirstCharToUpper(desMateria.ToLower()); } catch { return desMateria; }
            }
        }

        public string Voto
        {
            get
            {
                return votoOrale.codVoto;
            }
        }

        public class votoGropued
        {
            public string codVoto { get; set; }
        }
    }

    public class ScrutinioGrouped
    {
        public List<Scrutinio> Primo { get; set; }
        public List<Scrutinio> Secondo { get; set; }
    }
}