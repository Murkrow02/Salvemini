using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Reflection;
using System.Linq;
using MonkeyCache.SQLite;

namespace SalveminiApp.RestApi
{
    public class RestServiceOrari : IRestServiceOrari
    {
        HttpClient client;
        public List<Models.Lezione> Lezioni { get; private set; }
        public List<Models.Lezione> Orario { get; private set; }

        public RestServiceOrari()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        //Downloads orario of that class
        public async Task<Models.ResponseModel> GetOrario(string classe)
        {
            Lezioni = new List<Models.Lezione>();
            var data = new Models.ResponseModel();
            try
            {
                var uri = Costants.Uri("orari/classe/" + classe);

                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Lezioni = JsonConvert.DeserializeObject<List<Models.Lezione>>(content);
                        data.Data = Lezioni;
                        //Save Cache
                        Barrel.Current.Add("orario" + classe, Lezioni, TimeSpan.FromDays(90));
                        break;
                    case HttpStatusCode.NotFound:
                        data.Message = "L'orario della classe non è stato trovato";
                        break;
                    case HttpStatusCode.InternalServerError:
                        data.Message = "Non è stato possibile recuperare l'orario";
                        break;
                }
                
            }
            catch (Exception ex)
            {
                data.Message = "Si è verificato un errore";
            }

            return data;
        }

        //Gets only one day
        public async Task<List<Models.Lezione>> GetOrarioDay(int day, List<Models.Lezione> orario)
        {
            //Filter list per day and order by hour
            orario = orario.Where(x => x.Giorno == day).OrderBy(x => x.Ora).ToList();


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
                RestApi.Models.Lezione lezione()
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

                //Setup Height Of Hour
                lezione.OrarioFrameHeight = (App.ScreenHeight / 24) * lezione.numOre;

                //Setup Corner Radius
                lezione.OrarioFrameRadius = lezione.numOre > 1 ? 10 : 8;
            }

            //Remove to remove items
            orario = orario.Where(x => x.toRemove == false).ToList();


            Barrel.Current.Add("orarioday" + day, orario, TimeSpan.FromDays(90));

            return orario;
        }

        public async Task<string[]> UploadOrario(string classe, List<Models.newOrario> newOrario)
        {
            var uri = Costants.Uri("orari/classe/" + classe);

            try
            {
                var json = JsonConvert.SerializeObject(newOrario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Created:
                        return new string[] { "Grazie!", "Il nuovo orario è stato inviato correttamente" };
                    default:
                        return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }
    }

    public interface IRestServiceOrari
    {
        Task<Models.ResponseModel> GetOrario(string classe);
        Task<List<Models.Lezione>> GetOrarioDay(int day, List<Models.Lezione> orario);
        Task<string[]> UploadOrario(string classe, List<Models.newOrario> newOrario);

    }

    public class ItemManagerOrari
    {
        IRestServiceOrari restServiceOrari;


        public ItemManagerOrari(IRestServiceOrari serviceOrari)
        {
            restServiceOrari = serviceOrari;
        }

        public Task<Models.ResponseModel> GetOrario(string classe)
        {
            return restServiceOrari.GetOrario(classe);
        }

        public Task<List<Models.Lezione>> GetOrarioDay(int day, List<Models.Lezione> orario)
        {
            return restServiceOrari.GetOrarioDay(day, orario);
        }
        public Task<string[]> UploadOrario(string classe, List<Models.newOrario> newOrario)
        {
            return restServiceOrari.UploadOrario(classe, newOrario);
        }

    }
}

