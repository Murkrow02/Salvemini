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
                //Get Cache if no Network Access
                if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet && Barrel.Current.Exists("Index"))
                {
                    return Barrel.Current.Get<Models.Index>("Index");
                }

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Index = JsonConvert.DeserializeObject<Models.Index>(content);

                    //Save Cache
                    Barrel.Current.Add("Index", Index, TimeSpan.FromDays(10));
                }
                else
                {
                    if (Barrel.Current.Exists("Index"))
                    {
                        return Barrel.Current.Get<Models.Index>("Index");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return Index;
        }
    }

    public interface IRestServiceIndex
    {
        Task<Models.Index> GetIndex();
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

    }
}

