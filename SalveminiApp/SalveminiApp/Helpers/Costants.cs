using System;
using System.Collections.Generic;

namespace SalveminiApp
{
    public class Costants
    {
        //Api Url
        public static string Uri(string next)
        {
            return "http://mysalvemini.me/api/" + next;
        }

        //Materie Dictionary
        public static Dictionary<string, string[]> Colori = new Dictionary<string, string[]>
        {
            {"Letteratura", new string[] { "#95FFFF", "#07CFFF" } },
            {"Fisica", new string[] { "#7D77FF", "#0045FF" } },
            {"Arte Teoria", new string[] { "#FAE365", "#FF8900" } },
            {"Filosofia", new string[] { "#FCB46C", "#EB2020" } },
        };

        public static Dictionary<int, string> Ore = new Dictionary<int, string>
        {
            {1, " 8:00" },
            {2, " 9:00"  },
            {3, "10:00"  },
            {4, "11:00" },
            {5, "11:55" },
            {6, "12:50" },
        };

        public static Dictionary<int, string> Stazioni = new Dictionary<int, string>
        {
            {0, "Sorrento"},
            {1, "S. Agnello"},
            {2, "Piano"},
            {3, "Meta"},
            {4, "Seiano"},
            {5, "Vico Equense"},
            {6, "Castellammare di Stabia"},
            {7, "Via Nocera"},
            {8, "Pioppaino"},
            {9, "Moregine"},
            {10, "Pompei Scavi"},
            {11, "Villa Regina"},
            {12, "Torre Annunziata"},
            {13, "Trecase"},
            {14, "Via Viuli"},
            {15, "Leopardi"},
            {16, "Villa delle Ginestre"},
            {17, "Via dei Monaci"},
            {18, "Via del Monte"},
            {19, "Via S'Antonio"},
            {20, "Torre del Greco"},
            {21, "Ercolano Miglio d'Oro"},
            {22, "Ercolano Scavi"},
            {23, "Portici Via Libertà"},
            {24, "Portici Bellavista"},
            {25, "S'Giorgio Cavalli di Bronzo"},
            {26, "S'Giorgio a Cremano"},
            {27, "S'Maria del Pozzo"},
            {28, "Barra"},
            {29, "S'Giovanni a Teduccio"},
            {30, "Via Gianturco"},
            {31, "Napoli Piazza Garibaldi"},
            {32, "Napoli Porta Nolana"},
        };
    }
}
