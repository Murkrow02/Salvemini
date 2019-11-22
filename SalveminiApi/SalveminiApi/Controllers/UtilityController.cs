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

            var materie = new List<string>();

            var utenti = db.Utenti.ToList();

            foreach(var utente in utenti)
            {
                var argoUtils = new ArgoUtils();
                var argoClient = argoUtils.ArgoClient(utente.id, utente.ArgoToken);
                var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/compiti");
                if (!argoResponse.IsSuccessStatusCode)
                    continue;
                var argoContent = await argoResponse.Content.ReadAsStringAsync();
                var compiti = JsonConvert.DeserializeObject<compitiList>(argoContent).dati;
               
                foreach(var compito in compiti)
                {
                    if (!materie.Contains(compito.desMateria))
                        materie.Add(compito.desMateria);
                }

            }

            return materie;
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

    }
}
