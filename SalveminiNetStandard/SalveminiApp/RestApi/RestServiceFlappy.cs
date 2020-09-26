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
using System.Collections.ObjectModel;

namespace SalveminiApp.RestApi
{
    public class RestServiceFlappy : IRestServiceFlappy
    {
        HttpClient client;
        public ObservableCollection<Models.FlappySkinReturn> Skins { get; private set; }
        public Models.FlappyMoneteReturn Multiplier { get; private set; }
        public List<Models.UtentiClassifica> Scores { get; private set; }

        public RestServiceFlappy()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<ObservableCollection<Models.FlappySkinReturn>> GetSkins()
        {
            Skins = new ObservableCollection<Models.FlappySkinReturn>();
            var uri = Costants.Uri("flappy/getskins");
            try
            {
                //Get from url
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Skins = JsonConvert.DeserializeObject<ObservableCollection<Models.FlappySkinReturn>>(content);

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

        public async Task<List<Models.UtentiClassifica>> GetScores()
        {
            Scores = new List<Models.UtentiClassifica>();
            var uri = Costants.Uri("flappy/getscores");
            try
            {
                //Get from url
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Scores = JsonConvert.DeserializeObject<List<Models.UtentiClassifica>>(content);

                    //Save Cache
                    Barrel.Current.Add("Scores", Scores, TimeSpan.FromDays(10));
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
            return Scores;
        }

        public async Task<string> Upgrade(int idUpgrade)
        {
            var uri = Costants.Uri("flappy/buyUpgrade/" + idUpgrade);
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
                    Preferences.Set("multiplier", idUpgrade);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore GET indexargo", ex.Message);
                return "Si è verificato un errore";
            }
        }

        public async Task<Models.FlappyMoneteReturn> GetUpgrade()
        {
            var uri = Costants.Uri("flappy/getUpgrade");
            try
            {
                Multiplier = new Models.FlappyMoneteReturn();

                //Get from url
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Multiplier = JsonConvert.DeserializeObject<Models.FlappyMoneteReturn>(content);

                    //No more power ups
                    if (Multiplier == null)
                    {
                        Preferences.Set("multiplier", 10);
                    }

                    return Multiplier;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore GET indexargo", ex.Message);
                return null;
            }

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

        public async Task<string> PostScore(int score)
        {
            var uri = Costants.Uri("flappy/postscore");
            try
            {
                var json = JsonConvert.SerializeObject(score);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //Get from url
                var response = await client.PostAsync(uri, content);
                var returnValue = await response.Content.ReadAsStringAsync();
                if (returnValue != "\"Punteggio caricato\"")
                {
                    return returnValue;
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
        Task<ObservableCollection<Models.FlappySkinReturn>> GetSkins();
        Task<string> BuySkin(int id);
        Task<string> PostScore(int score);
        Task<string> Upgrade(int idUpgrade);
        Task<Models.FlappyMoneteReturn> GetUpgrade();
        Task<List<Models.UtentiClassifica>> GetScores();
    }

    public class ItemManagerFlappy
    {
        IRestServiceFlappy restServiceFlappy;

        public ItemManagerFlappy(IRestServiceFlappy serviceFlappy)
        {
            restServiceFlappy = serviceFlappy;
        }

        public Task<ObservableCollection<Models.FlappySkinReturn>> GetSkins()
        {
            return restServiceFlappy.GetSkins();
        }

        public Task<string> BuySkin(int id)
        {
            return restServiceFlappy.BuySkin(id);
        }

        public Task<string> PostScore(int score)
        {
            return restServiceFlappy.PostScore(score);
        }

        public Task<string> Upgrade(int idUpgrade)
        {
            return restServiceFlappy.Upgrade(idUpgrade);
        }

        public Task<Models.FlappyMoneteReturn> GetUpgrade()
        {
            return restServiceFlappy.GetUpgrade();
        }

        public Task<List<Models.UtentiClassifica>> GetScores()
        {
            return restServiceFlappy.GetScores();
        }
    }
}
