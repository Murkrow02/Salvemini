using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using MonkeyCache.SQLite;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Text;

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

        public async Task<string> PostCode(Models.PostCode model)
        {
            var uri = Costants.Uri("scoin/redeem");

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Crea offerta", ex.Message);
                return "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste";
            }
        }

        public async Task<string[]> PostEvento(Models.Evento model)
        {
            var uri = Costants.Uri("scoin/addevento");

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return new string[] { "Successo", "L'evento è stato creato con il codice MAMMT" };
                }
                else
                {
                    return new string[] { "Errore", "Si è verificato un errore" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Crea offerta", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore" };
            }
        }
    }

    public interface IRestServiceCoins
    {
        Task<string> PostCode(Models.PostCode model);
    }

    public class ItemManagerCoins
    {
        IRestServiceCoins restServiceCoins;


        public ItemManagerCoins(IRestServiceCoins serviceCoins)
        {
            restServiceCoins = serviceCoins;
        }

        public Task<string> PostCode(Models.PostCode model)
        {
            return restServiceCoins.PostCode(model);
        }
    }
}
