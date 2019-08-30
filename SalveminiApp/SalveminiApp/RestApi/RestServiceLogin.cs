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

        public async Task<Models.ResponseModel> Login(Models.LoginForm loginData)
        {
            UtentiLogin = new List<Models.Utente>();
            Models.ResponseModel Data = new Models.ResponseModel();
            var uri = Costants.Uri("login");

            try
            {
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var responseContent = await response.Content.ReadAsStringAsync();
                        UtentiLogin = JsonConvert.DeserializeObject<List<Models.Utente>>(responseContent);
                        Data.Data = UtentiLogin;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Data.Message = "Username o password non corretti";
                        break;
                    case HttpStatusCode.NotAcceptable:
                        Data.Message = "I server di ARGO non sono al momento disponibili";
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return Data;
        }
    }

    public interface IRestServiceLogin
    {
        Task<Models.ResponseModel> Login(Models.LoginForm loginData);
    }

    public class ItemManagerLogin
    {
        IRestServiceLogin restServiceLogin;


        public ItemManagerLogin(IRestServiceLogin serviceLogin)
        {
            restServiceLogin = serviceLogin;
        }

        public Task<Models.ResponseModel> Login(Models.LoginForm loginData)
        {
            return restServiceLogin.Login(loginData);
        }
    }
}
