using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalveminiApi_core.Argo.Models;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SalveminiApi_core.Controllers
{
    [Route("api/argo")]
    [ApiController]
    public class ArgoController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public ArgoController(Salvemini_DBContext context) { db = context; }

        [Route("compiti")]
        [HttpGet]
        public async Task<IActionResult> getCompiti()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/compiti");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<compitiList>(argoContent);
            return Ok(returnModel.dati);
        }

        [Route("argomenti")]
        [HttpGet]
        public async Task<IActionResult> getArgomenti()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/argomenti");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<argomentiList>(argoContent);
            return Ok(returnModel.dati);
        }

        [Route("promemoria")]
        [HttpGet]
        public async Task<IActionResult> getPromemoria()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);



            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/promemoria");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<promemoriaList>(argoContent);
            return Ok(returnModel.dati);
        }

        [Route("assenze")]
        [HttpGet]
        public async Task<IActionResult> getAssenze()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/assenze");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<AssenzeList>(argoContent);
            var returnList = new List<Assenze>();
            returnList = returnModel.dati;
            return Ok(returnList);
        }

        [Route("newAssenze")]
        [HttpGet]
        public async Task<IActionResult> newAssenze()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/assenze");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();

            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<AssenzeList>(argoContent);

            //Trova assenza non giustificata
            var daGiustificare = returnModel.dati.Where(x => x.flgDaGiustificare == true || x.datGiustificazione == null).ToList();
            if (daGiustificare.Count > 0)
            {
                return Ok(true);
            }
            return Ok(false);

        }

        [Route("giustifica")]
        [HttpPost]
        public async Task<IActionResult> giustificaAssenza(AssenzaModel giustifica)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Posta modello
            var argoUtils = new ArgoUtils();
            var json = JsonConvert.SerializeObject(giustifica);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.PostAsync("https://www.portaleargo.it/famiglia/api/rest/giustifica", content);
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();


            return Ok();
        }

        [Route("voti")]
        [HttpGet]
        public async Task<IActionResult> getVoti()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/votigiornalieri");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();

            var votiList = new List<Voti>();

            try
            {
                var argoContent = await argoResponse.Content.ReadAsStringAsync();
                var returnModel = JsonConvert.DeserializeObject<VotiList>(argoContent);
                if (id == 2125)
                {
                    returnModel.dati.Add(new Voti { datGiorno = "2020-10-30", desMateria = "Educazione sessuale", docente = "(Prof. Gessi)", decValore = 10, codVoto = "10", codVotoPratico = "N" });
                    returnModel.dati.Add(new Voti { datGiorno = "2020-9-30", desMateria = "Educazione sessuale", docente = "(Prof. Gessi)", decValore = 2, codVoto = "2", codVotoPratico = "N" });
                    returnModel.dati.Add(new Voti { datGiorno = "2020-10-20", desMateria = "Educazione sessuale", docente = "(Prof. Gessi)", decValore = 5, codVoto = "5", codVotoPratico = "N" });
                }
                 
                votiList = returnModel.dati;
            }
            catch //ARGO offline
            {
                return Forbid();
            }


            //Raggruppa per materia
            var groupedVoti = new List<GroupedVoti>();
            var votiGrouped = votiList.GroupBy(x => x.desMateria).ToList();
            var groupedCollection = new List<GroupedVoti>();

            return Ok(votiList);
        }

        [Route("pentagono")]
        [HttpGet]
        public async Task<IActionResult> getPentagono()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/votigiornalieri");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<VotiList>(argoContent);
            var votiList = new List<Voti>();
            votiList = returnModel.dati;

            //Raggruppa per materia
            var groupedVoti = new List<GroupedVoti>();
            var votiGrouped = votiList.GroupBy(x => x.desMateria).ToList();
            var groupedCollection = new List<Pentagono>();

            for (int i = 0; i < votiGrouped.Count(); i++)
            {
                var groupedMateria = new Pentagono();
                var listaVoti = new List<Voti>();

                foreach (var voto in votiGrouped[i])
                {


                    listaVoti.Add(voto);
                    groupedMateria.Materia = voto.Materia;
                }


                //Calcola media
                int count = 0;
                double media = 0.0;
                foreach (var voto in votiGrouped[i])
                {

                    //Controlla se è una giustifica
                    if (voto.decValore == null || !voto.codVoto.Any(char.IsDigit))
                    {
                        continue;
                    }

                    //Aggiungi alla media
                    media += Convert.ToDouble(voto.decValore);
                    count++;
                }

                //Controlla se ci sono voti
                if (count > 0)
                    groupedMateria.Media = media / count;
                else
                    groupedMateria.Media = 0;


                groupedCollection.Add(groupedMateria);

            }

            //Remove results with 0 (probably got with giustifica)
            groupedCollection.RemoveAll(x => x.Media == 0);

            return Ok(groupedCollection);
        }


        [Route("scrutinio")]
        [HttpGet]
        public async Task<IActionResult> getScrutinio()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/votiscrutinio");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<List<Scrutinio>>(argoContent);

            //Ragruppa voti
            var grouped = new ScrutinioGrouped();
            var Primo = new List<Scrutinio>();
            var Secondo = new List<Scrutinio>();

            Primo = (returnModel.Where(x => x.prgPeriodo == 1).ToList());
            Secondo = (returnModel.Where(x => x.prgPeriodo == 2).ToList());

            grouped.Primo = Primo;
            grouped.Secondo = Secondo;

            return Ok(grouped);
        }

        [Route("bacheca")]
        [HttpGet]
        public async Task<IActionResult> getBacheca()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/bachecanuova");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<bachecaList>(argoContent);
            var returnList = new List<Bacheca>();
            returnList = returnModel.dati;
            return Ok(returnList);
        }

        [Route("visualizzabacheca")]
        [HttpPost]
        public async Task<IActionResult> visualizzaBacheca(VisualizzaBacheca visto)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Posta modello
            var argoUtils = new ArgoUtils();
            var json = JsonConvert.SerializeObject(visto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var argoClient = argoUtils.ArgoClient(id, token);
            //Post
            var argoResponse = await argoClient.PostAsync("https://www.portaleargo.it/famiglia/api/rest/presavisionebachecanuova", content);
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();


            return Ok();
        }

        [Route("changePwd")]
        [HttpPost]
        public async Task<IActionResult> changePwd(changeBlock block)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Posta modello
            var argoUtils = new ArgoUtils();
            var json = JsonConvert.SerializeObject(block);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var argoClient = argoUtils.ArgoClient(id, token);

            //Post
            var argoResponse = await argoClient.PostAsync("https://www.portaleargo.it/famiglia/api/rest/cambiopassword", content);
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();


            return Ok();
        }

        [Route("note")]
        [HttpGet]
        public async Task<IActionResult> getNote()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get parameters from request
            int id = Convert.ToInt32(Utility.getUserId(Request));
            string token = Utility.getToken(Request);

            //Prendi modello
            var argoUtils = new ArgoUtils();
            var argoClient = argoUtils.ArgoClient(id, token);
            var argoResponse = await argoClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/notedisciplinari");
            if (!argoResponse.IsSuccessStatusCode)
                return Forbid();
            var argoContent = await argoResponse.Content.ReadAsStringAsync();
            var returnModel = JsonConvert.DeserializeObject<noteList>(argoContent);
            var returnList = new List<Note>();
            returnList = returnModel.dati;
            return Ok();
        }
    }
}
