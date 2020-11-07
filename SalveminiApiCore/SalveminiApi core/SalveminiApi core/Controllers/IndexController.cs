using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalveminiApi_core.Argo.Models;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public IndexController(Salvemini_DBContext context) { db = context; }

        [Route("index")]
        [HttpGet]
        public async Task<IActionResult> GetIndex()
        {
            var returnModel = new IndexModel();

            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Utility.getUserId(Request);
            string token = Utility.getToken(Request);

            //Authorized?
            var utente = db.Utenti.Find(id);

            //Bannato
            if (utente.Stato < 0)
            {
                returnModel.Authorized = -2;
                return Ok(returnModel);
            }

            //Token cambiato
            if (!authorized || utente == null)
            {
                returnModel.Authorized = -1;
                return Ok(returnModel);
            }

            //Versioni
            var appInfo = db.AppInfo.Find(0);if(appInfo != null)
            {
                returnModel.AppVersion = appInfo.AppVersion;
                returnModel.OrarioTrasportiVersion = appInfo.OrariVersion;
            }
           

            //sCoin
            returnModel.sCoin = utente.SCoin;

            //Classe
            returnModel.Classe = utente.Classe.ToString();
            returnModel.Corso = utente.Corso;

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
                var attivo = db.Sondaggi.FirstOrDefault(x => x.Attivo.Value);

                //Ci sono sondaggi attivi?
                if (attivo == null)
                    returnModel.ultimoSondaggio = null; //No

                else //Si
                {
                    //Find his voto
                    var suo = db.VotiSondaggi.FirstOrDefault(x => x.Utente == id && x.IdSondaggio == attivo.Id);

                    if (suo != null) //Ha votato
                        returnModel.VotedSondaggio = true;

                    returnModel.ultimoSondaggio = attivo;

                }
            }
            catch(Exception ex)
            {
                returnModel.ultimoSondaggio = null;
            }




            //ADS
            var adsList = new List<Ads>();
            var interstitial = db.Ads.Where(x => x.Tipo == 1).ToList();
            var banner = db.Ads.Where(x => x.Tipo == 0).ToList();

            //Giornalino
            var Giornalini = db.Giornalino.OrderByDescending(x => x.Data).ToList();
            returnModel.Giornalino = Giornalini.FirstOrDefault();

            //Get banner
            if (banner.Count > 0)
            {
                adsList.Add(banner[0]);
                db.Ads.Find(banner[0].Id).Impressions++;
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
            return Ok(returnModel);
        }


        [Route("giornalini")]
        [HttpGet]
        public IActionResult GetGiornalini()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            return Ok(db.Giornalino.OrderByDescending(x => x.Data).Take(30).ToList());
        }

        [Route("oggi/{giorno}")]
        [HttpGet]
        public async Task<IActionResult> argoOggi(string giorno)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Utility.getUserId(Request);
            string token = Utility.getToken(Request);

            //ARGO
            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);

            //Create new model to return
            var returnModel = new WholeModel();

            //Create new uri
            var uri = "https://www.portaleargo.it/famiglia/api/rest/oggi?datGiorno=" + giorno;

            try
            {
                //Get all today things
                var response = await argoClient.GetAsync(uri);

                //Token changed
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Forbid();

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
                            bacheca.Add(new Bacheca { adesione = notizia.dati.adesione, allegati = notizia.dati.allegati, desMessaggio = notizia.dati.desMessaggio, desOggetto = notizia.dati.desOggetto, desUrl = notizia.dati.desUrl, presaVisione = notizia.dati.presaVisione, prgMessaggio = notizia.dati.prgMessaggio, richiediAd = notizia.dati.richiediAd, richiediPv = notizia.dati.richiediPv });
                            break;
                        case "COM":
                            //COMPITI
                            compiti.Add(new Compiti { datGiorno = notizia.dati.datGiorno, desCompiti = notizia.dati.desCompiti, desMateria = notizia.dati.desMateria, docente = notizia.dati.docente });
                            break;
                        case "ARG":
                            //ARGOMENTI
                            argomenti.Add(new Argomenti { datGiorno = notizia.dati.datGiorno, desMateria = notizia.dati.desMateria, desArgomento = notizia.dati.desArgomento, docente = notizia.dati.docente });
                            break;
                        case "VOT":
                            //VOTI
                            voti.Add(new Voti { datGiorno = notizia.dati.datGiorno, prgMateria = notizia.dati.prgMateria, docente = notizia.dati.docente, codVoto = notizia.dati.codVoto, codVotoPratico = notizia.dati.codVotoPratico, decValore = notizia.dati.decValore, desCommento = notizia.dati.desCommento, desMateria = notizia.dati.desMateria, desProva = notizia.dati.desProva });
                            break;
                        case "PRO":
                            //PROMEMORIA
                            promemoria.Add(new Promemoria { datGiorno = notizia.dati.datGiorno, desAnnotazioni = notizia.dati.desAnnotazioni, desMittente = notizia.dati.desMittente });
                            break;
                        case "ASS":
                            //ASSENZE
                            assenze.Add(new Assenze { datAssenza = notizia.dati.datAssenza, codEvento = notizia.dati.codEvento, binUid = notizia.dati.binUid, datGiustificazione = notizia.dati.datGiustificazione, desAssenza = notizia.dati.desAssenza, flgDaGiustificare = notizia.dati.flgDaGiustificare, giustificataDa = notizia.dati.giustificataDa, numOra = notizia.dati.numOra, oraAssenza = notizia.dati.oraAssenza, registrataDa = notizia.dati.registrataDa });
                            break;
                    }

                }

                //Add to return model
                returnModel.bacheca = bacheca;
                returnModel.argomenti = argomenti;
                returnModel.assenze = assenze;
                returnModel.compiti = compiti;
                returnModel.promemoria = promemoria;
                returnModel.voti = voti;
            }
            catch (Exception ex)
            {
                return Forbid();
            }

            return Ok(returnModel);
        }

        [Route("indexargo")]
        [HttpGet]
        public async Task<IActionResult> argoOggi()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Utility.getUserId(Request);
            string token = Utility.getToken(Request);

            //ARGO
            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);

            //Create new model to return
            var returnModel = new WholeModel();

            //Create new uri
            var uri = "https://www.portaleargo.it/famiglia/api/rest/oggi?datGiorno=" + Utility.italianTime().ToString("yyyy-MM-dd");

            try
            {
                //Get all today things
                var response = await argoClient.GetAsync(uri);

                //Token changed
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Forbid();

                var oggi = new Oggi();
                var content = await response.Content.ReadAsStringAsync();
                oggi = JsonConvert.DeserializeObject<Oggi>(content);
                var notizie = oggi.dati;

                //Create new model
                var argoIndex = new Models.IndexArgo();
                argoIndex.NotizieCount = notizie.Count();

                //Create list to random extract later
                var listaRandom = new List<string[]>();

                foreach (var notizia in notizie)
                {
                    switch (notizia.tipo)
                    {
                        case "BAC":
                            //BACHECA
                            listaRandom.Add(new string[] { "Bacheca: ",notizia.dati.desMessaggio });
                            break;
                        case "COM":
                            //COMPITI
                            listaRandom.Add(new string[] { "Compiti da " + notizia.dati.docente.Replace("(","").Replace(")","") + ": ", notizia.dati.Materia });
                            break;
                        case "ARG":
                            //ARGOMENTI
                            listaRandom.Add(new string[] { notizia.dati.docente.Replace("(", "").Replace(")", "") + ": ", notizia.dati.desArgomento });
                            break;
                        case "VOT":
                            //VOTI
                            listaRandom.Add(new string[] { "", "Hai preso " + notizia.dati.codVoto + " di " + notizia.dati.Materia });
                            break;
                        case "PRO":
                            //PROMEMORIA
                            listaRandom.Add(new string[] { "Promemoria: ", notizia.dati.desAnnotazioni });
                            break;
                        case "ASS":
                            //ASSENZE
                            string cosa = "";
                            switch (notizia.dati.codEvento)
                            {
                                case "A":
                                    cosa = "Oggi sei stato assente";
                                    break;
                                case "U":
                                    cosa = "Oggi sei uscito in anticipo";
                                    break;
                                case "I":
                                    cosa = "Oggi sei entrato in ritardo";
                                    break;
                            }

                            listaRandom.Add(new string[] { "", cosa});
                            break;
                    }

                }

                //Add to return model
                //Prendi prima assenze o voti
                var priorità = listaRandom.Where(x => x[0] == "").ToList();
                if (priorità.Count > 0)
                {
                    argoIndex.TipoNotizia = priorità[0][0];
                    argoIndex.UltimaNotizia = priorità[0][1];

                }
                else
                {
                    if(listaRandom.Count > 0)
                    {
                        argoIndex.TipoNotizia = listaRandom[0][0];
                        argoIndex.UltimaNotizia = listaRandom[0][1];
                    }
                }
                return Ok(argoIndex);
            }
            catch (Exception ex)
            {
                return Forbid();
            }

        }

      
    }
}
