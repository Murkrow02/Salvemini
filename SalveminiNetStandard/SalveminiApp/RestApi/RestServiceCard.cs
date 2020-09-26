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
    public class RestServiceCard: IRestServiceCard
    {
        HttpClient client;
        public List<Models.Offerte> Offerte { get; private set; }

        public RestServiceCard()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<List<Models.Offerte>> GetOfferte()
        {
            Offerte = new List<Models.Offerte>();
            var uri = Costants.Uri("card/offerte");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Offerte = JsonConvert.DeserializeObject<List<Models.Offerte>>(content);

                    //Save Cache
                    Barrel.Current.Add("cardofferte", Offerte, TimeSpan.FromDays(10));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore offerte salvemini card", ex.Message);
            }
            return Offerte;
        }

        public async Task<string[]> PostOfferta(Offerte offerta)
        {
            var uri = Costants.Uri("card/offerta");

            try
            {
                var json = JsonConvert.SerializeObject(offerta);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", "L'offerta è stata creata" };
                    case HttpStatusCode.Unauthorized:
                        return new string[] { "Errore", "Non hai l'autorizzazione per creare un'offerta" };
                    case HttpStatusCode.InternalServerError:
                        return new string[] { "Errore", "Si è verificato un errore nella creazione dell'offerta, riprova più tardi o contattaci se il problema persiste" };
                    default:
                        return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Crea offerta", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

    }

    public interface IRestServiceCard
    {
        Task<List<Offerte>> GetOfferte();
        Task<string[]> PostOfferta(Offerte offerta);
    }

    public class ItemManagerCard
    {
        IRestServiceCard restServiceCard;


        public ItemManagerCard(IRestServiceCard serviceCard)
        {
            restServiceCard = serviceCard;
        }

        public Task<List<Offerte>> GetOfferte()
        {
            return restServiceCard.GetOfferte();
        }

        public Task<string[]> PostOfferta(Offerte offerta)
        {
            return restServiceCard.PostOfferta(offerta);
        }

    }
}
