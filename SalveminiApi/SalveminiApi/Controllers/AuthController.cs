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
            client.DefaultRequestHeaders.Add("x-key-app", "ax6542sdru3217t4eesd9");
            client.DefaultRequestHeaders.Add("x-produttore-software", "ARGO Software s.r.l. - Ragusa");
            client.DefaultRequestHeaders.Add("x-pwd", authBlock.password);
            client.DefaultRequestHeaders.Add("x-user-id", authBlock.username);
            client.DefaultRequestHeaders.Add("x-version", "2.1.0");
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
            var ArgoUser = JsonConvert.DeserializeObject<List<Utente>>(schedeContent);

            //Save each user in the db
            foreach (Utente utente in ArgoUser)
            {
                //Check if user is already in the db
                int id = utente.prgAlunno;
                var conflict = db.Utenti.Find(id);
                if (conflict != null)
                {
                    //Conflict found
                    returnList.Add(conflict);
                    continue;

                }

                //Conflict not found, create new user
                var newUser = new Utenti();
                newUser.Nome = FirstCharToUpper(utente.alunno.desNome.ToLower());
                newUser.Cognome = FirstCharToUpper(utente.alunno.desCognome.ToLower());
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
            }
            return returnList;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public static string FirstCharToUpper(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.  
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.  
            // ... Uppercase the lowercase letters following spaces.  
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
    }
}
