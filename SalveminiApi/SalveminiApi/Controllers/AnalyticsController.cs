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
    [RoutePrefix("api/analytics")]
    public class AnalyticsController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("all")]
        [HttpGet]
        public List<Models.Analytics> GetAnalytics()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 2);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            var fullAnalytics = new List<Analytics>();

            //Add db saved analytics
            fullAnalytics.AddRange(db.Analytics.ToList());

            //Add user count
            fullAnalytics.Add(new Analytics { Tipo = "UtentiCount", Valore = db.Utenti.Count() });

            return fullAnalytics;
        }

        [Route("console")]
        [HttpGet]
        public List<Models.EventsLog> GetConsole()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 3);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            return db.EventsLog.OrderByDescending(x => x.Data).ToList();
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