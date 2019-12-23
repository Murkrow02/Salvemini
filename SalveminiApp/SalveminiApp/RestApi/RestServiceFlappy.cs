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
    public class RestServiceFlappy : IRestServiceFlappy
    {
        HttpClient client;
        public List<Models.FlappySkinReturn> Skins { get; private set; }

        public RestServiceFlappy()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<List<Models.FlappySkinReturn>> GetSkins()
        {
            Skins = new List<Models.FlappySkinReturn>();
            var uri = Costants.Uri("flappy/getskins");
            try
            {
                //Get from url
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Skins = JsonConvert.DeserializeObject<List<Models.FlappySkinReturn>>(content);

                    //Save Cache
                    Barrel.Current.Add("Skins", Skins, TimeSpan.FromDays(10));
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
            return Skins;
        }

        public async Task<string> BuySkin(int id)
        {
            var uri = Costants.Uri("flappy/buyskin/" + id);
            try
            {
                //Get from url
                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore GET indexargo", ex.Message);
                return "Si è verificato un errore";
            }
        }
    }

    public interface IRestServiceFlappy
    {
        Task<List<Models.FlappySkinReturn>> GetSkins();
        Task<string> BuySkin(int id);
    }

    public class ItemManagerFlappy
    {
        IRestServiceFlappy restServiceFlappy;

        public ItemManagerFlappy(IRestServiceFlappy serviceFlappy)
        {
            restServiceFlappy = serviceFlappy;
        }

        public Task<List<Models.FlappySkinReturn>> GetSkins()
        {
            return restServiceFlappy.GetSkins();
        }

        public Task<string> BuySkin(int id)
        {
            return restServiceFlappy.BuySkin(id);
        }
    }
}
