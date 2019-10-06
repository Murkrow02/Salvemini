using System;
namespace PdfParse
{
    public class JsonModel
    {
        public string key_0 { get; set; }
        public string key_1 { get; set; }
        public string key_2 { get; set; }
        public string key_3 { get; set; }
        public string key_4 { get; set; }
        public string key_5 { get; set; }
        public string key_6 { get; set; }
        public string key_7 { get; set; }
        public string key_8 { get; set; }
        public string key_9 { get; set; }
        public string key_10 { get; set; }
        public string key_11 { get; set; }
        public string key_12 { get; set; }
        public string key_13 { get; set; }
        public string key_14 { get; set; }
        public string key_15 { get; set; }
        public string key_16 { get; set; }
        public string key_17 { get; set; }
        public string key_18 { get; set; }
        public string key_19 { get; set; }
        public string key_20 { get; set; }
        public string key_21 { get; set; }
        public string key_22 { get; set; }
        public string key_23 { get; set; }
        public string key_24 { get; set; }
        public string key_25 { get; set; }
        public string key_26 { get; set; }
        public string key_27 { get; set; }
        public string key_28 { get; set; }
        public string key_29 { get; set; }
        public string key_30 { get; set; }
        public string key_31 { get; set; }
        public string key_32 { get; set; }
        public string key_33 { get; set; }
        public string key_34 { get; set; }
        public string key_35 { get; set; }
        public string key_36 { get; set; }
        public string key_37 { get; set; }
        public string key_38 { get; set; }
        public string key_39 { get; set; }
        public string key_40 { get; set; }
        public string key_41 { get; set; }
        public string key_42 { get; set; }
        public string key_43 { get; set; }
    }

    public class Treno
    {
        public string Partenza { get; set; }

        //(D = Diretto, DD = Direttissimo)
        public string Importanza { get; set; }

        //(FER = Feriale, FES = Festivo)
        public string Variazioni { get; set; } 

        //(0 = Sorrento, 1 = Sant'Agnello)
        public int Stazione { get; set; }

        //(true = Napoli, false = Sorrento)
        public bool Direzione { get; set; }
    }
}
