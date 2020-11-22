using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.RestApi
{
    public class RestServiceFondo : IRestServiceFondo
    {
        HttpClient client;

        public RestServiceFondo()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public List<Models.FondoStudentesco> Fondo;
        public async Task<List<Models.FondoStudentesco>> GetTransactions()
        {
            var uri = Costants.Uri("fondo/transactions");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    Fondo = JsonConvert.DeserializeObject<List<Models.FondoStudentesco>>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Fondo = new List<Models.FondoStudentesco>();
                }

                return Fondo;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> PublishTransaction(Models.FondoStudentesco fondo)
        {
            var uri = Costants.Uri("fondo/transactions");

            try
            {
                var json = JsonConvert.SerializeObject(fondo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public interface IRestServiceFondo
    {
        Task<List<Models.FondoStudentesco>> GetTransactions();
        Task<bool> PublishTransaction(Models.FondoStudentesco fondo);
    }

    public class FondoManager
    {
        IRestServiceFondo restServiceFondo;

        public FondoManager(IRestServiceFondo serviceAnalytics)
        {
            restServiceFondo = serviceAnalytics;
        }

        public Task<List<Models.FondoStudentesco>> GetTransactions()
        {
            return restServiceFondo.GetTransactions();
        }

        public Task<bool> PublishTransaction(Models.FondoStudentesco fondo)
        {
            return restServiceFondo.PublishTransaction(fondo);
        }
    }
}
