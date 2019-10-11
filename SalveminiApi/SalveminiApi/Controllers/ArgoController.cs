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
        public async Task<List<GroupedVoti>> getVoti()
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
            var votiGrouped = votiList.GroupBy(x => x.Materia).ToList();
            var groupedCollection = new List<GroupedVoti>();
            
            for (int i = 0; i < votiGrouped.Count(); i++)
            {
                var groupedMateria = new GroupedVoti();
                var listaVoti = new List<Voti>();

                foreach (var voto in votiGrouped[i])
                {
                    listaVoti.Add(voto);
                    groupedMateria.Materia = voto.Materia;
                }

                foreach (var voto in listaVoti)
                {
                    groupedMateria.Add(voto);
                }

                //Calcola media
                int count = 0;
                double media = 0.0;
                foreach (var voto in votiGrouped[i])
                {

                    //Controlla se è una giustifica
                    if (voto.decValore == null) {
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
            var votiGrouped = votiList.GroupBy(x => x.Materia).ToList();
            var groupedCollection = new List<Pentagono>();

            for (int i = 0; i < votiGrouped.Count(); i++)
            {
                var groupedMateria = new Pentagono();
                var listaVoti = new List<Voti>();

                foreach (var voto in votiGrouped[i])
                {
                    listaVoti.Add(voto);
                    groupedMateria.Materia = voto.Materia;
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

        [Route("oggi")]
        [HttpGet]
        public async Task<List<WholeModel>> getOggi(DateTime data)
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

            //Codice copiato dalla salveminiapp vecchia, non si capisce un cazzo (ritorna cosa è successo in classe)
            var Oggi = new Oggi();
            List<WholeModel> Oggis = new List<WholeModel>();
            var uri = "https://www.portaleargo.it/famiglia/api/rest/oggi?datGiorno=" + data.ToString("yyyy-MM-dd").Split(' ')[0].Trim();

            try
            {
                var response = await argoClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                var content = await response.Content.ReadAsStringAsync();
                Oggi = JsonConvert.DeserializeObject<Oggi>(content);
                var Dates = Oggi.dati;
                for (int i = Dates.Count - 1; i >= 0; i--)
                {
                    var Model = new WholeModel();
                    Model.binUid = Dates[i].dati.binUid;
                    Model.codEvento = Dates[i].dati.codEvento;
                    Model.codMin = Dates[i].codMin;
                    Model.codVoto = Dates[i].dati.codVoto;
                    Model.datGiorno = Dates[i].dati.datGiorno;
                    Model.datGiustificazione = Dates[i].dati.datGiustificazione;
                    Model.decValore = Dates[i].dati.decValore;
                    Model.desAnnotazioni = Dates[i].dati.desAnnotazioni;
                    Model.desArgomento = Dates[i].dati.desArgomento;
                    Model.desAssenza = Dates[i].dati.desAssenza;
                    Model.desCompiti = Dates[i].dati.desCompiti;
                    Model.desMateria = Dates[i].dati.desMateria;
                    Model.desMittente = Dates[i].dati.desMittente;
                    Model.docente = Dates[i].dati.docente;
                    Model.flgDaGiustificare = Dates[i].dati.flgDaGiustificare;
                    Model.giorno = Dates[i].giorno;
                    Model.giustificataDa = Dates[i].dati.giustificataDa;
                    Model.numAnno = Dates[i].numAnno;
                    Model.ordine = Dates[i].ordine;
                    Model.prgAlunno = Dates[i].prgAlunno;
                    Model.prgClasse = Dates[i].dati.prgClasse;
                    Model.prgMateria = Dates[i].dati.prgMateria;
                    Model.prgScheda = Dates[i].prgScheda;
                    Model.prgScuola = Dates[i].prgScuola;
                    Model.registrataDa = Dates[i].dati.registrataDa;
                    Model.tipo = Dates[i].tipo;
                    Model.datAssenza = Dates[i].dati.datAssenza;
                    Oggis.Add(Model);
                }

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return Oggis;
        }






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
