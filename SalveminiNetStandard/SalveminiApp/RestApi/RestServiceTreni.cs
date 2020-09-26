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

namespace SalveminiApp.RestApi
{
    public class RestServiceTreni : IRestServiceTreni
    {
        HttpClient client;
        public Models.Treno NextTrain { get; private set; }
        public List<Models.Treno> Trains { get; private set; }
        public string TreniUpdate { get; private set; }

        public RestServiceTreni()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }

        public async Task<bool> GetTrainJson()
        {
            try
            {
                var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OrariTreni.txt");
                using (var client2 = new WebClient())
                {
                   client2.DownloadFile( Costants.Uri("Orari/OrariTreni.txt", false) , filename);
                }
                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);
                return false;
            }
        }


        public async Task<Models.Treno> GetNextTrain(int stazione, bool direzione)
        {
            NextTrain = new Models.Treno();
            Trains = new List<Models.Treno>();
            try
            {
                var assembly = typeof(RestServiceTreni).GetTypeInfo().Assembly;
                var manifest = assembly.GetManifestResourceNames();

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = Path.Combine(documents, "OrariTreni.txt");
                bool doesExist = File.Exists(filename);

                if (!doesExist)
                {
                    return null;
                }

                string text;

                text = File.ReadAllText(filename);

                //GetTrains
                Trains = JsonConvert.DeserializeObject<List<Models.Treno>>(text);

                //Filter by station and direction
                Trains = Trains.Where(x => x.Direzione == direzione && x.Stazione == stazione).OrderBy(x => x.LeaveTime).ToList();

                //Remove Campania Express
                Trains = Trains.Where(x => x.Importanza != "EXP").ToList();

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
                var assembly = typeof(RestServiceOrari).GetTypeInfo().Assembly;
                var manifest = assembly.GetManifestResourceNames();

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = Path.Combine(documents, "OrariTreni.txt");
                bool doesExist = File.Exists(filename);
                if (!doesExist)
                {
                    return null;
                }

                
                string text;

                text = File.ReadAllText(filename);

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

        Task<bool> GetTrainJson();
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

        public Task<bool> GetTrainJson()
        {
            return restServiceTreni.GetTrainJson();
        }
    }
}
