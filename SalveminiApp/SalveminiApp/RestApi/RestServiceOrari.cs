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
    public class RestServiceOrari : IRestServiceOrari
    {
        HttpClient client;
        public List<Models.Lezione> Lezioni { get; private set; }

        public RestServiceOrari()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<List<Models.Lezione>> GetOrario(string classe, int day)
        {
            Lezioni = new List<Models.Lezione>();
            try
            {
                var assembly = typeof(RestServiceOrari).GetTypeInfo().Assembly;

                var osgu = assembly.GetManifestResourceNames();

#if __ANDROID__
                Stream stream = assembly.GetManifestResourceStream("SalveminiApp.Droid.Helpers.OrariClassi." + classe + ".txt");

#endif
#if __IOS__
                Stream stream = assembly.GetManifestResourceStream("SalveminiApp.iOS.Helpers.OrariClassi." + classe + ".txt");
#endif

                string text;

                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }

                Lezioni = JsonConvert.DeserializeObject<List<Models.Lezione>>(text);

                //Filter list per day and order by hour
                Lezioni = Lezioni.Where(x => x.Giorno == day).OrderBy(x => x.Ora).ToList();


                //Group hours by duration
                for (int i = 0; i < Lezioni.Count; i++)
                {
                    //Skip hour if is equal to the previous
                    if (Lezioni.ElementAtOrDefault(i - 1) != null && Lezioni[i].Materia == Lezioni[i - 1].Materia)
                    {
                        Lezioni[i].toRemove = true;
                        continue;
                    }
                    int next = 1;

                    Models.Lezione lezione()
                    {
                        //Check if next hour is the same of this hour
                        if (Lezioni.ElementAtOrDefault(i + next) != null && Lezioni[i].Materia == Lezioni[i + next].Materia)
                        {
                            //Increment hours number
                            Lezioni[i].numOre = next + 1;
                            //Incre
                            next++;
                            lezione();
                        }

                            
                        return Lezioni[i];
                    }

                    Lezioni[i] = lezione();

                }

                foreach (var lezione in Lezioni)
                {
                    if (lezione.numOre == 0)
                    {
                        //Set Number of hours to default
                        lezione.numOre = 1;
                    }

                    //Setup Height Of Hour
                    lezione.OrarioFrameHeight = (App.ScreenHeight / 24) * lezione.numOre;

                    //Setup Corner Radius
                    lezione.OrarioFrameRadius = lezione.numOre > 1 ? 20 : 15;
                }
                Lezioni = Lezioni.Where(x => x.toRemove == false).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              ERROR {0}", ex.Message);

            }
            return Lezioni;
        }
    }

    public interface IRestServiceOrari
    {
        Task<List<Models.Lezione>> GetOrario(string classe, int day);
    }

    public class ItemManagerOrari
    {
        IRestServiceOrari restServiceOrari;


        public ItemManagerOrari(IRestServiceOrari serviceOrari)
        {
            restServiceOrari = serviceOrari;
        }

        public Task<List<Models.Lezione>> GetOrario(string classe, int day)
        {
            return restServiceOrari.GetOrario(classe, day);
        }

    }
}
