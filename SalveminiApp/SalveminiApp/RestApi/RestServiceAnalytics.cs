using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace SalveminiApp.RestApi
{
    public class RestServiceAnalytics : IRestServiceAnalytics
    {
        HttpClient client;
        public List<Models.Analytics> Analytics = new List<Models.Analytics>();
        public List<Models.EventLog> ConsoleEvents = new List<Models.EventLog>();
        public Models.AppInfo info = new Models.AppInfo();

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

        public async Task<List<Models.EventLog>> GetConsole()
        {
            ConsoleEvents = new List<Models.EventLog>();
            var uri = Costants.Uri("analytics/console");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ConsoleEvents = JsonConvert.DeserializeObject<List<Models.EventLog>>(content);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Error getting console", ex.Message);
            }
            return ConsoleEvents;
        }

        public async Task<Models.AppInfo> GetAppInfo()
        {
            info = new Models.AppInfo();

            var uri = Costants.Uri("utility/appinfo");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    info = JsonConvert.DeserializeObject<Models.AppInfo>(content);
                }
                else return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@" Error getting app info", ex.Message);
                return null;
            }
            return info;
        }

        public async Task<bool> PostAppInfo(Models.AppInfo newInfo)
        {

            var uri = Costants.Uri("utility/newappinfo");

            try
            {
                var json = JsonConvert.SerializeObject(newInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    return true;
                else return false;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore post new app info", ex.Message);
                return false;
            }

        }
    }

    public interface IRestServiceAnalytics
    {
        Task<List<Models.Analytics>> GetAnalytics();
        Task<List<Models.EventLog>> GetConsole();
        Task<Models.AppInfo> GetAppInfo();
        Task<bool> PostAppInfo(Models.AppInfo info);

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

        public Task<List<Models.EventLog>> GetConsole()
        {
            return restServiceAnalytics.GetConsole();
        }

        public Task<Models.AppInfo> GetAppInfo()
        {
            return restServiceAnalytics.GetAppInfo();
        }

       public Task<bool> PostAppInfo(Models.AppInfo info)
        {
            return restServiceAnalytics.PostAppInfo(info);
        }
    }
}
