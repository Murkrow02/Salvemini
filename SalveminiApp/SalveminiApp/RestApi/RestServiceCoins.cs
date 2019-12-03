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
            var uri = Costants.Uri("scoin/addevent");

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                //Risposta
                if (response.IsSuccessStatusCode)
                {
                    int codice = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                    return new string[] { "Successo", "L'evento è stato creato con il codice " + codice };
                }
                else
                {
                    return new string[] { "Errore", "Si è verificato un errore, contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Crea evento", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore, riprova più tardi" };
            }
        }

        public async Task<List<Models.Evento>> ListaEventi()
        {
            var uri = Costants.Uri("scoin/events");

            try
            {
                //Get response
                var response = await client.GetAsync(uri);

                //Risposta
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var eventi = JsonConvert.DeserializeObject<List<Models.Evento>>(content);
                    return eventi;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Lista eventi", ex.Message);
                return null;
            }
        }

        public async Task<string[]> ToggleEvento(int id)
        {
            var uri = Costants.Uri("scoin/event/" + id);

            try
            {
                //Get response
                var response = await client.GetAsync(uri);

                //Risposta
                if (response.IsSuccessStatusCode)
                {
                    return new string[] { "Successo", "L'evento è stato attivato/disattivato" };
                }
                else
                {
                    return new string[] { "Errore", "Si è verificato un errore, contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                return new string[] { "Errore", "Si è verificato un errore, riprova più tardi" };
            }
        }
    }

    public interface IRestServiceCoins
    {
        Task<string> PostCode(Models.PostCode model);
        Task<string[]> PostEvento(Models.Evento model);
        Task<List<Models.Evento>> ListaEventi();
        Task<string[]> ToggleEvento(int id);
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

        public Task<string[]> PostEvento(Models.Evento model)
        {
            return restServiceCoins.PostEvento(model);
        }

        public Task<List<Models.Evento>> ListaEventi()
        {
            return restServiceCoins.ListaEventi();
        }

        public Task<string[]> ToggleEvento(int id)
        {
            return restServiceCoins.ToggleEvento(id);
        }
    }
}
