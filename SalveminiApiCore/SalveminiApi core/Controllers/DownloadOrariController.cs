using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Controllers
{

    [Route("api/orari")]
    [ApiController]
    public class DownloadOrariController : ControllerBase
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private readonly Salvemini_DBContext db; public DownloadOrariController(Salvemini_DBContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) { db = context; _env = env; }

        [Route("treni")]
        [HttpGet]
        public IActionResult getOrari()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Read string from txt file
            try
            {
                var path = _env.WebRootPath + "/Orari/OrariTreni.txt";
                var a = System.IO.File.ReadAllText(path);
                return Ok(a);
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [Route("classe/{classe}")]
        [HttpPost]
        public IActionResult uploadOrariClasse(string classe, List<Models.Lezione> orario)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db,1);
            if (!authorized)
                return Unauthorized();

            //Serialize json
            try
            {
                string orarioString = JsonConvert.SerializeObject(orario);
                var path =_env.WebRootPath + "/Orari/Classi/" + classe + ".txt";

                //Save file
               System.IO.File.WriteAllText(path, orarioString);

                //Add record to log
                var id = Utility.getUserId(Request);

                return Created("","");
            }
            catch (Exception ex)
            {
                //Save crash
                throw new ArgumentException();

            }
        }

        [Route("classe/{classe}")]
        [HttpGet]
        public IActionResult getOrarioClasse(string classe)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Deserialize json
            try
            {
                var path = _env.WebRootPath +  "/Orari/Classi/" + classe + ".txt";
                var orarioString = System.IO.File.ReadAllText(path);
                var orario = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                return Ok(orario);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Route("materie")]
        [HttpGet]
        public IActionResult getMaterie()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get materie
            try
            {
                return Ok(db.Materie.ToList());
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Route("siri/{classe}")]
        [HttpGet]
        public IActionResult getOrarioSiri(string classe)
        {
            //Deserialize json
            try
            {
                var path = _env.WebRootPath +  "/Orari/Classi/" + classe + ".txt";
                var orarioString = System.IO.File.ReadAllText(path);
                var orario = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                var intOggi = (int)Utility.italianTime().DayOfWeek;
                var orarioDomani = orario.Where(x => x.Giorno == intOggi + 1 && x.Materia != null && x.Materia != "").OrderBy(x => x.Ora).ToList();

                ////remove this
                //return new string[] { "Domani", "Pensar en dios, " +
                //    "Etudiar todo el dia, nutrirme sanamente, " +
                //    "EL PENE" };

                //Detect sunday
                if (intOggi == 6)
                    return Ok(new string[] { "Domani non", "niente, è domenica!" });

                //Handle errors
                if (orarioDomani == null || orarioDomani.Count < 1)
                    return NotFound();

                //Detect freeday
                if (orarioDomani[0].Materia == "Libero")
                    return Ok(new string[] { "Domani non", "niente, è il tuo giorno libero!" });


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


                return Ok(new string[] { "Domani", returnString });
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [Route("siri/{stazione}/{direzione}/{single}")]
        [HttpGet]
        public IActionResult getNextTrain(int stazione, bool direzione, bool single)
        {
            //Deserialize json
            try
            {
                var path = _env.WebRootPath + "/Orari/OrariTreni.txt";
                var treniString = System.IO.File.ReadAllText(path);
                var Trains = JsonConvert.DeserializeObject<List<Models.Treno>>(treniString);

                //Filter by station and direction
                Trains = Trains.Where(x => x.Direzione == direzione && x.Stazione == stazione).OrderBy(x => x.LeaveTime).ToList();

                //Remove Campania Express
                Trains = Trains.Where(x => x.Importanza != "EXP").ToList();

                //Filter by Variations
                if (Utility.italianTime().DayOfWeek == DayOfWeek.Sunday)
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
                if (Trains[Trains.Count - 1].LeaveTime > new DateTime(1, 1, 1, Utility.italianTime().Hour, Utility.italianTime().Minute, 0))
                {
                    //Remove previous trains
                    Trains = Trains.Where(x => x.LeaveTime > new DateTime(1, 1, 1, Utility.italianTime().Hour, Utility.italianTime().Minute, 0)).ToList();
                }
                else
                {
                    return Ok(single ? new List<Models.Treno> { Trains[0] } : new List<Models.Treno> { Trains[1], Trains[2], Trains[3] });
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

                return Ok(treni);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Route("siri/oggi/{classe}")]
        [HttpGet]
        public IActionResult getOrarioOggi(string classe)
        {
            //Deserialize json
            try
            {
                var path = _env.WebRootPath + "/Orari/Classi/" + classe + ".txt";
                var orarioString = System.IO.File.ReadAllText(path);
                var orario_ = JsonConvert.DeserializeObject<List<Models.Lezione>>(orarioString);
                var intOggi = (int)Utility.italianTime().DayOfWeek;
                var orario = orario_.Where(x => x.Giorno == intOggi && x.Materia != null && x.Materia != "").OrderBy(x => x.Ora).ToList();


                //Handle errors
                if (orario == null || orario.Count < 1)
                    return NotFound();

                //Detect sunday
                if (intOggi == 0)
                    return Ok(new List<Models.Lezione>() { new Models.Lezione(db) { Materia = "Domenica" } });

                //Detect freeday
                if (orario[0].Materia == "Libero")
                    return Ok(new List<Models.Lezione>() { new Models.Lezione(db) { Materia = "Giorno Libero" } });

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




                return Ok(orario);
            }
            catch
            {
                throw new ArgumentException();
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