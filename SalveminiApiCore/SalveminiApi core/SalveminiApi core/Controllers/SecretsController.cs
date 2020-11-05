using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using SalveminiApi_core.OneSignalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SalveminiApi_core.Controllers
{
    [Route("api/icringe")]
    [ApiController]
    public class SecretsController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public SecretsController(Salvemini_DBContext context) { db = context; }

        [Route("postdomanda")]
        [HttpPost]
        public IActionResult postQuestion([FromBody] string domanda_)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Utility.getUserId(Request));

                //Too long
                if (domanda_.Length > 3000)
                    return Forbid("La domanda non puo' essere più lunga di 3000 caratteri");

                //Check if spamming
                var today = Utility.italianTime();
                var hisToday = db.Domande.ToList().Where(x => x.IdUtente == userId && x.Creazione.ToString("ddMMyyyy") == today.ToString("ddMMyyyy")).Count();
                if (hisToday > 10)
                    return Forbid("Hai già inviato molte domande oggi, ritorna domani per poterne inviare una nuova. Ricorda che l'utilizzo scorretto di questa funzione potrebbe comportare la sospensione del tuo account");

                //Create domanda
                var domanda = new Domande();
                domanda.Domanda = domanda_;
                domanda.Anonimo = false;
                //domanda.Approvata = false;
                domanda.Approvata = true;
                domanda.Creazione = Utility.italianTime();
                domanda.IdUtente = userId;
                db.Domande.Add(domanda);
                db.SaveChanges();
                return Ok("Il tuo post è stato pubblicato");
                //return Request.CreateResponse(HttpStatusCode.OK, "Il tuo post è stato inviato e verrà controllato prima di essere pubblicato. Ricorda che se hai scelto di pubblicare in forma anonima i moderatori NON potranno vedere che sei stato tu ad inviare questo post");
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }

        [Route("postcommento")]
        [HttpPost]
        public IActionResult postCommento([FromBody]Commenti commento_)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Utility.getUserId(Request));
                var utente = db.Utenti.Find(userId);
                var post = db.Domande.Find(commento_.IdPost);
                if (utente == null)
                    return NotFound("L' utente che vuole postare il commento non è stato trovato");
                if (post == null)
                    return NotFound("Il post che vuoi commentare non è stato trovato");

                //Too long
                if (commento_.Commento.Length > 1000)
                    return Forbid("Il commento non puo' essere più lungo di 1000 caratteri");

                //Check if spamming
                var today = Utility.italianTime();
                var inThisHour = db.Commenti.ToList().Where(x => x.IdUtente == userId && x.Creazione.Date == today.Date && x.Creazione.Hour == today.Hour && x.IdPost == commento_.IdPost).Count();
                if (inThisHour > 10)
                    return Forbid("Stai inviando troppi commenti per questo post, riprova più tardi. Ricorda che l'utilizzo scorretto di questa funzione potrebbe comportare la sospensione del tuo account");

                //Create comment
                commento_.Creazione = today;
                commento_.Anonimo = false;
                commento_.IdUtente = userId;

                ////Increment impressions
                //var post = db.Domande.Find(commento_.IdPost);
                //post.Impressions += 2;


                //Push notification
                if (commento_.IdUtente != post.IdUtente)
                {

                    //Send notification to post creator feed
                    string desc =  utente.Nome + " " + utente.Cognome + " ha commentato il tuo post";
                    if (commento_.IdUtente != post.IdUtente) //Do not add if comment creator = post creator
                        db.Notifiche.Add(new Notifiche { Creazione = Utility.italianTime(), Descrizione = desc, IdPost = commento_.IdPost, Tipo = 2, IdUtente = post.IdUtente });

                    //Create push
                    var notifica = new NotificationModel();
                    var titolo = new Localized { en = "iCringe" };
                    var dettagli = new Localized { en = desc };
                    var filter = new Tags { field = "tag", key = "Secrets", relation = "=", value = post.IdUtente.ToString() };
                    var tags = new List<Tags>();
                    var data = new AdditionalData { tipo = "push", id = "iCringe" }; tags.Add(filter);
                    notifica.headings = titolo;
                    notifica.contents = dettagli;
                    notifica.data = data;
                    notifica.filters = tags;
                    NotificationService.sendNotification(notifica);
                }

                db.Commenti.Add(commento_);
                db.SaveChanges();

                return Ok( "Il tuo commento è stato inviato");
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }



        [Route("removeCommento/{id}")]
        [HttpGet]
        public IActionResult removeComment(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Utility.getUserId(Request));
                var utente = db.Utenti.Find(userId);

                //Trova commento
                var commento = db.Commenti.Find(id);
                if (commento == null)
                    return NotFound( "Il commento selezionato non è stato trovato, probabilmente è già stato rimosso");

                //L'ha creato lui?
                if (commento.IdUtente != userId && utente.Stato != 3)
                    return Forbid("Non hai l'autorizzazione necessaria per rimuovere questo commento");

                db.Commenti.Remove(commento);
                db.SaveChanges();

                //Save event
                //Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha rimosso il commento della domanda " + commento.IdPost);
                return Ok( "Il tuo commento è stato rimosso");
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }

        [Route("removePost/{id}")]
        [HttpGet]
        public IActionResult removePost(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Utility.getUserId(Request));
                var utente = db.Utenti.Find(userId);

                //Trova commento
                var post = db.Domande.Find(id);
                if (post == null)
                    return NotFound("Il post selezionato non è stato trovato, probabilmente è già stato rimosso");

                //L'ha creato lui?
                if (post.IdUtente != userId && utente.Stato != 3)
                    return Forbid("Non hai l'autorizzazione necessaria per rimuovere questo commento");

                //Rimuovi notifiche
                var rimuoviNotifiche = db.Notifiche.Where(x => x.IdPost == id);
                db.Notifiche.RemoveRange(rimuoviNotifiche);

                //Rimuovi post
                db.Domande.Remove(post);
                db.SaveChanges();

                //Save event
                Utility.saveEvent(Request, db, utente.Nome + " " + utente.Cognome + " (" + utente.Id + ")" + " ha rimosso una domanda da iCringe");
                return Ok("Il tuo post è stato rimosso");
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }


        [Route("feed/{id}")]
        [HttpGet]
        public IActionResult getFeed(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Get all posts
                var posts = db.Domande.OrderByDescending(x => x.Creazione).Take(5000).ToList();

                //Remove already viewed posts if needed
                if (id != -1)
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
                foreach (var domanda in posts)
                {
                    //Get user if not anonymous
                    var utente = new Utenti();
                    if (!domanda.Anonimo)
                        utente = db.Utenti.Find(domanda.IdUtente);
                    else
                        utente = null;

                    //Get first 2 comments
                    var commenti = db.Commenti.Where(x => x.IdPost == domanda.Id).OrderByDescending(x => x.Creazione).Take(2).ToList();
                    var commentiCount = db.Commenti.Where(x => x.IdPost == domanda.Id).Count();
                    //commenti.Count();                                   
                    //   commenti.OrderByDescending(x => x.Creazione).Take(2).ToList();

                    var dataOra = Utility.italianTime();
                    var tempoFa = dataOra - domanda.Creazione;

                    //Censura anonimi
                    var filteredCommenti = new List<Commenti>();
                    foreach (var commento in commenti) { filteredCommenti.Add(new Commenti { Commento = commento.Commento, Creazione = commento.Creazione, IdUtenteNavigation = commento.Anonimo ? null : commento.IdUtenteNavigation }); }
                    returnModel.Add(new DomandeReturn { Commenti = filteredCommenti, Data = Utility.SpanString(tempoFa), Domanda = domanda.Domanda, id = domanda.Id, CommentiCount = commentiCount, Utente = utente });
                }

                return Ok(returnModel);
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }

        [Route("commenti/{id}")]
        [HttpGet]
        public IActionResult getCommenti(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Find post
                var post = db.Domande.Find(id);
                if (post == null)
                    return NotFound();

                //Get all comments
                var commenti = db.Commenti.Where(x => x.IdPost == id).OrderByDescending(x => x.Creazione).Take(1000).ToList();
            
                return Ok(new CommentiReturn { Commenti = commenti, Domanda = post.Domanda });
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }

        [Route("getnotifiche")]
        [HttpGet]
        public IActionResult getNotifiche()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var userId = Convert.ToInt32(Utility.getUserId(Request));

            try
            {
                return Ok(db.Notifiche.Where(x => x.IdUtente == userId).OrderByDescending(x => x.Creazione).Take(50).ToList());
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }

        [Route("getnewnotifiche/{id}")]
        [HttpGet]
        public IActionResult getNewNotifiche(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var userId = Convert.ToInt32(Utility.getUserId(Request));

            try
            {
                var notifiche = db.Notifiche.Where(x => x.IdUtente == userId).OrderBy(x => x.Creazione).Take(100).ToList();
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
                return Ok(returnModel);
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }

      



    }
}
