using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.Models
{
    public class Lezione
    {
        private readonly Salvemini_DBContext db; public Lezione(Salvemini_DBContext context) { db = context; }

        public int Giorno { get; set; }
        public int Ora { get; set; }
        public int idMateria { get; set; }
        public string Sede { get; set; }
        public int numOre { get; set; }
        public bool toRemove { internal get; set; }

        public string Materia
        {
            get {
                if (idMateria == -1) //Detect freeday
                    return "Libero";

               var materia = db.Materie.Find(idMateria);
                if (materia == null)
                    return "Materia non trovata";
                else if (!string.IsNullOrEmpty(materia.Materia))
                    return materia.Materia;
                else
                    return Utility.FirstCharToUpper(materia.DesMateria.ToLower()); //Materia non tradotta
                        
            } set { }
        }

       
    }
}