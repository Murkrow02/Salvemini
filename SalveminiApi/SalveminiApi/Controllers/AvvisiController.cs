using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SalveminiApi.Argo.Models;
using SalveminiApi.Models;
using SalveminiApi.OneSignalApi;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/avvisi")]
    public class AvvisiController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("all")]
        [HttpGet]
        public List<Avvisi> getAvvisi()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

           

            //Prendi avvisi
            try
            {
                var avvisi = db.Avvisi.OrderByDescending(x => x.Creazione).ToList();

                //Aggiungi visualizzazione ad analytics
                if (avvisi == null || avvisi.Count < 1)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                //Add avviso view to analytics
                Helpers.Utility.addToAnalytics("UltimoAvviso");

                return avvisi;
            }
            catch(Exception ex)
            {
              Helpers.Utility.saveCrash("Errore get lista avvisi", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
         
        }

        [Route("")]
        [HttpPost]
        public HttpResponseMessage addAvviso(Avvisi avviso)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 2);
            if (!authorized)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            //Get parameters from request
            int idUtente = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

         
            try
            {
                //Aggiungi avviso al database
                avviso.Creazione = Helpers.Utility.italianTime();
                db.Avvisi.Add(avviso);
                //Remove previous avvisi visual
                try { db.Analytics.ToList().RemoveAll(x => x.Tipo == "UltimoAvviso"); } catch { }
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Helpers.Utility.saveCrash("Errore aggiunta avviso", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            //Find user creating avviso
            var mittente = db.Utenti.Find(avviso.idCreatore);


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
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }

            //Remove previous avviso analytics
            Helpers.Utility.clearAnalytics("UltimoAvviso");

            //Add to console log
            Helpers.Utility.saveEvent(mittente.Nome + "(" + avviso.idCreatore + ")" + " ha creato l'avviso " + avviso.Titolo + "(" + avviso.id + ")");

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("delete/{id}")]
        [HttpGet]
        public HttpResponseMessage deleteAvviso(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 3);
            if (!authorized)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);


            //Prendi avviso da eliminare
            var avviso = db.Avvisi.Find(id);

            //Avviso non trovato
            if(avviso == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
           
            try
            {
                //Elimina avviso
                db.Avvisi.Remove(avviso);

                //Delete images
                var images = avviso.Immagini.Split(',').ToList();
                foreach(var image in images)
                {
                    File.Delete(HttpContext.Current.Server.MapPath("~/Images/avvisi/" + image + ".png"));
                }
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                //Save error crash
                Helpers.Utility.saveCrash("Error deleting avviso " + id, ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
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
