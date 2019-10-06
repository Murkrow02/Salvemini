using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
namespace PdfParse
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var path = "/Users/valeriopiodenicola/Desktop/TreniBack.json";
            var text = File.ReadAllText(path);
            var list = JsonConvert.DeserializeObject<List<JsonModel>>(text);
            var Importanze = list[0];
            var Variazioni = list[1];
            list.RemoveAt(0);
            list.RemoveAt(0);
            var Listagiusta = new List<Treno>();
            foreach (var stazione in list)
            {
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_1, Partenza = stazione.key_1, Variazioni = Variazioni.key_1, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_2, Partenza = stazione.key_2, Variazioni = Variazioni.key_2, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_3, Partenza = stazione.key_3, Variazioni = Variazioni.key_3, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_4, Partenza = stazione.key_4, Variazioni = Variazioni.key_4, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_5, Partenza = stazione.key_5, Variazioni = Variazioni.key_5, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_6, Partenza = stazione.key_6, Variazioni = Variazioni.key_6, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_7, Partenza = stazione.key_7, Variazioni = Variazioni.key_7, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_8, Partenza = stazione.key_8, Variazioni = Variazioni.key_8, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_9, Partenza = stazione.key_9, Variazioni = Variazioni.key_9, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_10, Partenza = stazione.key_10, Variazioni = Variazioni.key_10, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_11, Partenza = stazione.key_11, Variazioni = Variazioni.key_11, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_12, Partenza = stazione.key_12, Variazioni = Variazioni.key_12, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_13, Partenza = stazione.key_13, Variazioni = Variazioni.key_13, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_14, Partenza = stazione.key_14, Variazioni = Variazioni.key_14, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_15, Partenza = stazione.key_15, Variazioni = Variazioni.key_15, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_16, Partenza = stazione.key_16, Variazioni = Variazioni.key_16, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_17, Partenza = stazione.key_17, Variazioni = Variazioni.key_17, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_18, Partenza = stazione.key_18, Variazioni = Variazioni.key_18, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_19, Partenza = stazione.key_19, Variazioni = Variazioni.key_19, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_20, Partenza = stazione.key_20, Variazioni = Variazioni.key_20, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_21, Partenza = stazione.key_21, Variazioni = Variazioni.key_21, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_22, Partenza = stazione.key_22, Variazioni = Variazioni.key_22, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_23, Partenza = stazione.key_23, Variazioni = Variazioni.key_23, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_24, Partenza = stazione.key_24, Variazioni = Variazioni.key_24, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_25, Partenza = stazione.key_25, Variazioni = Variazioni.key_25, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_26, Partenza = stazione.key_26, Variazioni = Variazioni.key_26, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_27, Partenza = stazione.key_27, Variazioni = Variazioni.key_27, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_28, Partenza = stazione.key_28, Variazioni = Variazioni.key_28, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_29, Partenza = stazione.key_29, Variazioni = Variazioni.key_29, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_30, Partenza = stazione.key_30, Variazioni = Variazioni.key_30, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_31, Partenza = stazione.key_31, Variazioni = Variazioni.key_31, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_32, Partenza = stazione.key_32, Variazioni = Variazioni.key_32, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_33, Partenza = stazione.key_33, Variazioni = Variazioni.key_33, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_34, Partenza = stazione.key_34, Variazioni = Variazioni.key_34, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_35, Partenza = stazione.key_35, Variazioni = Variazioni.key_35, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_36, Partenza = stazione.key_36, Variazioni = Variazioni.key_36, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_37, Partenza = stazione.key_37, Variazioni = Variazioni.key_37, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_38, Partenza = stazione.key_38, Variazioni = Variazioni.key_38, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_39, Partenza = stazione.key_39, Variazioni = Variazioni.key_39, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_40, Partenza = stazione.key_40, Variazioni = Variazioni.key_40, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_41, Partenza = stazione.key_41, Variazioni = Variazioni.key_41, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_42, Partenza = stazione.key_42, Variazioni = Variazioni.key_42, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
                Listagiusta.Add(new Treno { Direzione = true, Importanza = Importanze.key_43, Partenza = stazione.key_43, Variazioni = Variazioni.key_43, Stazione = Costants.Stazioni.FirstOrDefault(x => x.Value == Costants.RealStazioni[stazione.key_0]).Key });
            }

            Listagiusta = Listagiusta.Where(x => !string.IsNullOrEmpty(x.Partenza)).ToList();
            Listagiusta = Listagiusta.Where(x => x.Partenza != "|").ToList();

            var jsonFinale = JsonConvert.SerializeObject(Listagiusta);
            File.WriteAllText("/Users/valeriopiodenicola/Desktop/OrariTreni2.txt", jsonFinale);
        }
    }
}
