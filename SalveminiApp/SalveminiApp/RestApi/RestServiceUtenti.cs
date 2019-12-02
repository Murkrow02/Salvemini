using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MonkeyCache.SQLite;
using Newtonsoft.Json;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;
using System.Text;

namespace SalveminiApp.RestApi
{
    public class RestServiceUtenti : IRestServiceUtenti
    {
        HttpClient client;
        public List<Models.Utente> Utenti { get; private set; }
        public Models.Utente Utente { get; private set; }

        public RestServiceUtenti()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<List<Models.Utente>> GetUtenti()
        {
            Utenti = new List<Models.Utente>();
            var uri = Costants.Uri("utenti/all");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Utenti = JsonConvert.DeserializeObject<List<Models.Utente>>(content);

                    //Save Cache
                    Barrel.Current.Add("utentilist", Utenti, TimeSpan.FromDays(10));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore lista utenti", ex.Message);
            }
            return Utenti;
        }

        public async Task<Models.Utente> GetUtente(int id)
        {
            Utente = new Models.Utente();
            var uri = Costants.Uri("utenti/") + id;

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Utente = JsonConvert.DeserializeObject<Utente>(content);

                    //Save Cache
                    Barrel.Current.Add("utente" + id.ToString(), Utente, TimeSpan.FromDays(10));
                }
                else
                {
                    return CacheHelper.GetCache<Models.Utente>("utente" + id.ToString());
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore utente donwload", ex.Message);
            }
            return Utente;
        }

        public async Task<string[]> ChangeStatus(int idUtente, int stato)
        {
            var uri = Costants.Uri("utenti/change/" + idUtente.ToString() + "/" + stato.ToString());

            try
            {
                var response = await client.GetAsync(uri);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", "Lo stato dell'utente è stato aggiornato, ricarica la lista per vedere i cambiamenti" };
                    case HttpStatusCode.Unauthorized:
                        return new string[] { "Errore", "Non hai l'autorizzazione per aggiornare lo stato di questo utente" };
                    case HttpStatusCode.NotFound:
                        return new string[] { "Errore", "L'utente selezionato non è stato trovato" };
                    case HttpStatusCode.InternalServerError:
                        return new string[] { "Errore", "Si è verificato un errore durante l'operazione, riprova più tardi o contattaci se il problema persiste" };
                    default:
                        return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Cambia stato utente", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

        public async Task<Models.ResponseModel> ChangePwd(Models.changeBlock changeData)
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            var uri = Costants.Uri("argo/changepwd");
            try
            {
                var json = JsonConvert.SerializeObject(changeData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Data.Data = true;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "La vecchia password non è corretta";
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



    public interface IRestServiceUtenti
    {
        Task<List<Utente>> GetUtenti();
        Task<Utente> GetUtente(int id);
        Task<string[]> ChangeStatus(int idUtente, int stato);
        Task<Models.ResponseModel> ChangePwd(Models.changeBlock changeData);

    }

    public class ItemManagerUtenti
    {
        IRestServiceUtenti restServiceUtenti;


        public ItemManagerUtenti(IRestServiceUtenti serviceUtenti)
        {
            restServiceUtenti = serviceUtenti;
        }

        public Task<List<Utente>> GetUtenti()
        {
            return restServiceUtenti.GetUtenti();
        }

        public Task<Utente> GetUtente(int id)
        {
            return restServiceUtenti.GetUtente(id);
        }

        public Task<string[]> ChangeStatus(int idUtente, int stato)
        {
            return restServiceUtenti.ChangeStatus(idUtente, stato);
        }

        public Task<Models.ResponseModel> ChangePwd(Models.changeBlock changeData)
        {
            return restServiceUtenti.ChangePwd(changeData);
        }
    }
}
