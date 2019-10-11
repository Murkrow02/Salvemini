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
    [RoutePrefix("api/Images")]
    public class ImagesController : ApiController
    {
        [Route("{path}/{id}")]
        [HttpGet]
        public IHttpActionResult getImmagini(string path, string id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            try
            {
                var stream = File.OpenRead(HttpContext.Current.Server.MapPath("~/Images/" + path + "/" + id + ".png"));
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = stream.Length;

                return ResponseMessage(response);
            }
            catch
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("upload/{path}/{id}")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostImmagine(string path, string id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

                        //IList<string> AllowedFileExtensions = new List<string> { ".png" };
                        //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        //var extension = ext.ToLower();
                        //if (!AllowedFileExtensions.Contains(extension))
                        //{

                        //    var message = string.Format("Puoi caricare solo immagini png");

                        //    dict.Add("error", message);
                        //    return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        //}
                         if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("I file devono pesare meno di 10MB.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/Images/" + path + "/" + id + ".png");
                            postedFile.SaveAs(filePath);
                        }
                    }

                    var message1 = string.Format("Immagine caricata con successo.");
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
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }


    }
}