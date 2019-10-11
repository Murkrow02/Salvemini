using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace SalveminiApp.RestApi
{
    public class RestServiceAnalytics: IRestServiceAnalytics
    {
        HttpClient client;
        public List<Models.Analytics> Analytics = new List<Models.Analytics>();

        public RestServiceAnalytics()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));

        }

        public async Task<List<Models.Analytics>> GetAnalytics()
        {
            Analytics = new List<Models.Analytics>();
            var uri = Costants.Uri("analytics/all");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Analytics = JsonConvert.DeserializeObject<List<Models.Analytics>>(content);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return Analytics;
        }
    }

    public interface IRestServiceAnalytics
    {
        Task<List<Models.Analytics>> GetAnalytics();

    }

    public class AnalytiscManager
    {
        IRestServiceAnalytics restServiceAnalytics;

        public AnalytiscManager(IRestServiceAnalytics serviceAnalytics)
        {
            restServiceAnalytics = serviceAnalytics;
        }

        public Task<List<Models.Analytics>> GetAnalytics()
        {
            return restServiceAnalytics.GetAnalytics();
        }

    }
}
