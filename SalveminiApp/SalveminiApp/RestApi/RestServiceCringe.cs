using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MonkeyCache.SQLite;
using Newtonsoft.Json;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;

namespace SalveminiApp.RestApi
{
    public class RestServiceCringe : IRestServiceCringe
    {
        HttpClient client;
        List<DomandeReturn> domande = new List<DomandeReturn>();

        public RestServiceCringe()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<List<DomandeReturn>> GetFeed(int id)
        {
            domande = new List<DomandeReturn>();
            var uri = Costants.Uri("icringe/feed/") + id.ToString();

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    domande = JsonConvert.DeserializeObject<List<DomandeReturn>>(content);

                    //Save Cache
                    if (id == -1)
                        Barrel.Current.Add("cringefeed", domande, TimeSpan.FromDays(20));
                }
                else
                {
                    //Return cache if error
                    return CacheHelper.GetCache<List<DomandeReturn>>("cringefeed");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore feed iCringe", ex.Message);
                return null;
            }
            return domande;
        }

        public async Task<string[]> PostVoto(string domanda)
        {
            var uri = Costants.Uri("icringe/postdomanda");

            try
            {
                var json = JsonConvert.SerializeObject(domanda);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                var messaggio = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", messaggio };
                    default:
                        return new string[] { "Errore", messaggio };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore post domanda", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }
    }

    public interface IRestServiceCringe
    {
        Task<List<DomandeReturn>> GetFeed(int id);
    }

    public class CringeManager
    {
        IRestServiceCringe restServiceCringe;

        public CringeManager(IRestServiceCringe serviceCringe)
        {
            restServiceCringe = serviceCringe;
        }

        public Task<List<DomandeReturn>> GetFeed(int id)
        {
            return restServiceCringe.GetFeed(id);
        }
    }
}
