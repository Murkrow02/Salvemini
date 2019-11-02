﻿using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Reflection;
using System.Linq;
using MonkeyCache.SQLite;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace SalveminiApp.RestApi
{
    public class RestServiceArgo : IRestServiceArgo
    {
        HttpClient client;
        public List<Models.Assenza> Assenze { get; private set; }
        public List<Models.Promemoria> Promemoria { get; private set; }
        public List<Models.Pentagono> Medie { get; private set; }
        public List<Models.Compiti> Compiti { get; private set; }
        public List<Models.Argomenti> Argomenti { get; private set; }
        public List<Models.Voti> Voti { get; private set; }
        public List<Models.Bacheca> Bacheca { get; private set; }
        public Models.ScrutinioGrouped VotiScrutinio { get; private set; }
        public Models.WholeModel Oggi { get; private set; }


        public RestServiceArgo()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }


        public async Task<Models.ResponseModel> GetAssenze()
        {
            Assenze = new List<Models.Assenza>();
            Models.ResponseModel Data = new Models.ResponseModel();
            var uri = Costants.Uri("argo/assenze");
            try
            {
                var response = await client.GetAsync(uri);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Assenze = JsonConvert.DeserializeObject<List<Models.Assenza>>(content);
                        if (Assenze.Count > 0)
                        {
                            Assenze[Assenze.Count - 1].SeparatorVisibility = false;
                            if (Assenze.Count > 1)
                            {
                                Assenze[Assenze.Count - 1].CellPadding = new Thickness(10, 10, 10, 20);
                                Assenze[0].CellPadding = new Thickness(10, 20, 10, 10);
                            }
                        }
                        Data.Data = Assenze;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }

                Barrel.Current.Add("Assenze", Assenze, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GiustificaAssenza(RestApi.Models.AssenzaModel item)
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            string uri = Costants.Uri("argo/giustifica");
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Data.Data = response.IsSuccessStatusCode;
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
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetPromemoria()
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            Promemoria = new List<Models.Promemoria>();
            var uri = Costants.Uri("argo/promemoria");
            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Promemoria = JsonConvert.DeserializeObject<List<Models.Promemoria>>(content);
                        if (Promemoria.Count > 0)
                        {
                            Promemoria[Promemoria.Count - 1].SeparatorVisibility = false;
                            Promemoria[0].CellPadding = new Thickness(10, 20, 10, 10);
                            if (Promemoria.Count > 1)
                            {
                                Promemoria[Promemoria.Count - 1].CellPadding = new Thickness(10, 10, 10, 20);
                            }
                        }

                        Data.Data = Promemoria;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("Promemoria", Promemoria, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetCompiti()
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            Compiti = new List<Models.Compiti>();
            var uri = Costants.Uri("argo/compiti");
            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Compiti = JsonConvert.DeserializeObject<List<Models.Compiti>>(content);
                        if (Compiti.Count > 0)
                        {
                            Compiti[Compiti.Count - 1].SeparatorVisibility = false;
                            Compiti[0].CellPadding = new Thickness(10, 20, 10, 10);
                            if (Compiti.Count > 1)
                            {
                                Compiti[Compiti.Count - 1].CellPadding = new Thickness(10, 10, 10, 20);
                            }
                        }
                        Data.Data = Compiti;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("Compiti", Compiti, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetArgomenti()
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            Argomenti = new List<Models.Argomenti>();
            var uri = Costants.Uri("argo/argomenti");
            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Argomenti = JsonConvert.DeserializeObject<List<Models.Argomenti>>(content);
                        if (Argomenti.Count > 0)
                        {
                            Argomenti[Argomenti.Count - 1].SeparatorVisibility = false;
                            Argomenti[0].CellPadding = new Thickness(10, 20, 10, 10);
                            if (Argomenti.Count > 1)
                            {
                                Argomenti[Argomenti.Count - 1].CellPadding = new Thickness(10, 10, 10, 20);
                            }
                        }
                        Data.Data = Argomenti;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("Argomenti", Argomenti, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetPentagono()
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            Medie = new List<Models.Pentagono>();

            var uri = Costants.Uri("argo/pentagono");

            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Medie = JsonConvert.DeserializeObject<List<Models.Pentagono>>(content);
                        Data.Data = Medie;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("Medie", Medie, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetVoti()
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            Voti = new List<Models.Voti>();

            var uri = Costants.Uri("argo/voti");

            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Voti = JsonConvert.DeserializeObject<List<Models.Voti>>(content);
                        Data.Data = Voti;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("Voti", Voti, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetVotiScrutinio()
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            VotiScrutinio = new Models.ScrutinioGrouped();

            var uri = Costants.Uri("argo/scrutinio");

            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        VotiScrutinio = JsonConvert.DeserializeObject<Models.ScrutinioGrouped>(content);

                        //var voti = new List<Models.Scrutinio>();
                        ////{ new Models.Scrutinio { assenze = 2, desMateria = "Matematica", Voto = "10" }, new Models.Scrutinio { assenze = 5, desMateria = "Italiano", Voto = "8" } };
                        //VotiScrutinio.Primo = voti;

                        //var voti2 = new List<Models.Scrutinio> { new Models.Scrutinio { assenze = 1, desMateria = "Italiano", Voto = "5" } };
                        //VotiScrutinio.Secondo = voti2;
                        if (VotiScrutinio.Primo.ElementAtOrDefault(VotiScrutinio.Primo.Count - 1) != null)
                        {
                            VotiScrutinio.Primo[VotiScrutinio.Primo.Count - 1].SeparatorVisibility = false;
                        }
                        if (VotiScrutinio.Secondo.ElementAtOrDefault(VotiScrutinio.Secondo.Count - 1) != null)
                        {
                            VotiScrutinio.Secondo[VotiScrutinio.Secondo.Count - 1].SeparatorVisibility = false;
                        }

                        Data.Data = VotiScrutinio;

                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("VotiScrutinio", VotiScrutinio, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetBacheca()
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            Bacheca = new List<Models.Bacheca>();

            var uri = Costants.Uri("argo/bacheca");

            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Bacheca = JsonConvert.DeserializeObject<List<Models.Bacheca>>(content);
                        if (Bacheca.Count > 0)
                        {
                            Bacheca[Bacheca.Count - 1].SeparatorVisibility = false;
                            Bacheca[0].CellPadding = new Thickness(10, 20, 10, 10);
                            if (Bacheca.Count > 1)
                            {
                                Bacheca[Bacheca.Count - 1].CellPadding = new Thickness(10, 10, 10, 20);
                            }
                        }
                        Data.Data = Bacheca;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("Bacheca", Bacheca, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> GetOggi(DateTime data)
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            Oggi = new Models.WholeModel();
            var uri = Costants.Uri("oggi/" + data.ToString("yyyy-MM-dd"));
            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        Oggi = JsonConvert.DeserializeObject<Models.WholeModel>(content);
                        Data.Data = Oggi;
                        break;
                    case HttpStatusCode.Forbidden:
                        Data.Message = "Si è verificato un errore nella connessione ad ARGO";
                        break;
                    case HttpStatusCode.InternalServerError:
                        Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
                        break;
                }
                Barrel.Current.Add("Oggi" + data.ToString("yyyy-MM-dd"), Oggi, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }

        public async Task<Models.ResponseModel> VisualizzaBacheca(RestApi.Models.VisualizzaBacheca item)
        {
            Models.ResponseModel Data = new Models.ResponseModel();
            string uri = Costants.Uri("argo/visualizzabacheca");
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Data.Data = response.IsSuccessStatusCode;
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
                Data.Message = "Si è verificato un errore, contattaci se il problema persiste";
            }
            return Data;
        }
    }

    public interface IRestServiceArgo
    {
        Task<Models.ResponseModel> GetAssenze();
        Task<Models.ResponseModel> GiustificaAssenza(RestApi.Models.AssenzaModel item);
        Task<Models.ResponseModel> VisualizzaBacheca(RestApi.Models.VisualizzaBacheca item);
        Task<Models.ResponseModel> GetPromemoria();
        Task<Models.ResponseModel> GetPentagono();
        Task<Models.ResponseModel> GetCompiti();
        Task<Models.ResponseModel> GetArgomenti();
        Task<Models.ResponseModel> GetVoti();
        Task<Models.ResponseModel> GetVotiScrutinio();
        Task<Models.ResponseModel> GetOggi(DateTime data);
        Task<Models.ResponseModel> GetBacheca();
    }

    public class ItemManagerArgo
    {
        IRestServiceArgo restServiceArgo;


        public ItemManagerArgo(IRestServiceArgo serviceArgo)
        {
            restServiceArgo = serviceArgo;
        }

        public Task<Models.ResponseModel> GetAssenze()
        {
            return restServiceArgo.GetAssenze();
        }

        public Task<Models.ResponseModel> GetCompiti()
        {
            return restServiceArgo.GetCompiti();
        }

        public Task<Models.ResponseModel> GetArgomenti()
        {
            return restServiceArgo.GetArgomenti();
        }

        public Task<Models.ResponseModel> GiustificaAssenza(RestApi.Models.AssenzaModel item)
        {
            return restServiceArgo.GiustificaAssenza(item);
        }

        public Task<Models.ResponseModel> VisualizzaBacheca(RestApi.Models.VisualizzaBacheca item)
        {
            return restServiceArgo.VisualizzaBacheca(item);
        }

        public Task<Models.ResponseModel> GetPromemoria()
        {
            return restServiceArgo.GetPromemoria();
        }

        public Task<Models.ResponseModel> GetPentagono()
        {
            return restServiceArgo.GetPentagono();
        }

        public Task<Models.ResponseModel> GetVoti()
        {
            return restServiceArgo.GetVoti();
        }
        public Task<Models.ResponseModel> GetVotiScrutinio()
        {
            return restServiceArgo.GetVotiScrutinio();
        }

        public Task<Models.ResponseModel> GetOggi(DateTime data)
        {
            return restServiceArgo.GetOggi(data);
        }

        public Task<Models.ResponseModel> GetBacheca()
        {
            return restServiceArgo.GetBacheca();
        }
    }
}

