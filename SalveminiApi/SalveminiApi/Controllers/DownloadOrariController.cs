using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SalveminiApi.Controllers
{

    [RoutePrefix("api/orari")]
    public class DownloadOrariController : ApiController
    {
        [Route("treni")]
        [HttpGet]
        public string getOrari()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Read string from txt file
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Orari/OrariTreni.txt");
              var a =   File.ReadAllText(path);
                return a;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

        }
    }
}