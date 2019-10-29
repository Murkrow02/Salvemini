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
        public static string allegatiUrl(string prgMessaggio)
        {
            return "https://www.portaleargo.it/famiglia/api/rest/messaggiobachecanuova?id=FFFSS16836EEEII0000100000000" + prgMessaggio + "{token}" + argoKey;
        }

    }
}