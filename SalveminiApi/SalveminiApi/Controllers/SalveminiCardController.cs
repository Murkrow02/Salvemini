using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SalveminiApi.Models;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/card")]
    public class SalveminiCardController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("offerte")]
        [HttpGet]
        public List<SalveminiCard> getOfferte()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi offerte
            var offerte = db.SalveminiCard.ToList();
            return offerte;
        }

        [Route("offerta")]
        [HttpPost]
        public HttpResponseMessage postOfferta(SalveminiCard offerta)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request,3);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            
            //Add offer to database
            db.SalveminiCard.Add(offerta);
            db.SaveChanges();
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