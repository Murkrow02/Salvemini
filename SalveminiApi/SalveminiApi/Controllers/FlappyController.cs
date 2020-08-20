﻿using SalveminiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/flappy")]
    public class FlappyController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("getSkins")]
        [HttpGet]
        public List<FlappySkinReturn> SkinsList()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

            try
            {
                //Create custom list for the user
                var returnSkins = new List<FlappySkinReturn>();
                var allSkins = db.FlappySkin.ToList();
                foreach (var skin in allSkins)
                {
                    //Comprata?
                    bool comprata = db.CoinSpese.FirstOrDefault(x => x.idOggetto == skin.id && x.Tipo == "FlappySkin") != null;
                    if (skin.Costo == 0)
                        comprata = true;

                    //Create image list separated by commma
                    var immagini = skin.Immagini.Split(',').ToList();

                    //Add to return list
                    returnSkins.Add(new FlappySkinReturn { Comprata = comprata, id = skin.id, Costo = skin.Costo, Nome = skin.Nome, Immagini = immagini });
                }
                return returnSkins;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Errore GET lista skin flappy", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("getUpgrade")]
        [HttpGet]
        public FlappyMoneteReturn UpgradeList()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

            try
            {
                //Create custom list for the user
                var upgradeList = new List<FlappyMoneteReturn>();
                var allUpgrade = db.FlappyMonete.ToList();
                foreach (var upgrade in allUpgrade)
                {
                    //Comprata?
                    bool comprata = db.CoinSpese.FirstOrDefault(x => x.idOggetto == upgrade.id) != null;

                    //Add to return list
                    upgradeList.Add(new FlappyMoneteReturn { Comprata = comprata, id = upgrade.id, Costo = upgrade.Costo, Descrizione = upgrade.Descrizione, Valore = upgrade.ValoreMonete });
                }

                //Get the highest that he has not bought
                var maxUpgrade = upgradeList.Where(x => x.Comprata == false).OrderBy(y => y.Costo).FirstOrDefault();

                return maxUpgrade;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Errore GET lista upgrade flappy", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("buySkin/{id}")]
        [HttpGet]
        public HttpResponseMessage BuySkin(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var idUtente = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var utente = db.Utenti.Find(idUtente);

            //Prendi la skin desiderata
            var skin = db.FlappySkin.Find(id);

            try
            {
                //Not found
                if (skin == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "La skin selezionata non è stata trovata");

                //Check if enough coins
                if (utente.sCoin < skin.Costo)
                    return Request.CreateResponse(HttpStatusCode.PaymentRequired, "Non hai abbastanza sCoin per comprare questa skin");

                //Check if already bought
                var conflict = db.CoinSpese.FirstOrDefault(x => x.Tipo == "FlappySkin" && x.idOggetto == skin.id) != null;
                if (conflict)
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Hai già comprato questa skin");

                //Buy skin
                if(skin.Costo > 0)
                {
                    utente.sCoin -= skin.Costo;
                    db.CoinSpese.Add(new CoinSpese { Attivazione = Helpers.Utility.italianTime(), idOggetto = skin.id, idUtente = idUtente, Quantità = skin.Costo, Tipo = "FlappySkin", Descrizione = "Skin di FlappyMimmo " + "\"" + skin.Nome + "\"" });
                    db.SaveChanges();
                }
               

                //Save event in console
                Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha comprato la skin di flappy " + skin.Nome + " (" + skin.id + ")");

                return Request.CreateResponse(HttpStatusCode.OK, "La skin è stata comprata con successo");
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Errore acquisto skin " + skin.id + " dall'utente " + utente.id, ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
        }


        [Route("buyUpgrade/{id}")]
        [HttpGet]
        public HttpResponseMessage BuyUpgrade(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var idUtente = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            var utente = db.Utenti.Find(idUtente);

            //Prendi l'upgrade desiderato
            var upgrade = db.FlappyMonete.Find(id);

            try
            {
                //Not found
                if (upgrade == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "L' oggetto selezionato non è stata trovato");

                //Check if enough coins
                if (utente.sCoin < upgrade.Costo)
                    return Request.CreateResponse(HttpStatusCode.PaymentRequired, "Non hai abbastanza sCoin per comprare quest'oggetto");

                //Check if already bought
                var conflict = db.CoinSpese.FirstOrDefault(x => x.Tipo == "FlappyUpgrade" && x.idOggetto == upgrade.id) != null;
                if (conflict)
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Hai già comprato quest'oggetto");

                //Buy skin
                utente.sCoin -= upgrade.Costo;
                db.CoinSpese.Add(new CoinSpese { Attivazione = Helpers.Utility.italianTime(), idOggetto = upgrade.id, idUtente = idUtente, Quantità = upgrade.Costo, Tipo = "FlappyUpgrade", Descrizione = "Potenziamento per Flappy Mimmo (Monete x" + upgrade.ValoreMonete });
                db.SaveChanges();

                //Save event in console
                Helpers.Utility.saveEvent(utente.Nome + " " + utente.Cognome + " (" + utente.id + ")" + " ha comprato l'upgrade per Flappy Mimmo (Monete x" + upgrade.ValoreMonete);

                return Request.CreateResponse(HttpStatusCode.OK, "L'oggetto è stato comprato con successo");
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Errore acquisto upgrade " + upgrade.id + " dall'utente " + utente.id, ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
        }

        [Route("postScore")]
        [HttpPost]
        public HttpResponseMessage PostScore([FromBody]int score)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var idUtente = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

            try
            {
                //Find if has already a score
                var conflict = db.FlappyClassifica.Find(idUtente);
                if (conflict != null)
                {
                    if (score > conflict.Punteggio)
                    {
                        conflict.Punteggio = score;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Hai appena fatto un nuovo record personale!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Punteggio caricato");
                    }
                }
                else
                {
                    db.FlappyClassifica.Add(new FlappyClassifica { idUtente = idUtente, Punteggio = score }); //Add to database
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Punteggio caricato");
                }

            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Errore caricamento highscore FlappyMimmo " + idUtente,ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Si è verificato un errore sul nostro server, ci scusiamo per il disagio");
            }
        }

        [Route("getScores")]
        [HttpGet]
        public List<UtentiClassifica> Classifica()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var idUtente = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());

            try
            {
                var returnModel = new List<UtentiClassifica>();

                //Take first 10 users
                var utenti =  db.FlappyClassifica.OrderByDescending(x => x.Punteggio).Take(10).ToList();

                //Return dictionary of User-Points
                foreach(var utente in utenti)
                {
                    var utente_ = db.Utenti.Find(utente.idUtente);
                    returnModel.Add(new UtentiClassifica { id = utente_.id, Image = utente_.Immagine, NomeCognome = utente_.Nome + " " + utente_.Cognome, Punteggio = utente.Punteggio});
                }
                return returnModel;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Errore GET lista classifica flappy", ex.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
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