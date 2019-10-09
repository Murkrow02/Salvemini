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
                    Barrel.Current.Add("Index", Avvisi, TimeSpan.FromDays(10));
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return Avvisi;
        }


    }

    public interface IRestServiceAvvisi
    {
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
    }
}
