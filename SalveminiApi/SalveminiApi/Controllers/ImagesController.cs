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



        [Route("deletepic")]
        [HttpGet]
        public HttpResponseMessage deleteProfilePic()
        {
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Get id from headers
            var _id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

            //Find user
            Models.DatabaseString db = new Models.DatabaseString();
            var utente = db.Utenti.Find(_id);
            if (utente == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            //Delete image
            try
            {
                File.Delete(HttpContext.Current.Server.MapPath("~/Images/users/" + utente.Immagine + ".png"));
                utente.Immagine = "";
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK);
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

                    int MaxContentLength = 1024 * 1024 * 20; //Size = 20 MB  

                    if (postedFile.ContentLength > MaxContentLength)
                    {

                        var message = string.Format("I file devono pesare meno di 15MB.");

                        dict.Add("error", message);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                    }
                    else
                    {
                        //Create write directory
                        var directory = HttpContext.Current.Server.MapPath("~/Images/" + path);

                        //Create directory if doesn't exist
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        //Create file path
                        var filePath = directory + "/" + id + ".png";

                        if (path == "users")
                        {
                            var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

                            //Create image with date
                            Models.DatabaseString db = new Models.DatabaseString();
                            var imageName = userId + "_" + Helpers.Utility.italianTime().ToString("ddMMyyyyHHmmss");
                            db.Utenti.Find(Convert.ToInt32(userId)).Immagine = imageName;
                            filePath = directory + "/" + imageName + ".png";
                            db.SaveChanges();

                            //Reduce image size if users
                            Helpers.Utility.CropImage(postedFile.InputStream, 320, 320, filePath);
                        }
                        else
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