using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Web;
namespace SalveminiApi_core.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private readonly Salvemini_DBContext db; public ImagesController(Salvemini_DBContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) { db = context; _env = env; }

        [Route("{path}/{id}")]
        [HttpGet]
        public IActionResult getImmagini(string path, string id)
        {

            try
            {
                var stream = System.IO.File.OpenRead(_env.WebRootPath + "/images/" + path + "/" + id + ".png");

                return File(stream, "image/png");
            }
            catch
            {
                return NotFound();
            }
        }

        //Take proSystem.IO.File pic from id
        [Route("fastuser/{id}")]
        [HttpGet]
        public IActionResult getUser(int id)
        {

            try
            {
                var utente = db.Utenti.Find(id);
                if (utente == null)
                    throw new ArgumentException();

                var stream = System.IO.File.OpenRead(_env.WebRootPath + "/Images/users/" + utente.Immagine + ".png");
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = stream.Length;
                return Ok(response);
            }
            catch
            {
                return NotFound();
            }
        }



        [Route("deletepic")]
        [HttpGet]
        public IActionResult deleteProfilePic()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get id from headers
            var _id = Utility.getUserId(Request);

            //Find user
            var utente = db.Utenti.Find(_id);
            if (utente == null)
                return NotFound();

            if (_id != utente.Id)
                return Unauthorized();


            //Delete image
            try
            {
                System.IO.File.Delete(_env.WebRootPath + "/images/users/" + utente.Immagine + ".png");
                utente.Immagine = "";
                db.SaveChanges();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
            return Ok();
        }

        [Route("upload/{path}/{id}")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostImmagine([FromForm] IFormFile file, string path, string id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Find user id
            var userId = Utility.getUserId(Request);
            var user = db.Utenti.Find(userId);

            if (path == "users")
            {
                //  Check if changing his own pic
                if (userId != Convert.ToInt32(id))
                    return Unauthorized();

                //Delete old pic
                try
                {
                    System.IO.File.Delete(_env.WebRootPath + "/images/users/" + user.Immagine + ".png");

                }
                catch { }

                //Generate imagename
                id = userId + "_" + Utility.italianTime().ToString("ddMMyyyyHHmmss");
                user.Immagine = id;
                db.SaveChanges();

                //Reduce image size if users

            }
            else
            {
                //Check if admin
                if (user.Stato < 2)
                    return Unauthorized();
            }

            var imagesFolder = _env.WebRootPath + $"/images/{path}/";
            var fullPath = Path.Combine(imagesFolder + id + ".png");

            //Resize image
            using (var image = Image.Load(file.OpenReadStream()))
            {
                image.Mutate(x =>
                x.Resize(
         new ResizeOptions()
         {
             Mode = ResizeMode.Max,
             Size = new Size() { Height = path == "users"? 300 : 1000 }
         }
     )
               );

                image.Save(fullPath);
            }


            return Ok();
        }


    }
}