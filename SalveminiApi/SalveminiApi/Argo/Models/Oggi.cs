using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Argo.Models
{


    public class Oggi
    {
        public List<Dati> dati { get; set; }
    }

    public class Dati
    {
        //public string giorno { get; set; }

        //public int numAnno { get; set; }

        //public string codMin { get; set; }

        //public int prgAlunno { get; set; }

        //public int prgScheda { get; set; }

        //public int prgScuola { get; set; }

        //public int ordine { get; set; }

        public string tipo { get; set; }

        public string titolo { get; set; }

        public OggiValues dati { get; set; }

    }

    public class OggiValues
    {

        //public string codMin { get; set; }
        //public int numAnno { get; set; }
        //public int prgClasse { get; set; }
        //public int prgScuola { get; set; }
        //ALL
        public string datGiorno { get; set; }

        //IDK
        public string codEvento { get; set; }

        //Compiti
        public string desCompiti { get; set; }

        //Argomenti
        public string desArgomento { get; set; }

        //Argomenti e compiti
        public string docente { get; set; }
        public string desMateria { get; set; }
        public int prgMateria { get; set; }

        public string Materia
        {
            get
            {
                return Costants.ShortSubject(desMateria);
            }
        }

        //Voti
        public string codVoto { get; set; }
        public double decValore { get; set; }
        public string desCommento { get; set; }
        public string codVotoPratico { get; set; }
        public string desProva { get; set; }

        //Assenze
        public string datGiustificazione { get; set; }
        public string giustificataDa { get; set; }
        public string registrataDa { get; set; }
        public string binUid { get; set; }
        public string desAssenza { get; set; }
        public bool flgDaGiustificare { get; set; }
        public string datAssenza { get; set; }
        public int? numOra { get; set; }
        public string oraAssenza { get; set; }

        //Promemoria
        public string desAnnotazioni { get; set; }
        public string desMittente { get; set; }


        //Bacheca
        public int prgMessaggio { get; set; }
        public string desOggetto { get; set; }
        public bool adesione { get; set; }
        public bool richiediAd { get; set; }
        public bool presaVisione { get; set; }
        public bool richiediPv { get; set; }
        public string desUrl { get; set; }
        public string desMessaggio { get; set; }
        public List<Allegati> allegati { get; set; }
    }


    public class WholeModel
    {
        public List<Argomenti> argomenti { get; set; }
        public List<Voti> voti { get; set; }
        public List<Compiti> compiti { get; set; }
        public List<Assenze> assenze { get; set; }
        public List<Promemoria> promemoria { get; set; }
        public List<Bacheca> bacheca { get; set; }

    }
}
