using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using System.Collections.Generic;

namespace SalveminiApp.RestApi
{
    public class RestServiceCoins : IRestServiceCoins
    {
        HttpClient client;
        
        public RestServiceCoins()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }
    }

    public interface IRestServiceCoins
    {

    }

    public class ItemManagerCoins
    {
        IRestServiceCoins restServiceCoins;


        public ItemManagerCoins(IRestServiceCoins serviceCoins)
        {
            restServiceCoins = serviceCoins;
        }
    }
}
