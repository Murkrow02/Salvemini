using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace SalveminiApi.Utility
{
    public class ArgoUtils
    {
        public  HttpClient ArgoClient(int id, string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-app-code", "APF");
            client.DefaultRequestHeaders.Add("x-cod-min", "SS16836");
            client.DefaultRequestHeaders.Add("x-key-app", "ax6542sdru3217t4eesd9");
            client.DefaultRequestHeaders.Add("x-produttore-software", "ARGO Software s.r.l. - Ragusa");
            client.DefaultRequestHeaders.Add("x-version", "2.1.0");
            client.DefaultRequestHeaders.Add("x-prg-scheda", "1");
            client.DefaultRequestHeaders.Add("x-prg-scuola", "1");
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Mobile Safari/537.36");
            client.DefaultRequestHeaders.Add("x-auth-token", token);
            client.DefaultRequestHeaders.Add("x-prg-alunno", id.ToString());
            return client;
        }
    }
    
}