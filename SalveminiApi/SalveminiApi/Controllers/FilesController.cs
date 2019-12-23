using SalveminiApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api")]
    public class FilesController : ApiController
    {
        private DatabaseString db = new DatabaseString();


        [Route("giornalino/{id}")]
        [HttpGet]
        public IHttpActionResult getGiornalino(string id)
        {

            try
            {
                var stream = File.OpenRead(HttpContext.Current.Server.MapPath("~/Giornalino/" + id + ".pdf"));
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = stream.Length;

                return ResponseMessage(response);
            }
            catch
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("giornalino/upload")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostImmagine()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request,3);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var giornalino = new Models.Giornalino { Data = Helpers.Utility.italianTime() };
                db.Giornalino.Add(giornalino);
                db.SaveChanges();

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        //Create write directory
                        var directory = HttpContext.Current.Server.MapPath("~/Giornalino");

                        //Create directory if doesn't exist
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        var filePath = directory + "/" + giornalino.id + ".pdf";
                        postedFile.SaveAs(filePath);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    var message1 = string.Format("File caricato con successo.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Nessun file selezionato.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("Errore Interno");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, dict);
            }
        }
    }
}
