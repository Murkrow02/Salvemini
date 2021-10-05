using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core;
using SalveminiApi_core.Models;
using SalveminiApi_core.OneSignalApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SalveminiApi_core.Controllers
{
    [Route("api/sondaggi")]
    [ApiController]
    public class SondaggiController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public SondaggiController(Salvemini_DBContext context) { db = context; }

        [Route("all")]
        [HttpGet]
        public IActionResult GetSondaggi()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            return Ok(db.Sondaggi.ToList());
        }

        [Route("")]
        [HttpPost]
        public IActionResult PostSondaggio(Sondaggi sondaggio)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Deactive others
            foreach (var sondaggio_ in db.Sondaggi)
            {
                sondaggio_.Attivo = false;
            }

            //Set current time to creation and active poll
            sondaggio.Creazione = Utility.italianTime();
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
                notifica.headings = titolo;
                notifica.contents = dettagli;
                NotificationService.sendNotification(notifica);
            }
            catch
            {
                //Errore nell inviare la notifica, ma fa niente lol avviso creato con successo
                return Ok();
            }


            return Ok();
        }

        [Route("vota")]
        [HttpPost]
        public IActionResult PostSondaggio(VotiSondaggi voto)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Find sondaggio
            var sondaggio = db.Sondaggi.Find(voto.IdSondaggio);
            if (sondaggio == null)
                return NotFound();

            try
            {
                //Check if already voted
                var alreadyVoted = db.VotiSondaggi.Where(x => x.IdSondaggio == voto.IdSondaggio && x.Utente == voto.Utente).ToList();
                if (alreadyVoted.Count > 0)
                    return Conflict();
 
                //Add new voto to db
                db.VotiSondaggi.Add(voto);
                db.SaveChanges();

                //Send signalR voti update
                try
                {
                    //var Hub = new Hubs.SondaggiHub();
                    //Hub.NewVoto();
#warning do this
                }
                catch { }
                return Ok();
            }
            catch(Exception ex)
            {
                throw new ArgumentException();
            }
        }

        [Route("risultati/{id}")]
        [HttpGet]
        public IActionResult ReturnRisultati(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Create new dictionary
            var Risultati = new List<SondaggiResult>();

            //Find sondaggio
            var sondaggio = db.Sondaggi.Find(id);
            if (sondaggio == null)
                return NotFound();//Sondaggio not found

            try
            {
                //Get voti sondaggio
                var votiSondaggio = db.VotiSondaggi.Where(x => x.IdSondaggio == id).ToList();

                //Count each vote
                foreach(var opzione in sondaggio.OggettiSondaggi)
                {
                    var risultato = new SondaggiResult();

                    //Conta voti opzione
                    int contoVoti = votiSondaggio.Where(x => x.Voto == opzione.Id).Count();
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
              
                return Ok(Risultati);
               
            }
            catch(Exception ex)
            {
                throw new ArgumentException();
            }
        }

      

       
    }
}