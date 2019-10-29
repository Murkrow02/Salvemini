using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using SalveminiApi.Models;
using SalveminiApi.Utility;
using SalveminiApi.Argo.Models;
using System.Net.Http;
using System.Text;
using SalveminiApi.Helpers;
using System.Collections.ObjectModel;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/argo")]
    public class ArgoController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("compiti")]
        [HttpGet]
        public async Task<List<Compiti>> getCompiti()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/compiti");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<compitiList>(argoContent);
            return returnModel.dati;
        }

        [Route("argomenti")]
        [HttpGet]
        public async Task<List<Argomenti>> getArgomenti()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/argomenti");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<argomentiList>(argoContent);
            return returnModel.dati;
        }

        [Route("promemoria")]
        [HttpGet]
        public async Task<List<Promemoria>> getPromemoria()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/promemoria");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<promemoriaList>(argoContent);
            return returnModel.dati;
        }

        [Route("assenze")]
        [HttpGet]
        public async Task<List<Assenze>> getAssenze()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/assenze");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<AssenzeList>(argoContent);

            var returnList = new List<Assenze>();

            returnList = returnModel.dati;

            return returnList;
        }

        [Route("newAssenze")]
        [HttpGet]
        public async Task<bool> newAssenze()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/assenze");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<AssenzeList>(argoContent);

            //Trova assenza non giustificata
            var daGiustificare = returnModel.dati.Where(x => x.flgDaGiustificare == true || x.datGiustificazione == null).ToList();
            if (daGiustificare.Count > 0)
            {
                return true;
            }
            return false;

        }

        [Route("giustifica")]
        [HttpPost]
        public async Task<HttpResponseMessage> giustificaAssenza(AssenzaModel giustifica)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Posta modello
            var argoUtils = new ArgoUtils();
            var json = JsonConvert.SerializeObject(giustifica);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.PostAsync("https://www.portaleargo.it/famiglia/api/rest/giustifica", content);
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);


            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("voti")]
        [HttpGet]
        public async Task<List<Voti>> getVoti()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/votigiornalieri");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<VotiList>(argoContent);

            var votiList = new List<Voti>();
            votiList = returnModel.dati;

            //Raggruppa per materia
            var groupedVoti = new List<GroupedVoti>();
            var votiGrouped = votiList.GroupBy(x => x.desMateria).ToList();
            var groupedCollection = new List<GroupedVoti>();

            return votiList;
        }

        [Route("pentagono")]
        [HttpGet]
        public async Task<List<Pentagono>> getPentagono()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/votigiornalieri");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<VotiList>(argoContent);
            var votiList = new List<Voti>();
            votiList = returnModel.dati;

            //Raggruppa per materia
            var groupedVoti = new List<GroupedVoti>();
            var votiGrouped = votiList.GroupBy(x => x.desMateria).ToList();
            var groupedCollection = new List<Pentagono>();

            for (int i = 0; i < votiGrouped.Count(); i++)
            {
                var groupedMateria = new Pentagono();
                var listaVoti = new List<Voti>();

                foreach (var voto in votiGrouped[i])
                {
                    listaVoti.Add(voto);
                    groupedMateria.Materia = voto.desMateria;
                }


                //Calcola media
                int count = 0;
                double media = 0.0;
                foreach (var voto in votiGrouped[i])
                {

                    //Controlla se è una giustifica
                    if (voto.decValore == null)
                    {
                        voto.codVoto = "G";
                        continue;
                    }

                    //Aggiungi alla media
                    media += Convert.ToDouble(voto.decValore);
                    count++;
                }

                //Controlla se ci sono voti
                if (count > 0)
                    groupedMateria.Media = media / count;
                else
                    groupedMateria.Media = 0;


                groupedCollection.Add(groupedMateria);

            }


            return groupedCollection;
        }


        [Route("scrutinio")]
        [HttpGet]
        public async Task<ScrutinioGrouped> getScrutinio()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get parameters from request
            int id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/votiscrutinio");
            if (!argoResponse.IsSuccessStatusCode)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<List<Scrutinio>>(argoContent);

            //Ragruppa voti
            var grouped = new ScrutinioGrouped();
            var Primo = new List<Scrutinio>();
            var Secondo = new List<Scrutinio>();

            Primo = (returnModel.Where(x => x.prgPeriodo == 1).ToList());
            Secondo = (returnModel.Where(x => x.prgPeriodo == 2).ToList());

            grouped.Primo = Primo;
            grouped.Secondo = Secondo;

            return grouped;
        }


        //new argo cazzi
//        Posta visione documento
//Post a url https://www.portaleargo.it/famiglia/api/rest/presavisionebachecanuova
//{"prgMessaggio":20,"presaVisione":true}

//Tutte le bacheche
//Get url https://www.portaleargo.it/famiglia/api/rest/bachecanuova

//Url documenti bacheca
//https://www.portaleargo.it/famiglia/api/rest/messaggiobachecanuova?id=FFFSS16836EEEII0000100000000 + prgMessaggio + userToken + ax6542sdru3217t4eesd9

//Esempi
//https://www.portaleargo.it/famiglia/api/rest/messaggiobachecanuova?id=FFFSS16836EEEII000010000000014e5308fc6214a41488372057ba09c422c.11ax6542sdru3217t4eesd9
//https://www.portaleargo.it/famiglia/api/rest/messaggiobachecanuova?id=FFFSS16836EEEII000010000000014e5308fc6214a41488372057ba09c422c.11ax6542sdru3217t4eesd9
//https://www.portaleargo.it/famiglia/api/rest/messaggiobachecanuova?id=FFFSS16836EEEII000010000000020bbb1bb6958f549e1860701fa46898311.11ax6542sdru3217t4eesd9


//Tutte le note
//Get url https://www.portaleargo.it/famiglia/api/rest/notedisciplinari

//Cambio password
//Post a https://www.portaleargo.it/famiglia/api/rest/cambiopassword
//{"vecchiaPassword":"vecchia","nuovaPassword":"nuova"}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

       
    }

    
}
