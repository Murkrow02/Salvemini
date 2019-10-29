using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

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

        public string Data
        {
            get
            {
                return Convert.ToDateTime(datGiorno).ToString("dddd, dd MMMM yyyy");
            }
        }

        public Color markColor
        {
            get
            {
                return decValore < 6 ? Color.FromHex("#E37070") : Styles.TextColor;
            }
        }

        public bool dotsVisibility
        {
            get
            {
                return !string.IsNullOrEmpty(desCommento);
            }
        }
        public bool SeparatorVisibility { get; set; } = true;
		public Thickness CellPadding { get; set; } = new Thickness(10);
	}



    //Pentagono
    public class Pentagono
    {
        public string Materia { get; set; }
        public double Media { get; set; }
    }
}

