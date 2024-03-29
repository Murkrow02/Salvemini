﻿using System;
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
    public class RestServiceSondaggi : IRestServiceSondaggi
    {
        HttpClient client;
        public List<Models.SondaggiResult> Risultati { get; private set; }

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
                        return new string[] { "Grazie!", "Il tuo voto è stato inviato correttamente" };
                    case HttpStatusCode.Conflict:
                        return new string[] { "Attenzione", "Hai già inviato un voto per questo sondaggio" };
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

        public async Task<string[]> PostSondaggio(Sondaggi sondaggio)
        {
            var uri = Costants.Uri("sondaggi");

            try
            {
                var json = JsonConvert.SerializeObject(sondaggio);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Grazie!", "Il tuo sondaggio è stato inviato correttamente!" };
                    default:
                        return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Crea sondaggio ", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

        public async Task<List<SondaggiResult>> ReturnRisultati(int id)
        {
            Risultati = new List<SondaggiResult>();
            var uri = Costants.Uri("sondaggi/risultati/") + id.ToString();

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Risultati = JsonConvert.DeserializeObject<List<SondaggiResult>>(content);

                    //Save Cache
                    Barrel.Current.Add("risultati" + id.ToString(), Risultati, TimeSpan.FromDays(10));
                }
                else
                {
                    return CacheHelper.GetCache<List<SondaggiResult>>("risultati" + id.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore risultati sondaggi", ex.Message);
            }
            return Risultati;
        }
    }
    public interface IRestServiceSondaggi
    {
        Task<string[]> PostVoto(VotoSondaggio voto);
        Task<string[]> PostSondaggio(Sondaggi sondaggio);
        Task<List<SondaggiResult>> ReturnRisultati(int id);
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

        public Task<string[]> PostSondaggio(Sondaggi sondaggio)
        {
            return restServiceSondaggi.PostSondaggio(sondaggio);
        }

        public Task<List<SondaggiResult>> ReturnRisultati(int id)
        {
            return restServiceSondaggi.ReturnRisultati(id);
        }
    }
}
