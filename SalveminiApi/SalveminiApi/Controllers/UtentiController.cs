using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SalveminiApi.Models;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/utenti")]
    public class UtentiController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("all")]
        [HttpGet]
        public List<Utenti> getAllUtenti()
        {
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 2);
            if (!authorized)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            //Prendi tutti gli utenti
            var utenti = db.Utenti.ToList();

            return utenti;
        }

        [Route("{id}")]
        [HttpGet]
        public Utenti getUtente(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            //Prendi utente selezionato
            var utente = db.Utenti.Find(id);

            if(utente == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return utente;
        }

        [Route("change/{id}/{status}")]
        [HttpGet]
        public HttpResponseMessage changeStatus(int id, int status)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request,2);
            if (!authorized)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            //Prendi utente selezionato
            var utente = db.Utenti.Find(id);
            if (utente == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            //Cambia stato utente 
            utente.Stato = status;
            db.SaveChanges();


            //Add to console log
            Helpers.Utility.saveEvent("L'utente " + utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " è passato allo stato " + status);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("classe")]
        [HttpGet]
        public List<Utenti> getClassUtenti()
        {
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 2);
            if (!authorized)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            //Prendi classe dell utente selezionato
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var utente = db.Utenti.Find(id);
            if(utente == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            var classe = utente.Classe.ToString() + utente.Corso;

            //Prendi tutti gli utenti di quella classe
            var utenti = db.Utenti.Where(x => x.Classe.ToString() + x.Corso == classe).ToList();

            return utenti;
        }
    }
}
