using System;
namespace SalveminiApp.RestApi.Models
{
    public class Assenza
    {
        public string codEvento { get; set; }
        public string datGiustificazione { get; set; }
        public string binUid { get; set; }
        public string codMin { get; set; }
        public string datAssenza { get; set; }
        public string giustificataDa { get; set; }
        public string desAssenza { get; set; }
        public string registrataDa { get; set; }
        public string oraAssenza { get; set; }
        public int? numOra { get; set; }
        public int prgScuola { get; set; }
        public int prgScheda { get; set; }
        public int numAnno { get; set; }
        public int prgAlunno { get; set; }
        public bool flgDaGiustificare { get; set; }

        public string Data
        {
            get
            {
                return Convert.ToDateTime(datAssenza).ToString("dddd, dd MMMM yyyy");
            }
        }

        public string Professore
        {
            get
            {
                string temp = registrataDa.Substring(7);
                temp = temp.Remove(temp.Length - 1);
                return temp;
            }
        }

        public string Tipo
        {
            get
            {
                switch (codEvento)
                {
                    case "A":
                        return "Assenza";
                    case "U":
                        return "Uscita";
                    case "I":
                        return "Ritardo";
                    default:
                        return "Assenza";
                }
            }
        }

        public string StatusColor
        {
            get
            {
                return datGiustificazione != null || flgDaGiustificare == false ? "#A9FA63" : "#FA6363";
            }
        }

        public double StatusHeight
        {
            get
            {
                return App.ScreenWidth / 20;
            }
        }



        public float StatusRadius
        {
            get
            {
                return (float)(App.ScreenWidth / 20) / 2;
            }
        }

        public double NumberHeight
        {
            get
            {
                return App.ScreenWidth / 25;
            }
        }

        public float NumberRadius
        {
            get
            {
                return (float)(App.ScreenWidth / 25) / 2;
            }
        }

        public bool NumberVisibility
        {
            get
            {
                return codEvento == "U" || codEvento == "I";
            }
        }
    }
}
