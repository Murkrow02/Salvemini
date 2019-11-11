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
using SalveminiApp.RestApi.Models;

namespace SalveminiApp.RestApi
{
    public class RestServiceAvvisi : IRestServiceAvvisi
    {
        HttpClient client;
        public List<Models.Avvisi> Avvisi = new List<Models.Avvisi>();

        public RestServiceAvvisi()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));

        }

        public async Task<List<Models.Avvisi>> GetAvvisi()
        {
            Avvisi = new List<Models.Avvisi>();
            var uri = Costants.Uri("avvisi/all");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Avvisi = JsonConvert.DeserializeObject<List<Models.Avvisi>>(content);

                    //Save Cache
                    Barrel.Current.Add("Avvisi", Avvisi, TimeSpan.FromDays(10));
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Error getting avvisi", ex.Message);
                return null;
            }

            return Avvisi;
        }

        public async Task<string[]> PostAvviso(Avvisi avviso)
        {
            var uri = Costants.Uri("avvisi");

            try
            {
                var json = JsonConvert.SerializeObject(avviso);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    //todo istruttore con troppe schede
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", "L'avviso è stato creato" };
                    case HttpStatusCode.Unauthorized:
                        return new string[] { "Errore", "Non hai l'autorizzazione per creare un avviso" };
                    case HttpStatusCode.InternalServerError:
                        return new string[] { "Errore", "Si è verificato un errore nella creazione dell'avviso, riprova più tardi o contattaci se il problema persiste" };
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

        public async Task<string> DeleteAvviso(int id)
        {
            Avvisi = new List<Models.Avvisi>();
            var uri = Costants.Uri("avvisi/delete/" + id);

            try
            {
                var response = await client.DeleteAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return "Avviso eliminato con successo";
                    case HttpStatusCode.NotFound:
                        return "L'avviso non è stato trovato";
                    case HttpStatusCode.InternalServerError:
                        return "Si è verificato un errore durante la richiesta";
                    default:
                        return "Si è verificato un errore sconosciuto";
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Error deleting avviso", ex.Message);
                return "Si è verificato un errore nella connessione al server";
            }

        }
    }

    public interface IRestServiceAvvisi
    {
        Task<string[]> PostAvviso(Avvisi avviso);
        Task<string> DeleteAvviso(int id);
        Task<List<Models.Avvisi>> GetAvvisi();

    }

    public class AvvisiManager
    {
        IRestServiceAvvisi restServiceAvvisi;

        public AvvisiManager(IRestServiceAvvisi serviceAvvisi)
        {
            restServiceAvvisi = serviceAvvisi;
        }

        public Task<List<Models.Avvisi>> GetAvvisi()
        {
            return restServiceAvvisi.GetAvvisi();
        }

        public Task<string[]> PostAvviso(Avvisi avviso)
        {
            return restServiceAvvisi.PostAvviso(avviso);
        }

        public Task<string> DeleteAvviso(int id)
        {
            return restServiceAvvisi.DeleteAvviso(id);
        }
    }
}
