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
    public class RestServiceIndex : IRestServiceIndex
    {
        HttpClient client;
        public Models.Index Index { get; private set; }
        public Models.IndexArgo IndexArgo { get; private set; }
        public List<Models.Giornalino> Giornalini { get; private set; }

        public RestServiceIndex()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<Models.Index> GetIndex()
        {
            Index = new Models.Index();
            var uri = Costants.Uri("index");
            try
            {
                //Get from url
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Index = JsonConvert.DeserializeObject<Models.Index>(content);

                    //Save Cache
                    Barrel.Current.Add("Index", Index, TimeSpan.FromDays(10));
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                   Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        Costants.Logout();
                    });
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore GET index", ex.Message);
            }
            return Index;
        }

        public async Task<List<Models.Giornalino>> GetGiornalini()
        {
            Index = new Models.Index();
            var uri = Costants.Uri("giornalini");
            try
            {
                //Get from url
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Giornalini = JsonConvert.DeserializeObject<List<Models.Giornalino>>(content);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore GET index", ex.Message);
            }
            return Giornalini;
        }

        public async Task<Models.IndexArgo> GetIndexArgo()
        {
            IndexArgo = new Models.IndexArgo();
            var uri = Costants.Uri("indexargo");
            try
            {
                //Get from url
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    IndexArgo = JsonConvert.DeserializeObject<Models.IndexArgo>(content);

                    //Save Cache
                    Barrel.Current.Add("indexargo" + DateTime.Today.ToString("yyyy-MM-dd"), IndexArgo, TimeSpan.FromDays(10));
                }
                //Disconnect if unauthorized
                else if(response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //Perform logout
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        Costants.Logout();
                    });
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore GET indexargo", ex.Message);
            }
            return IndexArgo;
        }

        public async Task<bool> PostLiveLink(string link)
        {
            var uri = Costants.Uri("live");

            try
            {
                var json = JsonConvert.SerializeObject(link);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return true;
                    default:
                        return false;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Crea offerta", ex.Message);
                return false;
            }
        }
    }


    public interface IRestServiceIndex
    {
        Task<Models.Index> GetIndex();
        Task<Models.IndexArgo> GetIndexArgo();
        Task<List<Models.Giornalino>> GetGiornalini();
        Task<bool> PostLiveLink(string link);

    }

    public class ItemManagerIndex
    {
        IRestServiceIndex restServiceIndex;


        public ItemManagerIndex(IRestServiceIndex serviceIndex)
        {
            restServiceIndex = serviceIndex;
        }

        public Task<Models.Index> GetIndex()
        {
            return restServiceIndex.GetIndex();
        }

       public Task<List<Models.Giornalino>> GetGiornalini()
        {
            return restServiceIndex.GetGiornalini();
        }


        public Task<Models.IndexArgo> GetIndexArgo()
        {
            return restServiceIndex.GetIndexArgo();
        }

        public Task<bool> PostLiveLink(string link)
        {
            return restServiceIndex.PostLiveLink(link);
        }

    }
}

