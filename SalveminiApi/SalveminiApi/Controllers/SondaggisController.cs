﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SalveminiApi.Models;
using SalveminiApi.OneSignalApi;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/sondaggi")]
    public class SondaggiController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("all")]
        [HttpGet]
        public List<Sondaggi> GetSondaggi()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            return db.Sondaggi.ToList();
        }

        [Route("")]
        [HttpPost]
        public HttpResponseMessage PostSondaggio(Sondaggi sondaggio)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, true);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Deactive others
            foreach (var sondaggio_ in db.Sondaggi)
            {
                sondaggio_.Attivo = false;
            }

            //Set current time to creation and active poll
            sondaggio.Creazione = Helpers.Utility.italianTime();
            sondaggio.Attivo = true;

            db.Sondaggi.Add(sondaggio);
            db.SaveChanges();

            //Invia notifica se necessario
            try
            {
                var mittente = db.Utenti.Find(sondaggio.Creatore);
                var notifica = new NotificationModel();
                var titolo = new Localized { en = "Nuovo sondaggio, apri l'app per votare" };
                var dettagli = new Localized { en = sondaggio.Nome};
                //var filter = new Tags { field = "tag", key = "SchedaId", relation = "=", value = ListaUtenti.SelectedValue };
                //var tags = new List<Tags>();
                //tags.Add(filter);
                notifica.headings = titolo;
                notifica.contents = dettagli;
                //notifica.filters = tags;
                NotificationService.sendNotification(notifica);
            }
            catch
            {
                //Errore nell inviare la notifica, ma fa niente lol avviso creato con successo
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("vota")]
        [HttpPost]
        public HttpResponseMessage PostSondaggio(VotiSondaggi voto)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                var alreadyVoted = db.VotiSondaggi.Where(x => x.idSondaggio == voto.idSondaggio && x.Utente == voto.Utente).ToList();
                if(alreadyVoted.Count > 0)
                    return new HttpResponseMessage(HttpStatusCode.Conflict); //Already voted

                db.VotiSondaggi.Add(voto);
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Route("risultati/{id}")]
        [HttpGet]
        public List<SondaggiResult> ReturnRisultati(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Create new dictionary
            var Risultati = new List<SondaggiResult>();
            
            try
            {
                //Find sondaggio
                var sondaggio = db.Sondaggi.Find(id);

                //Get voti sondaggio
                var votiSondaggio = db.VotiSondaggi.Where(x => x.idSondaggio == id).ToList();

                //Count each vote
                foreach(var opzione in sondaggio.OggettiSondaggi)
                {
                    var risultato = new SondaggiResult();

                    //Conta voti opzione
                    int contoVoti = votiSondaggio.Where(x => x.Voto == opzione.id).Count();
                    risultato.Voti = contoVoti;

                    //Prendi nome opzione
                    risultato.NomeOpzione = opzione.Nome;

                    //Calcola percentuale
                    if (contoVoti == 0)
                        risultato.Percentuale = 0;
                    else
                    {
                        var percentuale = contoVoti * 100 / votiSondaggio.Count;
                        risultato.Percentuale = percentuale;
                    }

                    //Add result to result list
                    Risultati.Add(risultato);
                }

                return Risultati;
               
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
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