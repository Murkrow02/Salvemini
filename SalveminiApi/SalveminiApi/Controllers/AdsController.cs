using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using SalveminiApi.Argo.Models;
using SalveminiApi.Models;
using SalveminiApi.Utility;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/ads")]
    public class AdsController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("canWatchAd")]
        [HttpGet]
        public HttpResponseMessage canWatchAd()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var utente = db.Utenti.Find(id);
            if (utente == null) throw new HttpResponseException(HttpStatusCode.NotFound); //User not found

            //Check if is new day
            if (!utente.LastAdWatched.HasValue || utente.LastAdWatched.Value.ToString("dd-MM-yyyy") != Helpers.Utility.italianTime().ToString("dd-MM-yyyy"))
            {
                utente.LastAdWatched = Helpers.Utility.italianTime();
                utente.AdsWatched = 0;
                db.SaveChanges();
            }

            //Check how many ads watched
            if (utente.AdsWatched <= 3)
                return new HttpResponseMessage(HttpStatusCode.OK);
            else
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        }

        [Route("watchedAd")]
        [HttpGet]
        public HttpResponseMessage watchedAd()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var utente = db.Utenti.Find(id);
            if (utente == null) throw new HttpResponseException(HttpStatusCode.NotFound); //User not found

            //He cheated or something
            if (utente.AdsWatched >= 4)
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable);

            //Save ad watch count
            utente.AdsWatched++;
            //Add new coin to balance
            utente.sCoin++;

            //Save changes
            db.SaveChanges();

            //Add to console log new user created
           Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha guadagnato una sCoin tramite Ad");

            return new HttpResponseMessage(HttpStatusCode.OK);
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
