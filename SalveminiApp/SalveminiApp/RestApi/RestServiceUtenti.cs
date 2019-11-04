﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MonkeyCache.SQLite;
using Newtonsoft.Json;
using SalveminiApp.RestApi.Models;
using Xamarin.Essentials;
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
                //Get Cache if no Network Access
                if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet && Barrel.Current.Exists("utentilist"))
                {
                    return Barrel.Current.Get<List<Models.Utente>>("utentilist");
                }

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Utenti = JsonConvert.DeserializeObject<List<Models.Utente>>(content);

                    //Save Cache
                    Barrel.Current.Add("utentilist", Utenti, TimeSpan.FromDays(10));
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                {
                    if (Barrel.Current.Exists("utentilist"))
                    {
                        return Barrel.Current.Get<List<Models.Utente>>("utentilist");
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
            }
            return Utenti;
        }

        public async Task<Models.Utente> GetUtente(int id)
        {
            Utente = new Models.Utente();
            var uri = Costants.Uri("utenti/") + id;

            try
            {
                //Get Cache if no Network Access
                if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet && Barrel.Current.Exists("utente" + id.ToString()))
                {
                    return Barrel.Current.Get<Models.Utente>("utente" + id.ToString());
                }

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Utente = JsonConvert.DeserializeObject<Utente>(content);

                    //Save Cache
                    Barrel.Current.Add("utente" + id.ToString(), Utenti, TimeSpan.FromDays(10));
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                {
                    if (Barrel.Current.Exists("utente" + id.ToString()))
                    {
                        return Barrel.Current.Get<Models.Utente>("utente" + id.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
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
    }



    public interface IRestServiceUtenti
    {
        Task<List<Utente>> GetUtenti();
        Task<Utente> GetUtente(int id);
        Task<string[]> ChangeStatus(int idUtente, int stato);

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

    }
}
