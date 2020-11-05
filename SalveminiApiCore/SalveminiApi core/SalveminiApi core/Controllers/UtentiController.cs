using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SalveminiApi_core.Controllers
{
    [Route("api/utenti")]
    [ApiController]
    public class UtentiController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public UtentiController(Salvemini_DBContext context) { db = context; }

        [Route("all")]
        [HttpGet]
        public IActionResult getAllUtenti()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi tutti gli utenti
            var utenti = db.Utenti.ToList();

            return Ok(utenti);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult getUtente(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi utente selezionato
            var utente = db.Utenti.Find(id);


            if (utente == null)
                return NotFound();

            return Ok(utente);
        }

        [Route("change/{id}/{status}")]
        [HttpGet]
        public IActionResult changeStatus(int id, int status)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi utente selezionato
            var utente = db.Utenti.Find(id);
            if (utente == null)
                return NotFound();

            //Cambia stato utente 
            utente.Stato = status;
            db.SaveChanges();

            //Add to console log
            Utility.saveEvent(Request, db, "L'utente " + utente.Nome + " " + utente.Cognome + " (" + utente.Id + ")" + " è passato allo stato " + status);

            return Ok();
        }

        [Route("classe")]
        [HttpGet]
        public IActionResult getClassUtenti()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi classe dell utente selezionato
            var id = Utility.getUserId(Request);
            var utente = db.Utenti.Find(id);
            if (utente == null)
                return NotFound();
            var classe = utente.Classe.ToString() + utente.Corso;

            //Prendi tutti gli utenti di quella classe
            var utenti = db.Utenti.Where(x => x.Classe.ToString() + x.Corso == classe).ToList();

            return Ok(utenti);
        }
    }
}
