using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalveminiApi_core.Argo.Models;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SalveminiApi_core.Controllers
{
    [Route("api/ads")]
    [ApiController]
    public class AgendaController : ControllerBase
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private readonly Salvemini_DBContext db; public AgendaController(Salvemini_DBContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) { db = context; _env = env; }

        [Route("compiti/{giorno}")]
        [HttpGet]
        public async Task<IActionResult> Compiti(int giorno)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi parametri utente da chiamata
            var id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Request.Headers["x-auth-token"].First();

            //Get utente
            var utente = db.Utenti.Find(id);
            var classeCorso = utente.Classe + utente.Corso;

            var returnCompiti = new List<Compiti>();

            try
            {
                //Get user orario
                var path = _env.WebRootPath + "/Orari/Classi/" + classeCorso + ".txt";
                var orarioString = System.IO.File.ReadAllText(path);
                var orario = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                var orarioDomani = orario.Where(x => x.Giorno == giorno).ToList();

                //Get compiti
                var argoUtils = new ArgoUtils();
                var argoClient = argoUtils.ArgoClient(id, token);
                var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/compiti");
                if (!argoResponse.IsSuccessStatusCode)
                    return Forbid();
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
                        foreach (var materia in materieSimili) { desMaterie.Add(materia.DesMateria); };
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
                return StatusCode(500);
            }

            return Ok(returnCompiti.Distinct().ToList());
        }
    }
}
