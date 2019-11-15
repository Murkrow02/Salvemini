using System;
using System.Collections.Generic;

namespace SalveminiApp.RestApi.Models
{
    public class Bus
    {
        public int Stazione { get; set; }
        public string Partenza { get; set; }
        public string Variazioni { get; set; } = "";
        public bool Direzione { get; set; }
    }

    public class Linea
    {
        public List<Bus> Buses { get; set; }
        public string Name { get; set; }
        public Dictionary<int,string> Stazioni { get; set; }
    }


}
