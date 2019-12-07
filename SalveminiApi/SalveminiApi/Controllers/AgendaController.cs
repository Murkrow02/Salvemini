using Newtonsoft.Json;
using SalveminiApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SalveminiApi.Argo.Models;
using SalveminiApi.Utility;
using System.Globalization;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/agenda")]
    public class AgendaController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("compiti/{giorno}")]
        [HttpGet]
        public async Task<List<Compiti>> Compiti(int giorno)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Request.Headers.GetValues("x-user-id").First());
            string token = Request.Headers.GetValues("x-auth-token").First();

            //Get utente
            var utente = db.Utenti.Find(id);
            var classeCorso = utente.Classe + utente.Corso;

            var returnCompiti = new List<Compiti>();

            try
            {
                //Get user orario
                var path = HttpContext.Current.Server.MapPath("~/Orari/Classi/" + classeCorso + ".txt");
                var orarioString = File.ReadAllText(path);
                var orario = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                var orarioDomani = orario.Where(x => x.Giorno == giorno).ToList();

                //Get compiti
                var argoUtils = new ArgoUtils();
                var argoClient = argoUtils.ArgoClient(id, token);
                var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/compiti");
                if (!argoResponse.IsSuccessStatusCode)
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                var argoContent = await argoResponse.Content.ReadAsStringAsync();
                //Order by last date
                var compiti = JsonConvert.DeserializeObject<compitiList>(argoContent).dati.OrderByDescending(x => DateTime.ParseExact(x.datGiorno, "yyyy-MM-dd", new CultureInfo("it-IT"))).ToList();

                //Get last homework foreach lesson of that day
                foreach (var lezione in orarioDomani)
                {
                    try
                    {
                        //Find materia with id in lezione
                        var Materia = db.Materie.Find(lezione.idMateria).Materia;
                        //Find similar materie (e.g. Matematica & Matematica con informatica)
                        var materieSimili = db.Materie.Where(x => x.Materia.ToLower().StartsWith(Materia.ToLower()) || Materia.ToLower().StartsWith(x.Materia.ToLower())).ToList();
                        var desMaterie = new List<string>();
                        //Create a list with subjects
                        foreach (var materia in materieSimili) { desMaterie.Add(materia.desMateria); };
                        //Get last assigned homework for that subject
                        var lastHomework = compiti.FirstOrDefault(x => desMaterie.Contains(x.desMateria));
                        //Remove nulls
                        if (lastHomework != null)
                            returnCompiti.Add(lastHomework);
                    }
                    catch
                    {
                        continue;
                    }
                   
                };
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

            return returnCompiti.Distinct().ToList(); ;
        }


    }
}
