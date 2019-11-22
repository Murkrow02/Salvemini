using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Models
{
    public class Lezione
    {
            public int Giorno { get; set; }
            public int Ora { get; set; }
            public string Materia { get; set; }
            public string Sede { get; set; }
            public int numOre { get; set; }
            public bool toRemove {internal get; set; }
        }
}