using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalveminiApi_core.Models;
namespace SalveminiApi_core.Controllers
{
    [Route("api/ads")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public AdsController(Salvemini_DBContext context) { db = context; }

        [Route("canWatchAd")]
        [HttpGet]
        public IActionResult canWatchAd()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Utility.getUserId(Request);
            var utente = db.Utenti.Find(id);
            if (utente == null) return NotFound(); //User not found

            //Check if is new day
            if (!utente.LastAdWatched.HasValue || utente.LastAdWatched.Value.ToString("dd-MM-yyyy") != Utility.italianTime().ToString("dd-MM-yyyy"))
            {
                utente.LastAdWatched = Utility.italianTime();
                utente.AdsWatched = 0;
                db.SaveChanges();
            }

            //Check how many ads watched
            if (utente.AdsWatched <= 3)
                return Ok(utente.AdsWatched);
            else
                return StatusCode(406);
        }

        [Route("watchedAd")]
        [HttpGet]
        public IActionResult watchedAd()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Utility.getUserId(Request);
            var utente = db.Utenti.Find(id);
            if (utente == null) return NotFound(); //User not found

            //He cheated or something
            if (utente.AdsWatched >= 4)
                return StatusCode(406);

            //Save ad watch count
            utente.AdsWatched++;

            //Add new coin to balance
            utente.SCoin++;

            //Save in his history that he gained coins
            db.CoinGuadagnate.Add(new CoinGuadagnate { IdUtente = utente.Id, Quantità = 1, Descrizione = "Pubblicità giornaliera", Attivazione = Utility.italianTime() });

            //Save changes
            db.SaveChanges();

            //Add to console log new user created
            Utility.saveEvent(Request,db, utente.Nome + " " + utente.Cognome + " (" + utente.Id + ")" + " ha guadagnato una sCoin tramite Ad");

            return new OkResult();
        }

    }
}