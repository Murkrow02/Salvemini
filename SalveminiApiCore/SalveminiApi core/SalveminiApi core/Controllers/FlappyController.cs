using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SalveminiApi_core.Controllers
{
    [Route("api/flappy")]
    [ApiController]
    public class FlappyController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public FlappyController(Salvemini_DBContext context) { db = context; }

        [Route("getSkins")]
        [HttpGet]
        public IActionResult SkinsList()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Utility.getUserId(Request);

            try
            {
                //Create custom list for the user
                var returnSkins = new List<FlappySkinReturn>();
                var allSkins = db.FlappySkin.ToList();
                foreach (var skin in allSkins)
                {
                    //Comprata?
                    bool comprata = db.CoinSpese.FirstOrDefault(x => x.IdOggetto == skin.Id && x.Tipo == "FlappySkin") != null;
                    if (skin.Costo == 0)
                        comprata = true;

                    //Create image list separated by commma
                    var immagini = skin.Immagini.Split(',').ToList();

                    //Add to return list
                    returnSkins.Add(new FlappySkinReturn { Comprata = comprata, id = skin.Id, Costo = skin.Costo, Nome = skin.Nome, Immagini = immagini });
                }
                return Ok(returnSkins);
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }

        [Route("getUpgrade")]
        [HttpGet]
        public IActionResult UpgradeList()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Utility.getUserId(Request);

            try
            {
                //Create custom list for the user
                var upgradeList = new List<FlappyMoneteReturn>();
                var allUpgrade = db.FlappyMonete.ToList();
                foreach (var upgrade in allUpgrade)
                {
                    //Comprata?
                    bool comprata = db.CoinSpese.FirstOrDefault(x => x.IdOggetto == upgrade.Id) != null;

                    //Add to return list
                    upgradeList.Add(new FlappyMoneteReturn { Comprata = comprata, id = upgrade.Id, Costo = upgrade.Costo, Descrizione = upgrade.Descrizione, Valore = upgrade.ValoreMonete });
                }

                //Get the highest that he has not bought
                var maxUpgrade = upgradeList.Where(x => x.Comprata == false).OrderBy(y => y.Costo).FirstOrDefault();

                return Ok(maxUpgrade);
            }
            catch (Exception ex)
            {
                throw new ArgumentException();

            }
        }

        [Route("buySkin/{id}")]
        [HttpGet]
        public IActionResult BuySkin(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var IdUtente = Utility.getUserId(Request);
            var utente = db.Utenti.Find(IdUtente);

            //Prendi la skin desiderata
            var skin = db.FlappySkin.Find(id);

            try
            {
                //Not found
                if (skin == null)
                    return NotFound("La skin selezionata non è stata trovata");

                //Check if enough coins
                if (utente.SCoin < skin.Costo)
                    return StatusCode(402,"Non hai abbastanza sCoin per comprare questa skin");

                //Check if already bought
                var conflict = db.CoinSpese.FirstOrDefault(x => x.Tipo == "FlappySkin" && x.IdOggetto == skin.Id) != null;
                if (conflict)
                    return Conflict("Hai già comprato questa skin");

                //Buy skin
                if(skin.Costo > 0)
                {
                    utente.SCoin -= skin.Costo;
                    db.CoinSpese.Add(new CoinSpese { Attivazione = Utility.italianTime(), IdOggetto = skin.Id, IdUtente = IdUtente, Quantità = skin.Costo, Tipo = "FlappySkin", Descrizione = "Skin di FlappyMimmo " + "\"" + skin.Nome + "\"" });
                    db.SaveChanges();
                }
               
                return Ok("La skin è stata comprata con successo");
            }
            catch (Exception ex)
            {
                throw new ArgumentException();

            }
        }


        [Route("buyUpgrade/{id}")]
        [HttpGet]
        public IActionResult BuyUpgrade(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var IdUtente = Utility.getUserId(Request);
            var utente = db.Utenti.Find(IdUtente);

            //Prendi l'upgrade desiderato
            var upgrade = db.FlappyMonete.Find(id);

            try
            {
                //Not found
                if (upgrade == null)
                    return NotFound("L' oggetto selezionato non è stata trovato");

                //Check if enough coins
                if (utente.SCoin < upgrade.Costo)
                    return StatusCode(402,"Non hai abbastanza sCoin per comprare quest'oggetto");

                //Check if already bought
                var conflict = db.CoinSpese.FirstOrDefault(x => x.Tipo == "FlappyUpgrade" && x.IdOggetto == upgrade.Id) != null;
                if (conflict)
                    return Ok("Hai già comprato quest'oggetto");

                //Buy skin
                utente.SCoin -= upgrade.Costo;
                db.CoinSpese.Add(new CoinSpese { Attivazione = Utility.italianTime(), IdOggetto = upgrade.Id, IdUtente = IdUtente, Quantità = upgrade.Costo, Tipo = "FlappyUpgrade", Descrizione = "Potenziamento per Flappy Mimmo (Monete x" + upgrade.ValoreMonete });
                db.SaveChanges();

                return Ok("L'oggetto è stato comprato con successo");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
        }

        [Route("postScore")]
        [HttpPost]
        public IActionResult PostScore([FromBody]int score)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var IdUtente = Utility.getUserId(Request);

            try
            {
                //Find if has already a score
                var conflict = db.FlappyClassifica.Find(IdUtente);
                if (conflict != null)
                {
                    if (score > conflict.Punteggio)
                    {
                        conflict.Punteggio = score;
                        db.SaveChanges();
                        return Ok("Hai appena fatto un nuovo record personale!");
                    }
                    else
                    {
                        return Ok("Punteggio caricato");
                    }
                }
                else
                {
                    db.FlappyClassifica.Add(new FlappyClassifica { IdUtente = IdUtente, Punteggio = score }); //Add to database
                    db.SaveChanges();
                    return Ok("Punteggio caricato");
                }

            }
            catch (Exception ex)
            {
                throw new ArgumentException();

            }
        }

        [Route("getScores")]
        [HttpGet]
        public IActionResult Classifica()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var IdUtente = Utility.getUserId(Request);

            try
            {
                var returnModel = new List<UtentiClassifica>();

                //Take first 10 users
                var utenti =  db.FlappyClassifica.OrderByDescending(x => x.Punteggio).Take(10).ToList();

                //Return dictionary of User-Points
                foreach(var utente in utenti)
                {
                    var utente_ = db.Utenti.Find(utente.IdUtente);
                    returnModel.Add(new UtentiClassifica { id = utente_.Id, Image = utente_.Immagine, NomeCognome = utente_.Nome + " " + utente_.Cognome, Punteggio = utente.Punteggio});
                }
                return Ok(returnModel);
            }
            catch (Exception ex)
            {
                throw new ArgumentException();

            }
        }

        public class UtentiClassifica
        {
            public string NomeCognome { get; set; }
            public string Image { get; set; }
            public int Punteggio { get; set; }
            public int id { get; set; }
        }
    }
}
