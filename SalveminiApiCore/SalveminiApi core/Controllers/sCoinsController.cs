using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalveminiApi_core;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Controllers
{

    [Route("api/scoin")]
    [ApiController]
    public class sCoinsController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public sCoinsController(Salvemini_DBContext context) { db = context; }

        [Route("redeem")]
        [HttpPost]
        public IActionResult CheckCode(UserPosition position)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Check if coupon exists 
                var coupon = db.Coupon.Find(position.Codice);
                if (coupon == null)
                    return Ok("Il codice inserito non è valido");

                //Prendi parametri utente da chiamata
                var userId = Utility.getUserId(Request);
                var utente = db.Utenti.Find(userId);

                //Check if code is already redeemed
                var conflict = db.CouponAttivi.SingleOrDefault(x => x.IdUtente == utente.Id && x.IdCoupon == position.Codice);
                if (conflict != null)
                    return Ok("Hai già riscattato questo codice");

                //Check if code is valid
                if (!coupon.Attivo.Value)
                    return Ok("Il codice per " + coupon.Nome + " è scaduto e non può essere usato");

                //Check if code is GeoRestricted
                if (coupon.XAttivazione == null || coupon.YAttivazione == null || coupon.Raggio == null) //Not georestricted
                {
                    db.CouponAttivi.Add(new CouponAttivi { IdCoupon = coupon.Codice, IdUtente = utente.Id }); //Save that user used that code
                    utente.SCoin += coupon.Valore; //Add sCoin to that user
                    db.SaveChanges();
                    Utility.saveEvent(Request,db, utente.Nome + " " + utente.Cognome + " (" + utente.Id + ")" + " ha riscattato " + coupon.Valore + " sCoin dall'evento " + coupon.Nome + " (" + coupon.Codice + ")");
                    return Ok("Il codice è stato riscattato, " + coupon.Valore.ToString() + " sCoin sono state aggiunte al tuo profilo");
                }

                //Georestricted, check position
                var offset = GpsUtils.IsInCircle(new Point { latitude = coupon.YAttivazione.Value, longitude = coupon.XAttivazione.Value }, new Point { latitude = position.yPosition, longitude = position.xPosition }, coupon.Raggio.Value);
                if (offset == 0)
                {
                    //Is in circle
                    //Save that user used that code so that cannot be used twice
                    db.CouponAttivi.Add(new CouponAttivi { IdCoupon = coupon.Codice, IdUtente = utente.Id }); 
                    //Save in his history that he gained coins
                    db.CoinGuadagnate.Add(new CoinGuadagnate { IdUtente = utente.Id, Quantità = coupon.Valore, Descrizione = "Evento " + coupon.Nome, Attivazione = Utility.italianTime() });
                    utente.SCoin += coupon.Valore; //Add sCoin to that user
                    db.SaveChanges();
                    Utility.saveEvent(Request, db, utente.Nome + " " + utente.Cognome + " (" + utente.Id + ")" + " ha riscattato " + coupon.Valore + " sCoin dall'evento " + coupon.Nome + " (" + coupon.Codice + ")");
                    return Ok("Il codice è stato riscattato, " + coupon.Valore.ToString() + " sCoin sono state aggiunte al tuo profilo");
                }
                else
                {
                    //Is not in circle
                    return Ok("Sei lontano " + offset + " metri da " + db.Utenti.Find(coupon.IdCreatore).Nome + ", avvicinati per poter attivare il codice");
                }
            }
            catch (Exception ex)
            {
                //Save crash in db
                throw new ArgumentException();
            }

        }

        [Route("addevent")]
        [HttpPost]
        public IActionResult addEvent(Coupon coupon)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db,2);
            if (!authorized)
                return Unauthorized();

            try
            {
                //Create code
                void codeloop()
                {
                    var codice = Convert.ToInt32(Utility.CreateToken(6));
                    var conflict = db.Coupon.Find(codice);
                    if (conflict != null) //Conflict found, repeat
                    {
                        codeloop();
                        return;
                    }
                    coupon.Codice = codice; //Set created code
                }

                codeloop();
                //Save with italian time
                coupon.Creazione = Utility.italianTime();
                //Active coupon
                coupon.Attivo = true;
                db.Coupon.Add(coupon);
                db.SaveChanges();
                //Save event
                return Ok(coupon.Codice);
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }

        }


        [Route("events")]
        [HttpGet]
        public IActionResult EventsList()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db, 2);
            if (!authorized)
                return Unauthorized();

            return Ok(db.Coupon.OrderByDescending(x => x.Creazione).ToList());
        }

        [Route("event/{id}")]
        [HttpGet]
        public IActionResult ToggleEventActive(int id)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db, 2);
            if (!authorized)
                return Unauthorized();

            //Find evento
            var evento = db.Coupon.Find(id);
            if (evento == null)
                return NotFound();

            //Toggle active
            if (evento.Attivo.Value) evento.Attivo = false;
            else evento.Attivo = true;
            db.SaveChanges();
            return Ok();
        }


        [Route("redeemed")]
        [HttpGet]
        public IActionResult Redeemed()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db, 2);
            if (!authorized)
                return Unauthorized();

            var userId = Utility.getUserId(Request);
            var returnList = new List<Coupon>();
            var attivi = db.CouponAttivi.Where(x => x.IdUtente == userId).ToList();
            foreach (var attivo in attivi) { var coupon = db.Coupon.Find(attivo.IdCoupon); returnList.Add(coupon); };
            return Ok(returnList);
        }

        [Route("usercoins")]
        [HttpGet]
        public IActionResult UserCoins()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db, 2);
            if (!authorized)
                return Unauthorized();

            //Find user
            var userId = Utility.getUserId(Request);
            var utente = db.Utenti.Find(userId);

            //User not found
            if (utente == null)
                return NotFound();
            //Success
            return Ok(utente.SCoin);
        }


        [Route("transactions")]
        [HttpGet]
        public IActionResult Transactions()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db, 2);
            if (!authorized)
                return Unauthorized();

            //Find user
            var userId = Utility.getUserId(Request);
            var utente = db.Utenti.Find(userId);

            //User not found
            if (utente == null)
                return NotFound();

            var returnList = new List<Transaction>();
            //Add entrate
            var entrate = db.CoinGuadagnate.Where(x => x.IdUtente == userId).ToList();
            foreach (var entrata in entrate) { returnList.Add(new Transaction { Amount = entrata.Quantità, Description = entrata.Descrizione, Date = entrata.Attivazione }); }
            //Add uscite with negative amount
            var uscite = db.CoinSpese.Where(x => x.IdUtente == userId).ToList();
            foreach (var uscita in uscite) { returnList.Add(new Transaction { Amount = -uscita.Quantità, Description = uscita.Descrizione, Date = uscita.Attivazione }); }

            //Success
            return Ok(returnList.OrderByDescending(x => x.Date).ToList());
        }
    }

    public class Transaction
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}