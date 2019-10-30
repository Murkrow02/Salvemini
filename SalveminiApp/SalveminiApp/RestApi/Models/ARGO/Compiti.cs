﻿using System;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Compiti
    {
        public string datGiorno { get; set; }
        public string desMateria { get; set; }
        public string Data { get; set; }
        public string desCompiti { get; set; }
        public string docente { get; set; }
        public string codMin { get; set; }
        public int numAnno { get; set; }
        public int prgMateria { get; set; }
        public int prgClasse { get; set; }
        public int prgScuola { get; set; }

        public string formattedSubject
        {
            get
            {
                return desMateria.ToUpper()[0] + desMateria.Substring(1).ToLower();
            }
        }

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
                return desCompiti;
            }
        }

        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);

    
    }
}
