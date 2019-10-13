using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{

    public class Dates
    {
        public string giorno { get; set; }

        public int numAnno { get; set; }

        public string tipo { get; set; }

        public string titolo { get; set; }

        public string codMin { get; set; }

        public int prgAlunno { get; set; }

        public int prgScheda { get; set; }

        public int prgScuola { get; set; }

        public int ordine { get; set; }

        public OggiValues dati { get; set; }

    }

    public class OggiValues
    {


        //public OggiValues(string desCompiti, string desMateria, string docente, string desArgomento)
        //{

        //    this.desCompiti = desCompiti;
        //    this.desMateria = desMateria;
        //    this.docente = docente;
        //    this.desArgomento = desArgomento;

        //}

        public string datGiorno { get; set; }

        public string codEvento { get; set; }

        public string desMateria { get; set; }

        public string desArgomento { get; set; }

        public string desCompiti { get; set; }

        public string docente { get; set; }

        public string codMin { get; set; }

        public string codVoto { get; set; }

        public double decValore { get; set; }

        public string datGiustificazione { get; set; }

        public string giustificataDa { get; set; }

        public string registrataDa { get; set; }

        public string binUid { get; set; }

        public string desAssenza { get; set; }

        public int numAnno { get; set; }

        public int prgMateria { get; set; }

        public int prgClasse { get; set; }

        public int prgScuola { get; set; }

        public bool flgDaGiustificare { get; set; }

        public string desAnnotazioni { get; set; }

        public string desMittente { get; set; }

        public string datAssenza { get; set; }


    }

    public class WholeModel
    {
        public string desCommento { get; set; }

        public string giorno { get; set; }

        public string tipo { get; set; }

        public string titolo { get; set; }

        public string codMin { get; set; }

        public string datGiorno { get; set; }

        public string codEvento { get; set; }

        public string desMateria { get; set; }

        public string desArgomento { get; set; }

        public string desCompiti { get; set; }

        public string docente { get; set; }

        public string datGiustificazione { get; set; }

        public string giustificataDa { get; set; }

        public string registrataDa { get; set; }

        public string binUid { get; set; }

        public string desAssenza { get; set; }

        public string desAnnotazioni { get; set; }

        public string desMittente { get; set; }

        public string codVoto { get; set; }

        public int numAnno { get; set; }

        public int prgAlunno { get; set; }

        public int prgScheda { get; set; }

        public int prgScuola { get; set; }

        public int ordine { get; set; }

        public int prgMateria { get; set; }

        public int prgClasse { get; set; }

        public double decValore { get; set; }

        public bool flgDaGiustificare { get; set; }

        public string datAssenza { get; set; }

        public string name
        {
            get
            {
                string value = "";
                switch (tipo)
                {
                    case "PRO":
                        value = "Promemoria";
                        break;
                    case "VOT":
                        value = "Voto " + codVoto;
                        break;
                    case "ARG":
                        value = "Argomento Lezione";
                        break;
                    case "ASS":
                        if (codEvento == "I")
                        {
                            value = "Ritardo";

                        }
                        else if (codEvento == "A")
                        {
                            value = "Assenza";
                        }
                        else
                        {
                            value = "Uscita Anticipata";
                        }
                        break;
                    case "COM":
                        value = "Compiti";
                        break;
                }
                return value;
            }
        }

        public string button
        {
            get
            {
                string value = "";
                switch (tipo)
                {
                    case "PRO":
                        value = " Visualizza ";
                        break;
                    case "VOT":
                        value = " Visualizza ";
                        break;
                    case "ARG":
                        value = " Visualizza ";
                        break;
                    case "ASS":
                        if (datGiustificazione != null || flgDaGiustificare != true)
                        {
                            value = " Giustificata ";
                        }
                        else
                        {
                            value = " Giustifica ";
                        }
                        break;
                    case "COM":
                        value = " Visualizza ";
                        break;
                }

                return value;
            }
        }

        public string color
        {
            get
            {
                string value = "";
                switch (tipo)
                {
                    case "PRO":
                        value = "#007AFF";
                        break;
                    case "VOT":
                        value = "#007AFF";
                        break;
                    case "ARG":
                        value = "#007AFF";
                        break;
                    case "ASS":
                        if (datGiustificazione != null || flgDaGiustificare != true)
                        {
                            value = "#00bf00";
                        }
                        else
                        {
                            value = "#007AFF";
                        }


                        break;
                    case "COM":
                        value = "#007AFF";
                        break;
                }
                return value;
            }
        }

        public string teacher
        {
            get
            {
                try
                {
                    string value = "";
                    if (registrataDa != null)
                    {
                        value = registrataDa;
                    }
                    else if (desMittente != null)
                    {
                        value = desMittente;
                    }
                    else
                    {
                        value = desMateria.First().ToString().ToUpper() + desMateria.Substring(1).ToLower();
                    }
                    return value;

                }
                catch
                {
                    return teacher;
                }
              
            }
        }



        public class OggiValues
        {


            //public OggiValues(string desCompiti, string desMateria, string docente, string desArgomento)
            //{

            //    this.desCompiti = desCompiti;
            //    this.desMateria = desMateria;
            //    this.docente = docente;
            //    this.desArgomento = desArgomento;

            //}

            public string datGiorno { get; set; }

            public string codEvento { get; set; }

            public string desMateria { get; set; }

            public string desArgomento { get; set; }

            public string desCompiti { get; set; }

            public string docente { get; set; }

            public string codMin { get; set; }

            public string codVoto { get; set; }

            public double decValore { get; set; }

            public string datGiustificazione { get; set; }

            public string giustificataDa { get; set; }

            public string registrataDa { get; set; }

            public string binUid { get; set; }

            public string desAssenza { get; set; }

            public int numAnno { get; set; }

            public int prgMateria { get; set; }

            public int prgClasse { get; set; }

            public int prgScuola { get; set; }

            public bool flgDaGiustificare { get; set; }

            public string desAnnotazioni { get; set; }

            public string desMittente { get; set; }

            public string datAssenza { get; set; }


        }
    }

    public class Oggi
    {
        public List<Dates> dati { get; set; }
    }
}
