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
    public class RestServiceArgo : IRestServiceArgo
    {
        HttpClient client;
        public List<Models.Assenza> Assenze { get; private set; }
        public RestServiceArgo()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        #region Assenze
        public async Task<List<Models.Assenza>> GetAssenze()
        {
            Assenze = new List<Models.Assenza>();
            var uri = Costants.Uri("argo/assenze");
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Assenze = JsonConvert.DeserializeObject<List<Models.Assenza>>(content);

                }
                Barrel.Current.Add("Assenze", Assenze, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return Assenze;
        }

        public async Task<bool> GiustificaAssenza(RestApi.Models.AssenzaModel item)
        {

            string uri = Costants.Uri("argo/giustifica");
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                return false;
            }
        }
        #endregion
    }

    public interface IRestServiceArgo
    {
        Task<List<Models.Assenza>> GetAssenze();
        Task<bool> GiustificaAssenza(RestApi.Models.AssenzaModel item);
    }

    public class ItemManagerArgo
    {
        IRestServiceArgo restServiceArgo;


        public ItemManagerArgo(IRestServiceArgo serviceArgo)
        {
            restServiceArgo = serviceArgo;
        }

        public Task<List<Models.Assenza>> GetAssenze()
        {
            return restServiceArgo.GetAssenze();
        }

        public Task<bool> GiustificaAssenza(RestApi.Models.AssenzaModel item)
        {
            return restServiceArgo.GiustificaAssenza(item);
        }
    }
}

