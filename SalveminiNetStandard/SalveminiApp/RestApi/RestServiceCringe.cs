using System;
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
    public class RestServiceCringe : IRestServiceCringe
    {
        HttpClient client;
        List<DomandeReturn> domande = new List<DomandeReturn>();
        CommentiReturn commenti = new CommentiReturn();
        List<Notifiche> notifiche = new List<Notifiche>();
        List<NewNotifiche> newNotifiche = new List<NewNotifiche>();

        public RestServiceCringe()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<List<DomandeReturn>> GetFeed(int id)
        {
            domande = new List<DomandeReturn>();
            var uri = Costants.Uri("icringe/feed/") + id.ToString();

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    domande = JsonConvert.DeserializeObject<List<DomandeReturn>>(content);

                    //Save Cache
                    if (id == -1)
                        Barrel.Current.Add("cringefeed", domande, TimeSpan.FromDays(20));
                }
                else
                {
                    //Return cache if error
                    return CacheHelper.GetCache<List<DomandeReturn>>("cringefeed");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore feed iCringe", ex.Message);
                return null;
            }
            return domande;
        }

        public async Task<CommentiReturn> GetCommenti(int id)
        {
            commenti = new CommentiReturn();
            var uri = Costants.Uri("icringe/commenti/") + id.ToString();

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    commenti = JsonConvert.DeserializeObject<CommentiReturn>(content);

                    //Save Cache
                    Barrel.Current.Add("commenti" + id, commenti, TimeSpan.FromDays(20));
                }
                else
                {
                    //Return cache if error
                    return CacheHelper.GetCache<CommentiReturn>("commenti" + id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore feed commenti", ex.Message);
                return null;
            }
            return commenti;
        }

        public async Task<string[]> PostDomanda(string domanda)
        {
            var uri = Costants.Uri("icringe/postdomanda");

            try
            {
                var json = JsonConvert.SerializeObject(domanda);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                var messaggio = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", messaggio  };
                    default:
                        return new string[] { "Errore", messaggio  };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore post domanda", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

        public async Task<string[]> PostCommento(Commenti commento)
        {
            var uri = Costants.Uri("icringe/postcommento");

            try
            {
                var json = JsonConvert.SerializeObject(commento);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                var messaggio = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", messaggio  };
                    default:
                        return new string[] { "Errore", messaggio  };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore post commento", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

        public async Task<string[]> DeleteCommento(int id)
        {
            var uri = Costants.Uri("icringe/removecommento/" + id);

            try
            {
                var response = await client.GetAsync(uri);
                var messaggio = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", messaggio  };
                    default:
                        return new string[] { "Errore", messaggio  };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore delete commento", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

        public async Task<string[]> DeletePost(int id)
        {
            var uri = Costants.Uri("icringe/removepost/" + id);

            try
            {
                var response = await client.GetAsync(uri);
                var messaggio = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", messaggio  };
                    default:
                        return new string[] { "Errore", messaggio  };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore delete post", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

        public async Task<List<Notifiche>> GetNotifiche()
        {
            notifiche = new List<Notifiche>();
            string uri;
            uri = Costants.Uri("icringe/getnotifiche");
            //nuove ? Costants.Uri("icringe/getnewnotifiche/" + id) : 

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    notifiche = JsonConvert.DeserializeObject<List<Notifiche>>(content);

                    //Save Cache
                        Barrel.Current.Add("cringenotifiche", notifiche, TimeSpan.FromDays(20));
                }
                else
                {
                    //Return cache if error
                        return CacheHelper.GetCache<List<Notifiche>>("cringenotifiche");
     
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore feed notifiche", ex.Message);
                return null;
            }
            return notifiche;
        }

        public async Task<List<NewNotifiche>> GetNewNotifiche(int id)
        {
            newNotifiche = new List<NewNotifiche>();
            string uri;
            uri = Costants.Uri("icringe/getnewnotifiche/" + id);

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    newNotifiche = JsonConvert.DeserializeObject<List<NewNotifiche>>(content);
                }
                else
                {
                        return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore new notifiche", ex.Message);
                return null;
            }
            return newNotifiche;
        }


        public async Task<List<Domande>> approveList()
        {
            var domande_ = new List<Domande>();
            var uri = Costants.Uri("icringe/approvelist");

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    domande_ = JsonConvert.DeserializeObject<List<Domande>>(content);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore feed iCringe", ex.Message);
                return null;
            }
            return domande_;
        }

        public async Task<string[]> ApprovaDomanda(int id, bool stato)
        {
            var uri = Costants.Uri("icringe/approva/" + id + "/" + stato);

            try
            {
                var response = await client.GetAsync(uri);
                var messaggio = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Successo", messaggio != null ? messaggio : ""};
                    default:
                        return new string[] { "Errore", messaggio != null ? messaggio : "" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore post domanda", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }
    }

    public interface IRestServiceCringe
    {
        Task<List<DomandeReturn>> GetFeed(int id);
        Task<CommentiReturn> GetCommenti(int id);
        Task<string[]> PostDomanda(string domanda);
        Task<string[]> PostCommento(Commenti commento);
        Task<string[]> DeleteCommento(int id);
        Task<string[]> DeletePost(int id);
        Task<List<Notifiche>> GetNotifiche();
        Task<List<NewNotifiche>> GetNewNotifiche(int id);
        Task<List<Domande>> approveList();
        Task<string[]> ApprovaDomanda(int id, bool stato);
    }

    public class CringeManager
    {
        IRestServiceCringe restServiceCringe;

        public CringeManager(IRestServiceCringe serviceCringe)
        {
            restServiceCringe = serviceCringe;
        }

        public Task<List<DomandeReturn>> GetFeed(int id)
        {
            return restServiceCringe.GetFeed(id);
        }

        public Task<CommentiReturn> GetCommenti(int id)
        {
            return restServiceCringe.GetCommenti(id);
        }

        public Task<List<Notifiche>> GetNotifiche()
        {
            return restServiceCringe.GetNotifiche();
        }

        public Task<List<NewNotifiche>> GetNewNotifiche(int id)
        {
            return restServiceCringe.GetNewNotifiche(id);
        }

        public Task<string[]> PostDomanda(string domanda)
        {
            return restServiceCringe.PostDomanda(domanda);
        }

        public Task<string[]> PostCommento(Commenti commento)
        {
            return restServiceCringe.PostCommento(commento);
        }

        public Task<string[]> DeleteCommento(int id)
        {
            return restServiceCringe.DeleteCommento(id);
        }

        public Task<string[]> DeletePost(int id)
        {
            return restServiceCringe.DeletePost(id);
        }

        public Task<List<Domande>> approveList()
        {
            return restServiceCringe.approveList();
        }

        public Task<string[]> ApprovaDomanda(int id, bool stato)
        {
            return restServiceCringe.ApprovaDomanda(id, stato);
        }
    }
}
