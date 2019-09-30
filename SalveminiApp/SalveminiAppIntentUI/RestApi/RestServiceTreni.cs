using System;
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

namespace SalveminiAppIntentUI.RestApi
{
    public class RestServiceTreni : IRestServiceTreni
    {
        HttpClient client;
        public Models.Treno NextTrain { get; private set; }
        public List<Models.Treno> Trains { get; private set; }
        public RestServiceTreni()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<Models.Treno> GetNextTrain(int stazione, bool direzione)
        {
            NextTrain = new Models.Treno();
            Trains = new List<Models.Treno>();
            try
            {
                var assembly = typeof(RestServiceTreni).GetTypeInfo().Assembly;
                var manifest = assembly.GetManifestResourceNames();

#if __ANDROID__
                Stream stream = assembly.GetManifestResourceStream("SalveminiApp.Droid.Helpers.OrariTreni.txt");

#endif
#if __IOS__
                Stream stream = assembly.GetManifestResourceStream("SalveminiAppIntentUI.OrariTreni.txt");
#endif

                string text;

                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }

                //GetTrains
                Trains = JsonConvert.DeserializeObject<List<Models.Treno>>(text);

                //Filter by station and direction
                Trains = Trains.Where(x => x.Direzione == direzione && x.Stazione == stazione).OrderBy(x => x.LeaveTime).ToList();

                //Remove Campania Express
                Trains = Trains.Where(x => x.Importanza != "EXP").ToList();

                //Check if there are more trains
                if (Trains[Trains.Count - 1].LeaveTime > new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, 0))
                {
                    //Remove previous trains
                    Trains = Trains.Where(x => x.LeaveTime > new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, 0)).ToList();
                }
                else
                {
                    //Return first train of the day
                    return Trains[0];
                }

                //Filter by Variations
                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    //Remove Feriali if sunday
                    Trains = Trains.Where(x => x.Variazioni != "FER").ToList();
                }
                else
                {
                    //Remove Festivi if not sunday
                    Trains = Trains.Where(x => x.Variazioni != "FES").ToList();
                }

                NextTrain = Trains[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);

            }
            return NextTrain;
        }

        public async Task<List<Models.Treno>> GetTrains(int stazione, bool direzione)
        {
            Trains = new List<Models.Treno>();
            try
            {
                var assembly = typeof(RestServiceTreni).GetTypeInfo().Assembly;
                var manifest = assembly.GetManifestResourceNames();

#if __ANDROID__
                Stream stream = assembly.GetManifestResourceStream("SalveminiApp.Droid.Helpers.OrariTreni.txt");

#endif
#if __IOS__
                Stream stream = assembly.GetManifestResourceStream("SalveminiApp.iOS.Helpers.OrariTreni.txt");
#endif

                string text;

                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }

                //GetTrains
                Trains = JsonConvert.DeserializeObject<List<Models.Treno>>(text);

                Trains = Trains.Where(x => x.Stazione == stazione && x.Direzione == direzione).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);

            }
            return Trains;
        }
    }

    public interface IRestServiceTreni
    {
        Task<Models.Treno> GetNextTrain(int stazione, bool direzione);

        Task<List<Models.Treno>> GetTrains(int stazione, bool direzione);
    }

    public class ItemManagerTreni
    {
        IRestServiceTreni restServiceTreni;


        public ItemManagerTreni(IRestServiceTreni serviceTreni)
        {
            restServiceTreni = serviceTreni;
        }

        public Task<Models.Treno> GetNextTrain(int stazione, bool direzione)
        {
            return restServiceTreni.GetNextTrain(stazione, direzione);
        }

        public Task<List<Models.Treno>> GetTrains(int stazione, bool direzione)
        {
            return restServiceTreni.GetTrains(stazione, direzione);
        }
    }
}
