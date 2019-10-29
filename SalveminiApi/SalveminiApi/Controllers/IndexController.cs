﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using SalveminiApi.Argo.Models;
using SalveminiApi.Models;
using SalveminiApi.Utility;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api")]
    public class IndexController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("index")]
        [HttpGet]
        public async Task<IndexModel> GetIndex()
        {
            var returnModel = new IndexModel();

            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Aggiungi accesso ad analytics
            Helpers.Utility.addToAnalytics("Accessi");

            //Authorized?
            var utente = db.Utenti.Find(id);
            if (utente.Stato < 0 || !authorized)
            {
                returnModel.Authorized = false;
                return returnModel;
            }
            returnModel.Authorized = true;

            //Versioni
            returnModel.AppVersion = 1.0M;
            returnModel.OrariTreniVersion = 1;
            returnModel.OrariBusVersion = 1;
            returnModel.OrariScuolaVersion = 1;
            returnModel.OrariAliVersion = 1;


            //Prendi ultimo avviso
            try
            {
                var avvisi = db.Avvisi.OrderBy(x => x.Creazione).ToList();
                returnModel.ultimoAvviso = avvisi.Last();
            }
            catch
            {
                //Prendi valore alto così non dice che ce ne sono altri
                returnModel.ultimoAvviso = new Avvisi { Creazione = new DateTime(2090, 10, 10) };
            }

            //Prendi ultimo sondaggio
            try
            {
                var attivo = db.Sondaggi.Where(x => x.Attivo).ToList();
                if (attivo.Count < 1)
                    returnModel.ultimoSondaggio = null;
                else
                {
                    //Controlla se ha votato
                    var voti = attivo[0].VotiSondaggi.ToList();
                    var suo = voti.Where(x => x.Utente == id).ToList();
                    if (suo.Count > 0) //Ha votato
                        returnModel.ultimoSondaggio = null;
                    else
                        returnModel.ultimoSondaggio = attivo[0]; //Non ha votato, manda il sondaggio

                }
            }
            catch
            {
                //Prendi valore alto così non dice che ce ne sono altri
                returnModel.ultimoSondaggio = null;
            }

           


            //ADS
            var adsList = new List<Ads>();
            var interstitial = db.Ads.Where(x => x.Tipo == 1).ToList();
            var banner = db.Ads.Where(x => x.Tipo == 0).ToList();

            //Get banner
            if (banner.Count > 0)
            {
                adsList.Add(banner[0]);
                db.Ads.Find(banner[0].id).Impressions ++;
            }

            //Get random interstitial
            if (interstitial.Count > 0)
            {
                Random r = new Random();
                int rInt = r.Next(0, interstitial.Count);
                adsList.Add(interstitial[rInt]);
                db.Ads.Find(interstitial[rInt]).Impressions++;
            }

            //Save ad downloads
            try
            {
                db.SaveChanges();
            }
            catch
            {
                //Fa niente
            }

            returnModel.Ads = adsList;
            return returnModel;
        }

        [Route("oggi/{giorno}")]
        [HttpGet]
        public async Task<WholeModel> argoOggi(string giorno)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if(!authorized)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //ARGO
            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);

            //Oggi or data selected
            string data = "";
            if (giorno == "oggi") //Today
                data = Helpers.Utility.italianTime().ToString("yyyy-MM-dd").Split(' ')[0].Trim();
            else
                data = giorno; //Custom date

            var returnModel = new WholeModel();
            //Create new uri
            var uri = "https://www.portaleargo.it/famiglia/api/rest/oggi?datGiorno=" + data;

            try
            {
                //Get all today things
                var response = await argoClient.GetAsync(uri);

                //Token changed
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new HttpResponseException(HttpStatusCode.Forbidden);

                var oggi = new Oggi();
                var content = await response.Content.ReadAsStringAsync();
                oggi = JsonConvert.DeserializeObject<Oggi>(content);

                var notizie = oggi.dati;

                //Initialize lists
                var compiti = new List<Compiti>();
                var argomenti = new List<Argomenti>();
                var assenze = new List<Assenze>();
                var bacheca = new List<Bacheca>();
                var promemoria = new List<Promemoria>();
                var voti = new List<Voti>();


                foreach (var notizia in notizie)
                {
                    switch (notizia.tipo)
                    {
                        case "BAC":
                            //BACHECA
                            bacheca.Add(new Bacheca { adesione = notizia.dati.adesione,  allegati = notizia.dati.allegati,  desMessaggio = notizia.dati.desMessaggio,  desOggetto = notizia.dati.desOggetto, desUrl = notizia.dati.desUrl, presaVisione = notizia.dati.presaVisione, prgMessaggio = notizia.dati.prgMessaggio,  richiediAd = notizia.dati.richiediAd, richiediPv = notizia.dati.richiediPv });
                            break;
                        case "COM":
                            break;
                        case "ARG":
                            break;
                        case "VOT":
                            break;
                        case "PRO":
                            break;
                        case "ASS":
                            break;
                    }

                    //COMPITI
                    //ARGOMENTI
                    //ASSENZE
                    //PROMEMORIA
                    //VOTI
                }

                returnModel.bacheca = bacheca;

                if (response.IsSuccessStatusCode)
                {

                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            return returnModel;
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
