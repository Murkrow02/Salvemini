using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace SalveminiApi.Controllers
{

    [RoutePrefix("api/orari")]
    public class DownloadOrariController : ApiController
    {
        [Route("treni")]
        [HttpGet]
        public string getOrari()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Read string from txt file
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Orari/OrariTreni.txt");
                var a = File.ReadAllText(path);
                return a;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error get orario trasporti", ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

        }

        [Route("classe/{classe}")]
        [HttpPost]
        public HttpResponseMessage uploadOrariClasse(string classe, List<Models.Lezione> orario)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request, 1);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Serialize json
            try
            {
                string orarioString = JsonConvert.SerializeObject(orario);
                var path = HttpContext.Current.Server.MapPath("~/Orari/Classi/" + classe + ".txt");

                //Save file
                File.WriteAllText(path, orarioString);

                //Add record to log
                var id = Request.Headers.GetValues("x-user-id").First();
                Helpers.Utility.saveEvent("L'utente " + id + " ha pubblicato un orario per la classe " + classe);

                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                //Save crash
                Helpers.Utility.saveCrash("Error saving orario classe " + classe, ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Route("classe/{classe}")]
        [HttpGet]
        public List<Models.Lezione> getOrarioClasse(string classe)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Deserialize json
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Orari/Classi/" + classe + ".txt");
                var orarioString = File.ReadAllText(path);
                var orario = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                return orario;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error getting orario classe " + classe, ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("siri/{classe}")]
        [HttpGet]
        public string[] getOrarioSiri(string classe)
        {
            //Deserialize json
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Orari/Classi/" + classe + ".txt");
                var orarioString = File.ReadAllText(path);
                var orario = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                var intOggi = 2;// (int)Helpers.Utility.italianTime().DayOfWeek ;
                var orarioDomani = orario.Where(x => x.Giorno == intOggi + 1 && x.Materia != null && x.Materia != "").ToList();


                //remove this
                return new string[] { "Domani", "Pensar en dios, Etudiar todo el dia, nutrirme sanamente, EL PENE" };



                //Handle errors
                if (orarioDomani == null || orarioDomani.Count < 1)
                    throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

                //Detect sunday
                if(intOggi == 0)
                    return new string[] { "Domani non", "niente, è domenica!" };

                //Detect freeday
                if (orarioDomani[0].Materia == "Libero")
                    return new string[] { "Domani non", "niente, è il tuo giorno libero!" };


                string orariGiorni = "";
                string lastMateria = "";
                int oreLastMateria = 1;
                
                for (int i = 0; i < orarioDomani.Count(); i++)
                {
                    string materia = orarioDomani[i].Materia;

                    //if (materia == lastMateria) {
                    //    oreLastMateria++;
                    //    lastMateria = materia;
                    //    continue;
                    //}
                       

                    orariGiorni +=  materia + separator();
                    lastMateria = materia;
                    oreLastMateria = 1;

                    string separator()
                    {
                        if (i == orarioDomani.Count - 2)
                            return " e ";
                        else if (i == orarioDomani.Count - 1)
                            return "";
                        else
                            return ", ";
                }

                    //string howManyHours()
                    //{
                    //    if (oreLastMateria == 1)
                    //        return "";
                    //    else
                    //        return oreLastMateria.ToString() + " ore di ";
                    //}
                }

               


                return new string[] { "Domani", orariGiorni};
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error getting orario classe " + classe, ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}