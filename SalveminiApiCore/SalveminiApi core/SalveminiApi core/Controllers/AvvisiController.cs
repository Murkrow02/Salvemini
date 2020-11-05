using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using SalveminiApi_core.OneSignalApi;

namespace SalveminiApi_core.Controllers
{
    [Route("api/avvisi")]
    [ApiController]
    public class AvvisiController : ControllerBase
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private readonly Salvemini_DBContext db; public AvvisiController(Salvemini_DBContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) { db = context; _env = env; }

        [Route("all")]
        [HttpGet]
        public IActionResult getAvvisi()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi avvisi
            try
            {
                var avvisi = db.Avvisi.OrderByDescending(x => x.Creazione).ToList();

                //Aggiungi visualizzazione ad analytics
                if (avvisi == null || avvisi.Count < 1)
                    return NotFound();

                return Ok(avvisi);
            }
            catch(Exception ex)
            {
                throw new ArgumentException();
            }
         
        }

        [Route("")]
        [HttpPost]
        public IActionResult addAvviso(Avvisi avviso)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db,2);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int idUtente = Utility.getUserId(Request);

         
            try
            {
                //Aggiungi avviso al database
                avviso.Creazione = Utility.italianTime();
                db.Avvisi.Add(avviso);
                //Remove previous avvisi visual
                try { db.Analytics.ToList().RemoveAll(x => x.Tipo == "UltimoAvviso"); } catch { }
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new ArgumentException();

            }

            //Find user creating avviso
            var mittente = db.Utenti.Find(avviso.IdCreatore);


            //Invia notifica se necessario
            if (avviso.SendNotification)
            {
                try
                {
                    var notifica = new NotificationModel();
                    var titolo = new Localized { en = avviso.Titolo };
                    var dettagli = new Localized { en = "Nuovo avviso da " + mittente.Nome + " " + mittente.Cognome + ", apri l'app per saperne di più!" };
                    var data = new AdditionalData { tipo = "push", id = "Avvisi" }; 
                    notifica.headings = titolo;
                    notifica.contents = dettagli;
                    notifica.data = data;
                    NotificationService.sendNotification(notifica);
                }
                catch
                {
                    //Errore nell inviare la notifica, ma fa niente lol avviso creato con successo
                    return Ok();
                }
            }

            return Ok();
        }

        [Route("delete/{id}")]
        [HttpGet]
        public IActionResult deleteAvviso(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db, 3);
            if (!authorized)
                return Unauthorized();


            //Prendi avviso da eliminare
            var avviso = db.Avvisi.Find(id);

            //Avviso non trovato
            if (avviso == null)
                return NotFound();
           
            try
            {
                //Elimina avviso
                db.Avvisi.Remove(avviso);

                //Delete images
                var images = avviso.Immagini.Split(',').ToList();
                foreach(var image in images)
                {
                  System.IO.File.Delete(_env.WebRootPath + "/Images/avvisi/" + image + ".png");
                }
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new ArgumentException();
            }

            return Ok();
        }

 
    }
}
