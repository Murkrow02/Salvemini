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
        public List<Models.Materie> Materie { get; private set; }

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

        public async Task<Models.ResponseModel> GetMaterie()
        {
            Materie = new List<Models.Materie>();
            var data = new Models.ResponseModel();
            try
            {
                var uri = Costants.Uri("orari/materie");

                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Materie = JsonConvert.DeserializeObject<List<Models.Materie>>(content);
                        data.Data = Materie;
                        break;
                    case HttpStatusCode.InternalServerError:
                        data.Message = "Non è stato possibile scaricare le materie";
                        break;
                }

            }
            catch (Exception ex)
            {
                data.Message = "Si è verificato un errore, controlla la tua connessione e riprova";
            }

            return data;
        }

    }

    public interface IRestServiceOrari
    {
        Task<Models.ResponseModel> GetOrario(string classe);
        Task<Models.ResponseModel> GetMaterie();
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
        public Task<Models.ResponseModel> GetMaterie()
        {
            return restServiceOrari.GetMaterie();
        }
        public Task<string[]> UploadOrario(string classe, List<Models.newOrario> newOrario)
        {
            return restServiceOrari.UploadOrario(classe, newOrario);
        }

    }
}

