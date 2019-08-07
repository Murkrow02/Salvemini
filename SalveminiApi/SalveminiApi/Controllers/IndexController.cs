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
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

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
