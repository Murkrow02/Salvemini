using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using SalveminiApi.Models;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api")]
    public class IndexController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("index")]
        [HttpPost]
        public IndexModel GetIndex()
        {
            var returnModel = new IndexModel();

            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();


            //Prendi versione dell'app
            try
            {
                returnModel.AppVersion = db.AppInfo.ToList()[0].Versione;
            }
            catch
            {
                //Prendi valore alto così non dice di aggiornare su app
                returnModel.AppVersion = 1000;
            }

            //Prendi ultimo avviso
            try
            {
                var avvisi = db.Avvisi.OrderBy(x => x.Creazione).ToList();
                returnModel.ultimoAvviso = avvisi.Last().Creazione;
            }
            catch
            {
                //Prendi valore alto così non dice che ce ne sono altri
                returnModel.ultimoAvviso = new DateTime(2090,10,10);
            }

            //Prendi notizie oggi argo
            //var argoUtils = new ArgoUtils();
            //var argoClient = argoUtils.ArgoClient(id, token);
            //var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/oggi");
            //var argoContent = await argoResponse.Content.ReadAsStringAsync();
            //var returnModel = JsonConvert.DeserializeObject<argomentiList>(argoContent);
            //return returnModel.dati;




            return new IndexModel();
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
