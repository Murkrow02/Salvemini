using SalveminiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/secrets")]
    public class SecretsController : ApiController
    {
        DatabaseString db = new DatabaseString();


        [Route("postdomanda")]
        [HttpPost]
        public HttpResponseMessage postQuestion(string domanda_)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

                //Too long
                if (domanda_.Length > 3000)
                    return Request.CreateResponse(HttpStatusCode.RequestEntityTooLarge, "La domanda non puo' essere più lunga di 3000 caratteri");

                //Check if spamming
                var hisToday = db.Domande.Where(x => x.idUtente == userId && x.Creazione.Date == Helpers.Utility.italianTime().Date).Count();
                if(hisToday > 10)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Hai già inviato molte domande oggi, ritorna domani per poterne inviare una nuova. Ricorda che l'utilizzo scorretto di questa funzione potrebbe comportare la sospensione del tuo account");

                //Create domanda
                var domanda = new Domande();
                domanda.Domanda = domanda_;
                domanda.Approvata = false;
                domanda.Creazione = Helpers.Utility.italianTime();
                domanda.idUtente = userId;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "La tua domanda è stata pubblicata");
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore post domanda", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
        }

        [Route("postcommento")]
        [HttpPost]
        public HttpResponseMessage postCommento(Commenti commento_)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

                //Too long
                if (commento_.Commento.Length > 1000)
                    return Request.CreateResponse(HttpStatusCode.RequestEntityTooLarge, "Il commento non puo' essere più lungo di 1000 caratteri");

                //Check if spamming
                var today = Helpers.Utility.italianTime();
                var inThisHour = db.Commenti.Where(x => x.idUtente == userId && x.Creazione.Date == today.Date && x.Creazione.Hour == today.Hour && x.idPost == commento_.idPost).Count();
                if (inThisHour > 10)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Stai inviando troppi commenti per questo post, riprova più tardi. Ricorda che l'utilizzo scorretto di questa funzione potrebbe comportare la sospensione del tuo account");

                //Create comment
                commento_.Creazione = today;

                ////Increment impressions
                //var post = db.Domande.Find(commento_.idPost);
                //post.Impressions += 2;
   
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Il tuo commento è stato inviato");
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore post commento", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
        }



        [Route("removeCommento")]
        [HttpGet]
        public HttpResponseMessage removeComment(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
                var utente = db.Utenti.Find(userId);

                //Trova commento
                var commento = db.Commenti.Find(id);
                if(commento == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Il commento selezionato non è stato trovato, probabilmente è già stato rimosso");

                //L'ha creato lui?
                if(commento.idUtente != userId && utente.Stato != 3)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Non hai l'autorizzazione necessaria per rimuovere questo commento");

                db.Commenti.Remove(commento);
                db.SaveChanges();

                //Save event
                Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha rimosso il commento della domanda " + commento.idPost);
                return Request.CreateResponse(HttpStatusCode.OK, "Il tuo commento è stato rimosso");
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore remove commento", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
            }


        [Route("feed/{id}")]
        [HttpGet]
        public List<DomandeReturn> getFeed(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Get all posts
                var posts = db.Domande.Where(x => x.Approvata).OrderByDescending(x => x.Creazione).ToList();

                //Remove already viewed posts if needed
                if(id != -1)
                {
                    var lastPost = db.Domande.Find(id);
                    var lastPostIndex = posts.IndexOf(lastPost);
                    posts = posts.Skip(lastPostIndex + 1).ToList();
                }

                //Take a maximum of 30 posts
                if (posts.Count > 30)
                    posts.Take(30);

                //Create returnable model
                var returnModel = new List<DomandeReturn>();
                foreach(var domanda in posts)
                {
                    //Get first 2 comments
                    var commenti = db.Commenti.Where(x => x.idPost == domanda.id).OrderByDescending(x => x.Creazione).Take(2).ToList();
                    var commentiCount = db.Commenti.Where(x => x.idPost == domanda.id).Count();//commenti.Count();
                 //   commenti.OrderByDescending(x => x.Creazione).Take(2).ToList();

                    //Censura anonimi
                    var filteredCommenti = new List<Commenti>();
                    foreach (var commento in commenti) { filteredCommenti.Add(new Commenti { Commento = commento.Commento, Creazione = commento.Creazione, Utenti = commento.Anonimo ? null : commento.Utenti }); }
                    returnModel.Add(new DomandeReturn { Commenti = filteredCommenti, Data = domanda.Creazione, Domanda = domanda.Domanda, id = domanda.id, CommentiCount = commentiCount });
                }

                return returnModel;
            }
            catch(Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore feed domande", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            }

        [Route("commenti/{id}")]
        [HttpGet]
        public List<Commenti> getCommenti(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Get all comments
                var commenti = db.Commenti.Where(x => x.idPost == id).OrderByDescending(x => x.Creazione).ToList();

                //Take a maximum of 1000 comments
                if (commenti.Count > 1000)
                    commenti.Take(1000);

                //Censura anonimi
                var filteredCommenti = new List<Commenti>();
                foreach (var commento in commenti) { filteredCommenti.Add(new Commenti { Commento = commento.Commento, Creazione = commento.Creazione, Utenti = commento.Anonimo ? null : commento.Utenti }); }

                return filteredCommenti;
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore feed commenti post " + id, ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }



    }
}
