using System;
using System.Collections.Generic;
using SalveminiApi.Argo.Models;
using SalveminiApi.Utility;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using SalveminiApi.Models;
using System.Linq;
using SalveminiApi.Helpers;
namespace UlysseApi.Controllers
{
    [RoutePrefix("api")]
    public class IstruttoriController : ApiController
    {
        DatabaseString db = new DatabaseString();
       

        [Route("login")]
        [HttpPost]
        public async Task<List<Utenti>> Auth(AuthBlock authBlock)
        {
            var returnList = new List<Utenti>();

            //Initialize login request
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-app-code", "APF");
            client.DefaultRequestHeaders.Add("x-cod-min", "SS16836");
            client.DefaultRequestHeaders.Add("x-key-app", SalveminiApi.Costants.argoKey);
            client.DefaultRequestHeaders.Add("x-produttore-software", "ARGO Software s.r.l. - Ragusa");
            client.DefaultRequestHeaders.Add("x-pwd", authBlock.password);
            client.DefaultRequestHeaders.Add("x-user-id", authBlock.username);
            client.DefaultRequestHeaders.Add("x-version", SalveminiApi.Costants.argoVersion);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Mobile Safari/537.36");
            var response = await client.GetAsync("https://www.portaleargo.it/famiglia/api/rest/login");

            //L'utente ha inviato password o username non corretti
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Errore di argo: manutenzione o rompono il cazzo
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotAcceptable);

            //Boh altri errori che posssono succedere
            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
          
            var content = await response.Content.ReadAsStringAsync();
            var Token = JsonConvert.DeserializeObject<AuthResponse>(content).token;

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
                throw new HttpResponseException(System.Net.HttpStatusCode.NotAcceptable);
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
                        returnList.Add(conflict);
                        db.SaveChanges();
                        continue;

                    }

                    //Conflict not found, create new user
                    var newUser = new Utenti();
                    newUser.Nome = Utility.FirstCharToUpper(utente.alunno.desNome.ToLower());
                    newUser.Cognome = Utility.FirstCharToUpper(utente.alunno.desCognome.ToLower());
                    newUser.id = id;
                    newUser.Sesso = utente.alunno.flgSesso;
                    newUser.Classe = Convert.ToInt32(utente.desDenominazione);
                    newUser.Corso = utente.desCorso;
                    newUser.Creazione = Utility.italianTime();
                    newUser.ArgoToken = Token;
                    newUser.Stato = 0;
                    db.Utenti.Add(newUser);
                    db.SaveChanges();
                    returnList.Add(newUser);

                    //Add to console log new user created
                    Utility.saveEvent(newUser.Nome + " " + newUser.Cognome + " (" + newUser.id + ")" + " si è registrato all'app");
                }
                return returnList;
            }
            catch(Exception ex)
            {
                //Save crash in db
                Utility.saveCrash("Errore salvataggio utente argo", ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
           
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
