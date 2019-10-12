using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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
                //Get Cache if no Network Access
                if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet && Barrel.Current.Exists("cardofferte"))
                {
                    return Barrel.Current.Get<List<Models.Offerte>>("cardofferte");
                }

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Offerte = JsonConvert.DeserializeObject<List<Models.Offerte>>(content);

                    //Save Cache
                    Barrel.Current.Add("cardofferte", Offerte, TimeSpan.FromDays(10));
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                {
                    if (Barrel.Current.Exists("cardofferte"))
                    {
                        return Barrel.Current.Get<List<Models.Offerte>>("cardofferte");
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return Offerte;
        }

    }

    public interface IRestServiceCard
    {
        Task<List<Offerte>> GetOfferte();
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

    }
}
