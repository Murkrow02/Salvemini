using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaturdayMP.XPlugins.Notifications;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.RestApi
{
    public class RestServiceAds : IRestServiceAds
    {
        HttpClient client;

        public RestServiceAds()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));

        }

        public async Task<Models.ResponseModel> canWatchAd()
        {
            var uri = Costants.Uri("ads/canWatchAd");

            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        var count = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());

                        //Alert user next time with push notification
                        if (count == 3)
                        {
                            var notificationScheduler = DependencyService.Get<INotificationScheduler>();
                            notificationScheduler.Create("sCoin gratis 🤑", "Il tuo premio giornaliero ti sta aspettando, che aspetti? Prendi le tue sCoin GRATIS!!!", DateTime.Now.AddDays(1));
                        }

                        return new Models.ResponseModel { Data = true, Message = "" };
                    case System.Net.HttpStatusCode.NotAcceptable:
                        return new Models.ResponseModel { Data = false, Message = "Hai guadagnato già abbastanza sCoin per oggi, ritorna domani per prenderne di nuove!" };
                    default:
                        return new Models.ResponseModel { Data = false, Message = "Si è verificato un errore sconosciuto, contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore can watch ad api", ex.Message);
                return new Models.ResponseModel { Data = false, Message = "Si è verificato un errore durante la richiesta, constrolla la tua connessione e riprova" };
            }
        }

        public async Task<Models.ResponseModel> watchedAd()
        {
            var uri = Costants.Uri("ads/watchedAd");

            try
            {
                var response = await client.GetAsync(uri);
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        return new Models.ResponseModel { Data = true, Message = "" };
                    case System.Net.HttpStatusCode.NotAcceptable:
                        return new Models.ResponseModel { Data = false, Message = "Hai guadagnato già abbastanza sCoin per oggi, ritorna domani per prenderne di nuove!" };
                    default:
                        return new Models.ResponseModel { Data = false, Message = "Si è verificato un errore sconosciuto, contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Errore can watch ad api", ex.Message);
                return new Models.ResponseModel { Data = false, Message = "Si è verificato un errore durante la richiesta, constrolla la tua connessione e riprova" };
            }
        }
    }

    public interface IRestServiceAds
    {
        Task<Models.ResponseModel> canWatchAd();
        Task<Models.ResponseModel> watchedAd();
    }

    public class AdsManager
    {
        IRestServiceAds restServiceAds;

        public AdsManager(IRestServiceAds serviceAnalytics)
        {
            restServiceAds = serviceAnalytics;
        }

        public Task<Models.ResponseModel> canWatchAd()
        {
            return restServiceAds.canWatchAd();
        }

        public Task<Models.ResponseModel> watchedAd()
        {
            return restServiceAds.watchedAd();
        }

    }
}
