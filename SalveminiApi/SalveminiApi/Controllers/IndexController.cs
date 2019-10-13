using System;
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
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Aggiungi ad analytics
            Helpers.Utility.addToAnalytics("Accessi");

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

            //ARGO
            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);

            //Codice copiato dalla salveminiapp vecchia, non si capisce un cazzo (ritorna cosa è successo in classe)
            //var Oggi = new Oggi();
            //List<WholeModel> Oggis = new List<WholeModel>();
            //var uri = "https://www.portaleargo.it/famiglia/api/rest/oggi?datGiorno=" + Helpers.Utility.italianTime().ToString("yyyy-MM-dd").Split(' ')[0].Trim();

            //try
            //{
            //    returnModel.ArgoAuth = true;
            //    var response = await argoClient.GetAsync(uri);
            //    if (response.StatusCode == HttpStatusCode.Unauthorized)
            //         returnModel.ArgoAuth = false;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var content = await response.Content.ReadAsStringAsync();
            //        Oggi = JsonConvert.DeserializeObject<Oggi>(content);
            //        var Dates = Oggi.dati;
            //        for (int i = Dates.Count - 1; i >= 0; i--)
            //        {
            //            var Model = new WholeModel();
            //            Model.binUid = Dates[i].dati.binUid;
            //            Model.codEvento = Dates[i].dati.codEvento;
            //            Model.codMin = Dates[i].codMin;
            //            Model.codVoto = Dates[i].dati.codVoto;
            //            Model.datGiorno = Dates[i].dati.datGiorno;
            //            Model.datGiustificazione = Dates[i].dati.datGiustificazione;
            //            Model.decValore = Dates[i].dati.decValore;
            //            Model.desAnnotazioni = Dates[i].dati.desAnnotazioni;
            //            Model.desArgomento = Dates[i].dati.desArgomento;
            //            Model.desAssenza = Dates[i].dati.desAssenza;
            //            Model.desCompiti = Dates[i].dati.desCompiti;
            //            Model.desMateria = Dates[i].dati.desMateria;
            //            Model.desMittente = Dates[i].dati.desMittente;
            //            Model.docente = Dates[i].dati.docente;
            //            Model.flgDaGiustificare = Dates[i].dati.flgDaGiustificare;
            //            Model.giorno = Dates[i].giorno;
            //            Model.giustificataDa = Dates[i].dati.giustificataDa;
            //            Model.numAnno = Dates[i].numAnno;
            //            Model.ordine = Dates[i].ordine;
            //            Model.prgAlunno = Dates[i].prgAlunno;
            //            Model.prgClasse = Dates[i].dati.prgClasse;
            //            Model.prgMateria = Dates[i].dati.prgMateria;
            //            Model.prgScheda = Dates[i].prgScheda;
            //            Model.prgScuola = Dates[i].prgScuola;
            //            Model.registrataDa = Dates[i].dati.registrataDa;
            //            Model.tipo = Dates[i].tipo;
            //            Model.datAssenza = Dates[i].dati.datAssenza;
            //            Oggis.Add(Model);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //Argo fail, don't mind this
            //    Oggis.Add(new WholeModel());
            //}

            //returnModel.Oggi = Oggis;


            //ADS
            var adsList = new List<Ads>();
            var interstitial = db.Ads.Where(x => x.Tipo == 1).ToList();
            var banner = db.Ads.Where(x => x.Tipo == 0).ToList();

            //Get banner
            if (banner.Count > 0)
                adsList.Add(banner[0]);

            //Get random interstitial
            if(interstitial.Count > 0)
            {
                Random r = new Random();
                int rInt = r.Next(0, interstitial.Count);
                adsList.Add(interstitial[rInt]);
            }
            returnModel.Ads = adsList;


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
