using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using SalveminiApi.Argo.Models;
using SalveminiApi.Models;
using SalveminiApi.Utility;

namespace SalveminiApi.Controllers
{

    [RoutePrefix("api/scoin")]
    public class sCoinsController : ApiController
    {
        DatabaseString db = new DatabaseString();

        [Route("redeem")]
        [HttpPost]
        public string CheckCode(UserPosition position)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Check if coupon exists 
                var coupon = db.Coupon.Find(position.Codice);
                if (coupon == null)
                    return "Il codice inserito non è valido";

                //Prendi parametri utente da chiamata
                var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
                var utente = db.Utenti.Find(userId);

                //Check if code is already redeemed
                var conflict = db.CouponAttivi.SingleOrDefault(x => x.idUtente == utente.id && x.idCoupon == position.Codice);
                if (conflict != null)
                    return "Hai già riscattato questo codice";

                //Check if code is valid
                if (!coupon.Attivo)
                    return "Il codice per " + coupon.Nome + " è scaduto e non può essere usato";

                //Check if code is GeoRestricted
                if (coupon.xAttivazione == null || coupon.yAttivazione == null || coupon.Raggio == null) //Not georestricted
                {
                    db.CouponAttivi.Add(new CouponAttivi { idCoupon = coupon.Codice, idUtente = utente.id }); //Save that user used that code
                    utente.sCoin += coupon.Valore; //Add sCoin to that user
                    db.SaveChanges();
                    Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha riscattato " + coupon.Valore + " sCoin dall'evento " + coupon.Nome + " (" + coupon.Codice + ")");
                    return "Il codice è stato riscattato, " + coupon.Valore.ToString() + " sCoin sono state aggiunte al tuo profilo";
                }

                //Georestricted, check position
                var offset = GpsUtils.IsInCircle(new Point { latitude = coupon.yAttivazione.Value, longitude = coupon.xAttivazione.Value }, new Point { latitude = position.yPosition, longitude = position.xPosition }, coupon.Raggio.Value);
                if (offset == 0)
                {
                    //Is in circle
                    //Save that user used that code so that cannot be used twice
                    db.CouponAttivi.Add(new CouponAttivi { idCoupon = coupon.Codice, idUtente = utente.id }); 
                    //Save in his history that he gained coins
                    db.CoinGuadagnate.Add(new CoinGuadagnate { idUtente = utente.id, Quantità = coupon.Valore, Descrizione = "Evento " + coupon.Nome, Attivazione = Helpers.Utility.italianTime() });
                    utente.sCoin += coupon.Valore; //Add sCoin to that user
                    db.SaveChanges();
                    Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha riscattato " + coupon.Valore + " sCoin dall'evento " + coupon.Nome + " (" + coupon.Codice + ")");
                    return "Il codice è stato riscattato, " + coupon.Valore.ToString() + " sCoin sono state aggiunte al tuo profilo";
                }
                else
                {
                    //Is not in circle
                    return "Sei lontano " + offset + " metri da " + db.Utenti.Find(coupon.idCreatore).Nome + ", avvicinati per poter attivare il codice";
                }
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore riscatta codice", ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

        }

        [Route("addevent")]
        [HttpPost]
        public int addEvent(Coupon coupon)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 2);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                //Create code
                void codeloop()
                {
                    var codice = Convert.ToInt32(Helpers.Utility.CreateToken(6));
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
                coupon.Creazione = Helpers.Utility.italianTime();
                //Active coupon
                coupon.Attivo = true;
                db.Coupon.Add(coupon);
                db.SaveChanges();
                //Save event
                Helpers.Utility.saveEvent("Nuovo evento creato: " + coupon.Nome);
                return coupon.Codice;
            }
            catch (Exception ex)
            {
                //Save crash in db
                Helpers.Utility.saveCrash("Errore crea evento", ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

        }


        [Route("events")]
        [HttpGet]
        public List<Coupon> EventsList()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 2);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            return db.Coupon.OrderByDescending(x => x.Creazione).ToList();
        }

        [Route("event/{id}")]
        [HttpGet]
        public HttpResponseMessage ToggleEventActive(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 2);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Find evento
            var evento = db.Coupon.Find(id);
            if (evento == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            //Toggle active
            if (evento.Attivo) evento.Attivo = false;
            else evento.Attivo = true;
            db.SaveChanges();
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }


        [Route("redeemed")]
        [HttpGet]
        public List<Coupon> Redeemed()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var returnList = new List<Coupon>();
            var attivi = db.CouponAttivi.Where(x => x.idUtente == userId).ToList();
            foreach (var attivo in attivi) { var coupon = db.Coupon.Find(attivo.idCoupon); returnList.Add(coupon); };
            return returnList;
        }

        [Route("usercoins")]
        [HttpGet]
        public int UserCoins()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Find user
            var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var utente = db.Utenti.Find(userId);

            //User not found
            if (utente == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            //Success
            return utente.sCoin;
        }


        [Route("transactions")]
        [HttpGet]
        public List<Transaction> Transactions()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Find user
            var userId = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var utente = db.Utenti.Find(userId);

            //User not found
            if (utente == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            var returnList = new List<Transaction>();
            //Add entrate
            var entrate = db.CoinGuadagnate.Where(x => x.idUtente == userId).ToList();
            foreach (var entrata in entrate) { returnList.Add(new Transaction { Amount = entrata.Quantità, Description = entrata.Descrizione, Date = entrata.Attivazione }); }
            //Add uscite with negative amount
            var uscite = db.CoinSpese.Where(x => x.idUtente == userId).ToList();
            foreach (var uscita in uscite) { returnList.Add(new Transaction { Amount = -uscita.Quantità, Description = uscita.Descrizione, Date = uscita.Attivazione }); }

            //Success
            return returnList.OrderByDescending(x => x.Date).ToList();
        }
    }

    public class Transaction
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}