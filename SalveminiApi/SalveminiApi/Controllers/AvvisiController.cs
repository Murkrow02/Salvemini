﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Helpers.Utility.saveCrash("Errore aggiunta avviso", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            //Invia notifica se necessario
            if (avviso.SendNotification)
            {
                try
                {
                    var mittente = db.Utenti.Find(avviso.idCreatore);
                    var notifica = new NotificationModel();
                    var titolo = new Localized { en = avviso.Titolo };
                    var dettagli = new Localized { en = "Nuovo avviso da " + mittente.Nome + " " + mittente.Cognome + ", apri l'app per saperne di più!" };
                    notifica.headings = titolo;
                    notifica.contents = dettagli;
                    NotificationService.sendNotification(notifica);
                }
                catch
                {
                    //Errore nell inviare la notifica, ma fa niente lol avviso creato con successo
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
           

            //Add to console log
            Helpers.Utility.saveEvent(avviso.Utenti.Nome + "(" + avviso.idCreatore + ")" + " ha creato l'avviso " + avviso.Titolo + "(" + avviso.id + ")");

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("delete/{id}")]
        [HttpDelete]
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
