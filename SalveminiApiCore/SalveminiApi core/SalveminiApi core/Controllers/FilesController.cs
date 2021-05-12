using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
namespace SalveminiApi_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private readonly Salvemini_DBContext db; public FilesController(Salvemini_DBContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) { db = context; _env = env; }


        [Route("giornalino/{id}")]
        [HttpGet]
        public IActionResult getGiornalino(string id)
        {

            try
            {
                var stream = new FileStream(_env.WebRootPath + "/Giornalino/" + id + ".pdf", FileMode.Open);
                return new FileStreamResult(stream, "application/pdf");
            }
            catch
            {
                return NotFound();
            }
        }

        //[Route("giornalino/upload")]
        //[HttpPost]
        //public async Task<IActionResult> PostImmagine()
        //{
        //    //Check Auth
        //    bool authorized = AuthHelper.Authorize(Request, db,3);
        //    if (!authorized)
        //        return Unauthorized();

        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    try
        //    {
        //        var giornalino = new Models.Giornalino { Data = Utility.italianTime() };
        //        db.Giornalino.Add(giornalino);
        //        db.SaveChanges();

        //        var httpRequest = Request;

        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {
        //                //Create write directory
        //                var directory = HttpContext.Current.Server.MapPath("~/Giornalino");

        //                //Create directory if doesn't exist
        //                if (!Directory.Exists(directory))
        //                    Directory.CreateDirectory(directory);

        //                var filePath = directory + "/" + giornalino.id + ".pdf";
        //                postedFile.SaveAs(filePath);
        //            }
        //            else
        //            {
        //                return Request.CreateResponse(HttpStatusCode.NotFound);
        //            }
        //            var message1 = string.Format("File caricato con successo.");
        //            return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
        //        }
        //        var res = string.Format("Nessun file selezionato.");
        //        dict.Add("error", res);
        //        return Request.CreateResponse(HttpStatusCode.NotFound, dict);
        //    }
        //    catch (Exception ex)
        //    {
        //        var res = string.Format("Errore Interno");
        //        dict.Add("error", res);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, dict);
        //    }
        //}
    }
}
