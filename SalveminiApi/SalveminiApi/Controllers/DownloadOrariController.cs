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
                var intOggi = (int)Helpers.Utility.italianTime().DayOfWeek;
                var orarioDomani = orario.Where(x => x.Giorno == intOggi + 1 && x.Materia != null && x.Materia != "").OrderBy(x => x.Ora).ToList();

                ////remove this
                //return new string[] { "Domani", "Pensar en dios, " +
                //    "Etudiar todo el dia, nutrirme sanamente, " +
                //    "EL PENE" };

                //Detect sunday
                if (intOggi == 6)
                    return new string[] { "Domani non", "niente, è domenica!" };

                //Handle errors
                if (orarioDomani == null || orarioDomani.Count < 1)
                    throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

                //Detect freeday
                if (orarioDomani[0].Materia == "Libero")
                    return new string[] { "Domani non", "niente, è il tuo giorno libero!" };


                var orariGiorni = new List<string>();

                for (int i = orarioDomani.Count() - 1; i >= 0; i--)
                {
                    string materia = orarioDomani[i].Materia;

                    if (orarioDomani.ElementAtOrDefault(i + 1) != null && materia == orarioDomani[i + 1].Materia)
                    {
                        orarioDomani.RemoveAt(i);
                        continue;
                    }

                    int numOre = 1;


                    //Detect how many hours of that subject
                    string lezione()
                    {
                        if (orarioDomani.ElementAtOrDefault(i - numOre) != null && materia == orarioDomani[i - numOre].Materia)
                        {
                            numOre++;
                            lezione();
                        }
                        return numOre > 1 ? NumberToWord[numOre] + " ore di " + materia : materia;
                    }

                    materia = lezione();

                    //Add lesson
                    orariGiorni.Add(materia);
                }


                var materie = orariGiorni;

                //Add commas and "e"
                materie.Reverse();
                var returnString = string.Join(", ", materie);
                returnString = new string(returnString.Reverse().ToArray());
                var ultima = returnString.IndexOf(',');
                returnString = returnString.Remove(ultima + 1).Replace(" ,", " e ") + returnString.Substring(ultima + 1);
                returnString = new string(returnString.Reverse().ToArray());


                return new string[] { "Domani", returnString };
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error getting orario classe " + classe, ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }


        }

        [Route("siri/{stazione}/{direzione}/{single}")]
        [HttpGet]
        public List<Models.Treno> getNextTrain(int stazione, bool direzione, bool single)
        {
            //Deserialize json
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Orari/OrariTreni.txt");
                var treniString = File.ReadAllText(path);
                var Trains = JsonConvert.DeserializeObject<List<Models.Treno>>(treniString);

                //Filter by station and direction
                Trains = Trains.Where(x => x.Direzione == direzione && x.Stazione == stazione).OrderBy(x => x.LeaveTime).ToList();

                //Remove Campania Express
                Trains = Trains.Where(x => x.Importanza != "EXP").ToList();

                //Filter by Variations
                if (Helpers.Utility.italianTime().DayOfWeek == DayOfWeek.Sunday)
                {
                    //Remove Feriali if sunday
                    Trains = Trains.Where(x => x.Variazioni != "FER").ToList();
                }
                else
                {
                    //Remove Festivi if not sunday
                    Trains = Trains.Where(x => x.Variazioni != "FES").ToList();
                }

                //Check if is last train of the day
                if (Trains[Trains.Count - 1].LeaveTime > new DateTime(1, 1, 1, Helpers.Utility.italianTime().Hour, Helpers.Utility.italianTime().Minute, 0))
                {
                    //Remove previous trains
                    Trains = Trains.Where(x => x.LeaveTime > new DateTime(1, 1, 1, Helpers.Utility.italianTime().Hour, Helpers.Utility.italianTime().Minute, 0)).ToList();
                }
                else
                {
                    return single ? new List<Models.Treno> { Trains[0] } : new List<Models.Treno> { Trains[1], Trains[2], Trains[3] };
                }

                var treni = new List<Models.Treno>();
                for (int i = 0; i < Trains.Count(); i++)
                {
                    //Skip first if we need next three
                    if (i == 0 && !single)
                        continue;

                    //Exit if we already have 4 trains
                    if (i == 4)
                        break;

                    //Add train to return list
                    if (Trains.ElementAtOrDefault(i) != null)
                    {
                        treni.Add(Trains[i]);
                    }

                    //If we need only one exit the cycle
                    if (single)
                        break;
                }

                return treni;
            }
            catch (Exception ex)
            {
                Helpers.Utility.saveCrash("Error get next train siri", ex.ToString());
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("siri/oggi/{classe}")]
        [HttpGet]
        public List<Models.Lezione> getOrarioOggi(string classe)
        {
            //Deserialize json
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Orari/Classi/" + classe + ".txt");
                var orarioString = File.ReadAllText(path);
                var orario_ = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                var intOggi = (int)Helpers.Utility.italianTime().DayOfWeek;
                var orario = orario_.Where(x => x.Giorno == intOggi && x.Materia != null && x.Materia != "").OrderBy(x => x.Ora).ToList();


                //Handle errors
                if (orario == null || orario.Count < 1)
                    throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

                //Detect sunday
                if (intOggi == 0)
                    return new List<Models.Lezione>() { new Models.Lezione { Materia = "Domenica" } };

                //Detect freeday
                if (orario[0].Materia == "Libero")
                    return new List<Models.Lezione>() { new Models.Lezione { Materia = "Giorno Libero" } };

                //Group hours by duration
                for (int i = 0; i < orario.Count; i++)
                {
                    //Skip hour if is equal to the previous
                    if (orario.ElementAtOrDefault(i - 1) != null && orario[i].Materia == orario[i - 1].Materia || string.IsNullOrEmpty(orario[i].Materia))
                    {
                        orario[i].toRemove = true;
                        continue;
                    }
                    int next = 1;

                    Models.Lezione lezione()
                    {
                        //Check if next hour is the same of this hour
                        if (orario.ElementAtOrDefault(i + next) != null && orario[i].Materia == orario[i + next].Materia)
                        {
                            //Increment hours number
                            orario[i].numOre = next + 1;
                            //Increment
                            next++;
                            lezione();
                        }


                        return orario[i];
                    }

                    orario[i] = lezione();
                }

                foreach (var lezione in orario)
                {
                    if (lezione.numOre == 0)
                    {
                        //Set Number of hours to default
                        lezione.numOre = 1;
                    }
                }

                //Remove to remove items
                orario = orario.Where(x => x.toRemove == false).ToList();




                return orario;
            }
            catch
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }



        public Dictionary<int, string> NumberToWord = new Dictionary<int, string>()
        {
            {2 , "due" },
            {3 , "tre" },
            {4 , "quattro" },
            {5 , "cinque" },
            {6 , "sei" }
        };
    }


}