using System;
using System.Collections.Generic;

namespace SalveminiApp
{
    public class Costants
    {
        //Api Url
        public static string Uri(string next)
        {
            return "http://mysalvemini.me/Api/" + next;
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
    }
}
