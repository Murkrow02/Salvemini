//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Globalization;
//using Microsoft.AspNetCore.Mvc;
//using SalveminiApi_core.Models;
//using SalveminiApi_core;

//namespace SalveminiApi_core.Controllers
//{

//    [Route("api/utility")]
//    [ApiController]
//    public class UtilityController : ControllerBase
//    {
//        private readonly Salvemini_DBContext db; public UtilityController(Salvemini_DBContext context) { db = context; }

//        //Returns all the subjects existing in ARGO
//        [Route("materielist")]
//        [HttpGet]
//        public async IActionResult getMaterie()
//        {
//            //Check Auth
//            bool authorized = AuthHelper.Authorize(Request, db);
//            if (!authorized)
//                return Unauthorized();

//            var newMaterie = new List<string>();
//            var currentMaterie = db.Materie.ToList();
//            var utenti = db.Utenti.ToList();
//            //Scan through each user
//            foreach (var utente in utenti)
//            {
//                //Download compiti
//                var argoUtils = new ArgoUtils();
//                var argoClient = argoUtils.ArgoClient(utente.Id, utente.ArgoToken);
//                var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/compiti");
//                if (!argoResponse.IsSuccessStatusCode) //Argo fail 
//                    continue;
//                var argoContent = await argoResponse.Content.ReadAsStringAsync();

//                //Compiti
//                var compiti = JsonConvert.DeserializeObject<compitiList>(argoContent).dati;

//                foreach (var compito in compiti)
//                {
//                    //Check if materia already exists
//                    var conflict = currentMaterie.SingleOrDefault(x => x.desMateria == compito.desMateria);
//                    if (conflict == null && !newMaterie.Contains(compito.desMateria))  //New materia found

//                    {
//                        newMaterie.Add(compito.desMateria);
//                        db.Materie.Add(new Materie { desMateria = compito.desMateria });
//                        db.SaveChanges();
//                    }
//                }

//            }
//            return newMaterie;
//        }


//        [Route("appinfo")]
//        [HttpGet]
//        public IActionResult getAppInfo()
//        {

//            //Check Auth
//            bool authorized = AuthHelper.Authorize(Request, db);
//            if (!authorized)
//                return Unauthorized();

//            return Ok(db.AppInfo.Find(0));
//        }

//        //Save new appversion when update has been published
//        [Route("newappinfo")]
//        [HttpPost]
//        public IActionResult postAppInfo(AppInfo newInfo)
//        {
//            //Check Auth
//            bool authorized = AuthHelper.Authorize(Request, db);
//            if (!authorized)
//                return Unauthorized();

//            db.AppInfo.Find(0).AppVersion = newInfo.AppVersion;
//            db.AppInfo.Find(0).OrariVersion = newInfo.OrariVersion;
//            db.SaveChanges();
//            return Ok();
//        }

//        //Returns all the subjects existing in ARGO
//        [Route("updateinfo")]
//        [HttpGet]
//        public async Task a()
//        {

//            var utenti = db.Utenti.ToList();

//            //Scan through each user
//            foreach (var utente in utenti)
//            {
//                try
//                {
//                    //Download compiti
//                    var argoUtils = new ArgoUtils();
//                    var argoClient = argoUtils.ArgoClient(utente.Id, utente.ArgoToken);
//                    var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/schede");
//                    var schedeContent = await argoResponse.Content.ReadAsStringAsync();
//                    var ArgoUser = new List<Utente>();
//                    try
//                    {
//                        ArgoUser = JsonConvert.DeserializeObject<List<Utente>>(schedeContent);
//                    }
//                    catch
//                    {
//                        continue;
//                    }
//                }
//                catch
//                {
//                    continue;
//                }
//            }
//            db.SaveChanges();
//        }
//    }
//}
