using SalveminiApi.Models;
using SalveminiApi.OneSignalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/icringe")]
    public class SecretsController : ApiController
    {
        DatabaseString db = new DatabaseString();


        [Route("postdomanda")]
        [HttpPost]
        public HttpResponseMessage postQuestion([FromBody] string domanda_)
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
                var today = Helpers.Utility.italianTime();
                var hisToday = db.Domande.ToList().Where(x => x.idUtente == userId && x.Creazione.ToString("ddMMyyyy") == today.ToString("ddMMyyyy")).Count();
                if (hisToday > 10)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Hai già inviato molte domande oggi, ritorna domani per poterne inviare una nuova. Ricorda che l'utilizzo scorretto di questa funzione potrebbe comportare la sospensione del tuo account");

                //Create domanda
                var domanda = new Domande();
                domanda.Domanda = domanda_;
                domanda.Anonimo = false;
                //domanda.Approvata = false;
                domanda.Approvata = true;
                domanda.Creazione = Helpers.Utility.italianTime();
                domanda.idUtente = userId;
                db.Domande.Add(domanda);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Il tuo post è stato pubblicato, ricorda che se hai scelto di pubblicare in forma anonima gli utenti NON potranno vedere in nessun modo che sei stato tu a crearlo. Inviando post non appropriati il tuo account potrebbe essere disabilitato.");
                //return Request.CreateResponse(HttpStatusCode.OK, "Il tuo post è stato inviato e verrà controllato prima di essere pubblicato. Ricorda che se hai scelto di pubblicare in forma anonima i moderatori NON potranno vedere che sei stato tu ad inviare questo post");
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
                var utente = db.Utenti.Find(userId);
                var post = db.Domande.Find(commento_.idPost);
                if(utente == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "L' utente che vuole postare il commento non è stato trovato");
                if (post == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Il post che vuoi commentare non è stato trovato");

                //Too long
                if (commento_.Commento.Length > 1000)
                    return Request.CreateResponse(HttpStatusCode.RequestEntityTooLarge, "Il commento non puo' essere più lungo di 1000 caratteri");

                ////Check if spamming
                var today = Helpers.Utility.italianTime();
                var inThisHour = db.Commenti.ToList().Where(x => x.idUtente == userId && x.Creazione.Date == today.Date && x.Creazione.Hour == today.Hour && x.idPost == commento_.idPost).Count();
                if (inThisHour > 10)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Stai inviando troppi commenti per questo post, riprova più tardi. Ricorda che l'utilizzo scorretto di questa funzione potrebbe comportare la sospensione del tuo account");

                //Create comment
                bool anonimo = commento_.Anonimo;
                commento_.Creazione = today;
                commento_.idUtente = userId;

                ////Increment impressions
                //var post = db.Domande.Find(commento_.idPost);
                //post.Impressions += 2;


                //Push notification
                if(commento_.idUtente != post.idUtente)
                {

                    //Send notification to post creator feed
                    string desc = anonimo ? "Un utente anonimo ha commentato il tuo post" : utente.Nome + " " + utente.Cognome + " ha commentato il tuo post";
                    if (commento_.idUtente != post.idUtente) //Do not add if comment creator = post creator
                        db.Notifiche.Add(new Notifiche { Creazione = Helpers.Utility.italianTime(), Descrizione = desc, idPost = commento_.idPost, Tipo = 2, idUtente = post.idUtente });

                    //Create push
                    var notifica = new NotificationModel();
                    var titolo = new Localized { en = "iCringe" };
                    var dettagli = new Localized { en = desc };
                    var filter = new Tags { field = "tag", key = "Secrets", relation = "=", value = post.idUtente.ToString() };
                    var tags = new List<Tags>();
                    var data = new AdditionalData { tipo = "push", id = "iCringe"}; tags.Add(filter);
                    notifica.headings = titolo;
                    notifica.contents = dettagli;
                    notifica.data = data;
                    notifica.filters = tags;
                    NotificationService.sendNotification(notifica);
                }

                db.Commenti.Add(commento_);
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



        [Route("removeCommento/{id}")]
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
                //Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha rimosso il commento della domanda " + commento.idPost);
                return Request.CreateResponse(HttpStatusCode.OK, "Il tuo commento è stato rimosso");
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore remove commento", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
            }

        [Route("removePost/{id}")]
        [HttpGet]
        public HttpResponseMessage removePost(int id)
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
                var post = db.Domande.Find(id);
                if (post == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Il post selezionato non è stato trovato, probabilmente è già stato rimosso");

                //L'ha creato lui?
                if (post.idUtente != userId && utente.Stato != 3)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Non hai l'autorizzazione necessaria per rimuovere questo commento");

                //Rimuovi notifiche
                var rimuoviNotifiche = db.Notifiche.Where(x => x.idPost == id);
                db.Notifiche.RemoveRange(rimuoviNotifiche);

                //Rimuovi post
                db.Domande.Remove(post);
                db.SaveChanges();

                //Save event
                Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha rimosso una domanda da iCringe");
                return Request.CreateResponse(HttpStatusCode.OK, "Il tuo post è stato rimosso");
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
                var posts = db.Domande.OrderByDescending(x => x.Creazione).Take(5000).ToList();

                //Remove already viewed posts if needed
                if(id != -1)
                {
                    var lastPost = db.Domande.Find(id);
                    var lastPostIndex = posts.IndexOf(lastPost);
                    posts = posts.Skip(lastPostIndex + 1).ToList();
                }

                //Take a maximum of 30 posts
                if (posts.Count > 30)
                  posts = posts.Take(30).ToList();

                //Create returnable model
                var returnModel = new List<DomandeReturn>();
                foreach(var domanda in posts)
                {
                    //Get user if not anonymous
                    var utente = new Utenti();
                    if (!domanda.Anonimo)
                        utente = db.Utenti.Find(domanda.idUtente);
                    else
                        utente = null;

                    //Get first 2 comments
                    var commenti = db.Commenti.Where(x => x.idPost == domanda.id).OrderByDescending(x => x.Creazione).Take(2).ToList();
                    var commentiCount = db.Commenti.Where(x => x.idPost == domanda.id).Count();
                    //commenti.Count();                                   
                    //   commenti.OrderByDescending(x => x.Creazione).Take(2).ToList();

                    var dataOra = Helpers.Utility.italianTime();
                    var tempoFa = dataOra - domanda.Creazione;

                    //Censura anonimi
                    var filteredCommenti = new List<Commenti>();
                    foreach (var commento in commenti) { filteredCommenti.Add(new Commenti { Commento = commento.Commento, Creazione = commento.Creazione, Utenti = commento.Anonimo ? null : commento.Utenti }); }
                    returnModel.Add(new DomandeReturn { Commenti = filteredCommenti, Data = tempoFa, Domanda = domanda.Domanda, id = domanda.id, CommentiCount = commentiCount,Utente = utente });
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
        public CommentiReturn getCommenti(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Find post
                var post = db.Domande.Find(id);
                if(post == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                //Get all comments
                var commenti = db.Commenti.Where(x => x.idPost == id).OrderByDescending(x => x.Creazione).ToList();

                //Take a maximum of 1000 comments
                if (commenti.Count > 1000)
                    commenti.Take(1000);

                //Censura anonimi
                var returnModel = new CommentiReturn();
                var filteredCommenti = new List<Commenti>();
              
                foreach (var commento in commenti) { filteredCommenti.Add(new Commenti { Commento = commento.Commento, Creazione = commento.Creazione, Utenti = commento.Anonimo ? null : commento.Utenti, id = commento.id }); }

                returnModel.Domanda = post.Domanda;
                returnModel.Commenti = filteredCommenti;
                return returnModel;
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore feed commenti post " + id, ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("getnotifiche")]
        [HttpGet]
        public List<Notifiche> getNotifiche()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

            try
            {
                return db.Notifiche.Where(x=>x.idUtente == userId).OrderByDescending(x => x.Creazione).Take(50).ToList();
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore GET notifiche iCringe " + userId, ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("getnewnotifiche/{id}")]
        [HttpGet]
        public List<NewNotifiche> getNewNotifiche(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

            try
            {
                var notifiche = db.Notifiche.Where(x => x.idUtente == userId).OrderBy(x => x.Creazione).Take(100).ToList();
                var lastNotifica = db.Notifiche.Find(id);
                var lastNotificaIndex = notifiche.IndexOf(lastNotifica);
                notifiche = notifiche.Skip(lastNotificaIndex + 1).ToList();

                //Add only the count of each noticiation type
                var returnModel = new List<NewNotifiche>();
                var nonApprovato = notifiche.Where(x => x.Tipo == 0).Count();
                var approvato = notifiche.Where(x => x.Tipo == 1).Count();
                var commenti = notifiche.Where(x => x.Tipo == 2).Count();
                returnModel.Add(new NewNotifiche { Tipo = 0, Count = nonApprovato > 9 ? "9+" : nonApprovato.ToString() });
                returnModel.Add(new NewNotifiche { Tipo = 1, Count = approvato > 9 ? "9+" : approvato.ToString() });
                returnModel.Add(new NewNotifiche { Tipo = 2, Count = commenti > 9 ? "9+" : commenti.ToString() });
                return returnModel;
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore GET new notifiche iCringe " + userId, ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("approvelist")]
        [HttpGet]
        public List<Domande> approveList()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request,2);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                var domande = db.Domande.Where(x => x.Approvata == false).OrderBy(x => x.Creazione).ToList();
                return domande;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("approva/{id}/{approved}")]
        [HttpGet]
        public HttpResponseMessage approveDomande(int id, bool approved)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 3);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Find requested domanda
                var domanda = db.Domande.Find(id);
                if(domanda == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "La domanda selezionata non è stata trovata, probabilmente è già stata rimossa");

                //Approve or delete
                if (approved)
                    domanda.Approvata = true;
                else
                    db.Domande.Remove(domanda);

                //Send notification to post creator feed
                
                string desc = approved ?  "Il tuo post è stato approvato" : "Il tuo post non è stato approvato";
                db.Notifiche.Add(new Notifiche { Creazione = Helpers.Utility.italianTime(), Descrizione = desc, idPost = approved ? domanda.id : -1, Tipo = approved ? 1 : 0, idUtente = domanda.idUtente });

                //Push notification
                var notifica = new NotificationModel();
                var titolo = new Localized { en = "iCringe" };
                var dettagli = new Localized { en = desc };
                var filter = new Tags { field = "tag", key = "Secrets", relation = "=", value = domanda.idUtente.ToString() };
                var tags = new List<Tags>();
                //var data = new AdditionalData { tipo = "chatIstr", id = admin.id };
                tags.Add(filter);
                var data = new AdditionalData { tipo = "push", id = "iCringe" }; tags.Add(filter);
                notifica.headings = titolo;
                notifica.contents = dettagli;
                notifica.data = data;
                NotificationService.sendNotification(notifica);

                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "La domanda è stata " + (approved ? "approvata" : "rifiutata"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
        }



    }
}
