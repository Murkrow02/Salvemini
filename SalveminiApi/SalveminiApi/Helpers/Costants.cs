using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi
{
    public class Costants
    {

        //Argo static cose
        public static string argoVersion = "2.1.0";
        public static string argoKey = "ax6542sdru3217t4eesd9";
        public static string allegatiUrl(string prgMessaggio, string prgAllegato)
        {
            return "https://www.portaleargo.it/famiglia/api/rest/messaggiobachecanuova?id=FFFSS16836EEEII0000" + prgAllegato + "00000000" + prgMessaggio + "{token}" + argoKey;
        }

        //Return our dictionary's subject or a formatted string if there isn't one
        public static string ShortSubject(string subject)
        {
                     Models.DatabaseString db = new Models.DatabaseString();

        var materia = db.Materie.SingleOrDefault(x=> x.desMateria == subject);
            if (materia != null)
            {
                return SubjectsList[subject];
            }
            else
            {
               return Helpers.Utility.FirstCharToUpper(subject.ToLower());
            }
        }

        //Dizionario materie privato ARGO FA SCHIFO
        public static Dictionary<string, string> SubjectsList = new Dictionary<string, string>
        {
            {"LINGUA e LETTERATURA ITALIANA","Letteratura" },
            { "DISEGNO E STORIA DELL'ARTE","Arte e Disegno" },
            { "FILOSOFIA","Filosofia" },
            { "LINGUA E CULTURA STRANIERA(INGLESE)","Inglese" },
            { "MATEMATICA","Matematica" },
            { "FISICA","Fisica" },
            { "INFORMATICA","Informatica" },
            { "STORIA","Storia" },
            { "SCIENZE NATURALI","Scienze" },
            { "LINGUA E CULTURA STRANIERA(FRANCESE)","Francese" },
            { "LINGUA E CULTURA STRANIERA(SPAGNOLO)","Spagnolo" },
            { "CONVERSAZIONE IN LINGUA STRANIERA (FRANCESE)","Francese" },
            { "STORIA DELL'ARTE","Arte" },
            { "Biology nei corsi Cambridge","Biology" },
            { "LINGUA e CULTURA LATINA","Latino" },
            { "RELIGIONE","Religione" },
            { "SCIENZE MOTORIE E SPORTIVE","Ed. Fisica" },
            { "History nei corsi Cambridge","History" },
            { "LINGUA E CULTURA STRANIERA(TEDESCO)","Tedesco" },
            { "English Literature nei corsi Cambridge","Literature" },
            { "CONVERSAZIONE IN LINGUA STRANIERA (SPAGNOLO)","Spagnolo" },
            { "STORIA E GEOGRAFIA","Storia e Geografia" },
            { "MATEMATICA (CON INFORMATICA)","Matematica e informatica" },


        };

    }
}