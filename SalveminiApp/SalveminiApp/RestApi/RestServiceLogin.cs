using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Net;

namespace SalveminiApp.RestApi
{
    public class RestServiceLogin : IRestServiceLogin
    {
        HttpClient client;
        public List<Models.Utente> UtentiLogin { get; private set; }
        public RestServiceLogin()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<List<Models.Utente>> Login(Models.LoginForm loginData)
        {
            UtentiLogin = new List<Models.Utente>();
            var uri = Costants.Uri("login");

            try
            {
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    UtentiLogin = JsonConvert.DeserializeObject<List<Models.Utente>>(responseContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return UtentiLogin;
        }
    }

    public interface IRestServiceLogin
    {
        Task<List<Models.Utente>> Login(Models.LoginForm loginData);
    }

    public class ItemManagerLogin
    {
        IRestServiceLogin restServiceLogin;


        public ItemManagerLogin(IRestServiceLogin serviceLogin)
        {
            restServiceLogin = serviceLogin;
        }

        public Task<List<Models.Utente>> Login(Models.LoginForm loginData)
        {
            return restServiceLogin.Login(loginData);
        }
    }
}
