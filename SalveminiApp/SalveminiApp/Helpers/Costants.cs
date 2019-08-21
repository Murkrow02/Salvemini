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
            {0, "Sorrento"}
        };
        public List<RestApi.Models.Treno> Treni = new List<RestApi.Models.Treno>
        {
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "6:01", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "6:25", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "6:47", Importanza = "DD", Variazioni = "FES" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "6:49", Importanza = "DD", Variazioni = "FER" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "7:22", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "7:38", Importanza = "D", Variazioni = "FER" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "7:55", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "8:26", Importanza = "DD" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "8:52", Importanza = "D", Variazioni="FER" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "9:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "9:37", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "10:37", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "11:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "11:37", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "12:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "12:37", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "13:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "13:25", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "13:56", Importanza = "DD" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "14:22", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "14:55", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "15:26", Importanza = "DD" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "16:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "16:37", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "17:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "17:25", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "17:56", Importanza = "DD" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "18:22", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "18:55", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "19:26", Importanza = "DD" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "19:52", Importanza = "D", Variazioni = "FER" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "20:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "20:37", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "21:07", Importanza = "D" },
            new RestApi.Models.Treno { Stazione = 0 , Partenza = "21:55", Importanza = "D" },
        };
    }
}
