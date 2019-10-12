using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;

namespace SalveminiApp.RestApi
{
    public class RestServiceSondaggi: IRestServiceSondaggi
    {
        HttpClient client;

        public RestServiceSondaggi()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }


        public async Task<string[]> PostVoto(VotoSondaggio voto)
        {
            var uri = Costants.Uri("sondaggi/vota");

            try
            {
                var json = JsonConvert.SerializeObject(voto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    //todo istruttore con troppe schede
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", "L'avviso è stato creato" };
                    case HttpStatusCode.Unauthorized:
                        return new string[] { "Errore", "Non hai l'autorizzazione per creare un avviso" };
                    case HttpStatusCode.InternalServerError:
                        return new string[] { "Errore", "Si è verificato un errore nella creazione dell'avviso, riprova più tardi o contattaci se il problema persiste" };
                    default:
                        return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }
    }
    public interface IRestServiceSondaggi
    {
        Task<string[]> PostVoto(VotoSondaggio voto);

    }

    public class SondaggiManager
    {
        IRestServiceSondaggi restServiceSondaggi;

        public SondaggiManager(IRestServiceSondaggi serviceSondaggi)
        {
            restServiceSondaggi = serviceSondaggi;
        }

        public Task<string[]> PostVoto(VotoSondaggio voto)
        {
            return restServiceSondaggi.PostVoto(voto);
        }
    }
}
