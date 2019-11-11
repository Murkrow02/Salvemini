using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

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
                var a = File.ReadAllText(path);
                return a;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error get orario trasporti", ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

        }

        [Route("classe/{classe}")]
        [HttpPost]
        public HttpResponseMessage uploadOrariClasse(string classe, List<Models.Lezione> orario)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 1);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Serialize json
            try
            {
                string orarioString = JsonConvert.SerializeObject(orario);
                var path = HttpContext.Current.Server.MapPath("~/Orari/Classi/" + classe + ".txt");

                //Save file
                File.WriteAllText(path, orarioString);

                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error saving orario classe " + classe, ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Route("classe/{classe}")]
        [HttpGet]
        public List<Models.Lezione> getOrarioClasse(string classe)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Deserialize json
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Orari/Classi/" + classe + ".txt");
                var orarioString = File.ReadAllText(path);
                var orario = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                return orario;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error getting orario classe " + classe, ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}