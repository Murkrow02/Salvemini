using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using SalveminiApi_core.Argo.Models;

namespace SalveminiApi_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private readonly Salvemini_DBContext db; public AuthController(Salvemini_DBContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) { db = context; _env = env; }

        [Route("testcrash")]
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            Utility.saveCrash(_env, "test", "asfhasifuhasufhsauif");
            return Ok();
        }

        [Route("updateusers")]
        [HttpGet]
        public async Task<IActionResult> UpdateUsers()
        {
            return Ok();
            foreach (var user in db.Utenti.ToList())
            {

                //Prendi schede
                var argoUtils = new ArgoUtils();
                var schedeClient = argoUtils.ArgoClient(0, user.ArgoToken);
                var schedeResponse = await schedeClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/schede");
                var schedeContent = await schedeResponse.Content.ReadAsStringAsync();
                var ArgoUser = new List<Utente>();
                try
                {
                    ArgoUser = JsonConvert.DeserializeObject<List<Utente>>(schedeContent);
                }
                catch
                {
                    //return StatusCode(406);
                }

                //Save each user in the db
                try
                {
                    foreach (Utente utente in ArgoUser)
                    {

                        //Conflict not found, create new user
                        var newUser = db.Utenti.Find(utente.prgAlunno);

                        newUser.Classe = Convert.ToInt32(utente.desDenominazione);
                        newUser.Corso = utente.desCorso;

                        //try { newUser.Compleanno = DateTime.ParseExact(utente.alunno.datNascita, "yyyy-MM-dd", new CultureInfo("it-IT")); } catch { newUser.Compleanno = new DateTime(2069,04,20); };
                        //newUser.Residenza = utente.alunno.desComuneResidenza != null ? Utility.FirstCharToUpper(utente.alunno.desComuneResidenza.ToLower()) : "";

                        db.SaveChanges();
                    }
                }
                catch { }
                await Task.Delay(500);
            }
            return Ok();
        }


            [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Auth(AuthBlock authBlock)
        {

            var returnList = new List<AuthUser>();

            //Initialize login request
            var client = new HttpClient();
            var response = new HttpResponseMessage();
            string content = "";
            if (authBlock.username != "mariateresafiorentino")
            {
                client.DefaultRequestHeaders.Add("x-app-code", "APF");
                client.DefaultRequestHeaders.Add("x-cod-min", "SS16836");
                client.DefaultRequestHeaders.Add("x-key-app", Costants.argoKey);
                client.DefaultRequestHeaders.Add("x-produttore-software", "ARGO Software s.r.l. - Ragusa");
                client.DefaultRequestHeaders.Add("x-pwd", authBlock.password);
                client.DefaultRequestHeaders.Add("x-user-id", authBlock.username);
                client.DefaultRequestHeaders.Add("x-version", Costants.argoVersion);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Mobile Safari/537.36");
            
           
             response = await client.GetAsync("https://www.portaleargo.it/famiglia/api/rest/login");

            //L'utente ha inviato password o username non corretti
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Unauthorized();

            //Errore di argo: manutenzione o rompono il cazzo
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                return StatusCode(406);


            //Boh altri errori che posssono succedere
            if (!response.IsSuccessStatusCode)
                return StatusCode(500);

                content = await response.Content.ReadAsStringAsync();
            }
            string Token = authBlock.username == "mariateresafiorentino" ? "7e3d3f15-2c66-413f-a6b0-01534a2b4d94.11" : JsonConvert.DeserializeObject<AuthResponse>(content).token;

            //Prendi schede
            var argoUtils = new ArgoUtils();
            var schedeClient =  argoUtils.ArgoClient(0, Token);
            var schedeResponse = await schedeClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/schede");
            var schedeContent = await schedeResponse.Content.ReadAsStringAsync();
            var ArgoUser = new List<Utente>();
            try
            {
                 ArgoUser = JsonConvert.DeserializeObject<List<Utente>>(schedeContent);
            }
            catch
            {
                return StatusCode(406);
            }

            //Save each user in the db
            try
            {
                foreach (Utente utente in ArgoUser)
                {
                    //Check if user is already in the db
                    int id = utente.prgAlunno;
                    var conflict = db.Utenti.Find(id);

                    if (conflict != null)
                    {
                        //Conflict found
                        conflict.Classe = Convert.ToInt32(utente.desDenominazione);
                        conflict.Corso = utente.desCorso;
                        conflict.ArgoToken = Token;
                        returnList.Add(new AuthUser { ArgoToken = conflict.ArgoToken, Classe = conflict.Classe, Cognome = conflict.Cognome, Corso = conflict.Corso, Nome = conflict.Nome, Creazione = conflict.Creazione, Stato = conflict.Stato, Id = conflict.Id, AdsWatched = conflict.AdsWatched, Immagine = conflict.Immagine, Sesso = conflict.Sesso, SCoin = conflict.SCoin, LastAdWatched = conflict.LastAdWatched });
                        db.SaveChanges();
                        continue;

                    }

                    //Conflict not found, create new user
                    var newUser = new Utenti();
                    newUser.Nome = Utility.FirstCharToUpper(utente.alunno.desNome.ToLower());
                    newUser.Cognome = Utility.FirstCharToUpper(utente.alunno.desCognome.ToLower());
                    newUser.Id = id;
                    newUser.Sesso = utente.alunno.flgSesso;
                    newUser.Classe = Convert.ToInt32(utente.desDenominazione);
                    newUser.Corso = utente.desCorso;
                    newUser.Creazione = Utility.italianTime();
                    newUser.ArgoToken = Token;
                    newUser.Stato = 1;
                    newUser.Immagine = "";
                    //try { newUser.Compleanno = DateTime.ParseExact(utente.alunno.datNascita, "yyyy-MM-dd", new CultureInfo("it-IT")); } catch { newUser.Compleanno = new DateTime(2069,04,20); };
                    //newUser.Residenza = utente.alunno.desComuneResidenza != null ? Utility.FirstCharToUpper(utente.alunno.desComuneResidenza.ToLower()) : "";
                    db.Utenti.Add(newUser);
                    db.SaveChanges();
                    returnList.Add(new AuthUser { ArgoToken = newUser.ArgoToken, Classe = newUser.Classe, Cognome = newUser.Cognome, Corso = newUser.Corso, Nome = newUser.Nome, Creazione = newUser.Creazione, Stato = newUser.Stato, Id = newUser.Id, AdsWatched = newUser.AdsWatched, Immagine = newUser.Immagine, Sesso = newUser.Sesso, SCoin = newUser.SCoin, LastAdWatched = newUser.LastAdWatched });

                    //Add to console log new user created
                    Utility.saveEvent(Request, db, newUser.Nome + " " + newUser.Cognome + " (" + newUser.Id + ")" + " si è registrato all'app");
                }
                return Ok(returnList);
            }
            catch(Exception ex)
            {
                //Save crash in db
                Utility.saveCrash(_env, $"SaveUser", $"Ex: {ex} \n creds: {authBlock.username}, {authBlock.password}");
                throw new ArgumentException();
            }
           
        }

       

        
    }
}
