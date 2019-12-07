using SalveminiApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SalveminiApi.Models;
using Newtonsoft.Json;
using SalveminiApi.Argo.Models;
using System.Globalization;

namespace SalveminiApi.Controllers
{

    [RoutePrefix("api/utility")]
    public class UtilityController : ApiController
    {

        public DatabaseString db = new DatabaseString();


        //Returns all the subjects existing in ARGO
        [Route("materielist")]
        [HttpGet]
        public async Task<List<string>> getMaterie()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 3);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            var newMaterie = new List<string>();
            var currentMaterie = db.Materie.ToList();
            var utenti = db.Utenti.ToList();
            //Scan through each user
            foreach (var utente in utenti)
            {
                //Download compiti
                var argoUtils = new ArgoUtils();
                var argoClient = argoUtils.ArgoClient(utente.id, utente.ArgoToken);
                var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/compiti");
                if (!argoResponse.IsSuccessStatusCode) //Argo fail 
                    continue;
                var argoContent = await argoResponse.Content.ReadAsStringAsync();

                //Compiti
                var compiti = JsonConvert.DeserializeObject<compitiList>(argoContent).dati;

                foreach (var compito in compiti)
                {
                    //Check if materia already exists
                    var conflict = currentMaterie.SingleOrDefault(x => x.desMateria == compito.desMateria);
                    if (conflict == null && !newMaterie.Contains(compito.desMateria))  //New materia found

                    {
                        newMaterie.Add(compito.desMateria);
                        db.Materie.Add(new Materie { desMateria = compito.desMateria });
                        db.SaveChanges();
                    }
                }

            }
            return newMaterie;
        }


        [Route("appinfo")]
        [HttpGet]
        public AppInfo getAppInfo() {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            return db.AppInfo.Find(0);

        }

        //Save new appversion when update has been published
        [Route("newappinfo")]
        [HttpPost]
        public HttpResponseMessage postAppInfo(AppInfo newInfo)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 3);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            db.AppInfo.Find(0).AppVersion = newInfo.AppVersion;
            db.AppInfo.Find(0).OrariVersion = newInfo.OrariVersion;
            db.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        //Returns all the subjects existing in ARGO
        [Route("updateinfo")]
        [HttpGet]
        public async Task a()
        {

            var utenti = db.Utenti.ToList();

            //Scan through each user
            foreach (var utente in utenti)
            {
                try
                {
                    //Download compiti
                    var argoUtils = new ArgoUtils();
                    var argoClient = argoUtils.ArgoClient(utente.id, utente.ArgoToken);
                    var argoResponse = await  argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/schede");
                    var schedeContent = await argoResponse.Content.ReadAsStringAsync();
                    var ArgoUser = new List<Utente>();
                    try
                    {
                        ArgoUser = JsonConvert.DeserializeObject<List<Utente>>(schedeContent);
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (Utente argouser in ArgoUser)
                    {
                        try
                        {
                            utente.Residenza = Helpers.Utility.FirstCharToUpper(argouser.alunno.desComuneResidenza.ToLower());
                            utente.Compleanno = DateTime.ParseExact(argouser.alunno.datNascita, "yyyy-MM-dd", new CultureInfo("it-IT"));
                        }
                        catch
                        {
                            continue;
                        }

                    }

                }
                catch
                {
                    continue;
                }
            }
            db.SaveChanges();
            } 

    }
}
