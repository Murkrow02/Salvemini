using System;
using UIKit;
using IntentsKit;
using SalveminiApp;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using ObjCRuntime;
using Newtonsoft.Json;

namespace IntentsKit
{
    public class TrenoHandler : TrenoIntentHandling
    {
        public override void ConfirmTreno(TrenoIntent intent, Action<TrenoIntentResponse> completion)
        {
            completion(new TrenoIntentResponse(TrenoIntentResponseCode.Ready, null));
            //base.ConfirmTreno(intent, completion);
        }

        public override void HandleTreno(TrenoIntent intent, Action<TrenoIntentResponse> completion)
        {
            //Get direction and station
            var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
            defaults.AddSuite("group.com.codex.SalveminiApp");
            var station = defaults.IntForKey(new NSString("SiriStation"));
            var direction = defaults.BoolForKey(new NSString("SiriDirection"));

            //Download strings
            WebClient wc = new WebClient();
            var json = wc.DownloadString("https://www.mysalvemini.me/api/orari/siri/" + station + "/" + direction + "/true");
            var response = JsonConvert.DeserializeObject<List<Models.Treno>>(json);

            //Success, send to siri
            completion(TrenoIntentResponse.SuccessIntentResponseWithCitta(response[0].DirectionString, response[0].Partenza + " da " + Stazioni[response[0].Stazione]));


        }

        public static Dictionary<int, string> Stazioni = new Dictionary<int, string>
        {
            {0, "Sorrento"},
            {1, "S. Agnello"},
            {2, "Piano"},
            {3, "Meta"},
            {4, "Seiano"},
            {5, "Vico Equense"},
            {6, "Castellammare di Stabia"},
            {7, "Via Nocera"},
            {8, "Pioppaino"},
            {9, "Moregine"},
            {10, "Pompei Scavi"},
            {11, "Villa Regina"},
            {12, "Torre Annunziata"},
            {13, "Trecase"},
            {14, "Via Viuli"},
            {15, "Leopardi"},
            {16, "Villa delle Ginestre"},
            {17, "Via dei Monaci"},
            {18, "Via del Monte"},
            {19, "Via S'Antonio"},
            {20, "Torre del Greco"},
            {21, "Ercolano Miglio d'Oro"},
            {22, "Ercolano Scavi"},
            {23, "Portici Via Libertà"},
            {24, "Portici Bellavista"},
            {25, "S'Giorgio Cavalli di Bronzo"},
            {26, "S'Giorgio a Cremano"},
            {27, "S'Maria del Pozzo"},
            {28, "Barra"},
            {29, "S'Giovanni a Teduccio"},
            {30, "Via Gianturco"},
            {31, "Napoli Piazza Garibaldi"},
            {32, "Napoli Porta Nolana"},
        };

    }

    public class OrarioHandler : OrarioIntentHandling
    {
        public override void ConfirmOrario(OrarioIntent intent, Action<OrarioIntentResponse> completion)
        {
            completion(new OrarioIntentResponse(OrarioIntentResponseCode.Ready, null));
            //base.ConfirmTreno(intent, completion);
        }

        public override void HandleOrario(OrarioIntent intent, Action<OrarioIntentResponse> completion)
        {
            try
            {
                //Get his class
                var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
                defaults.AddSuite("group.com.codex.SalveminiApp");
                var classe = defaults.StringForKey("SiriClass");

                //Download strings
                WebClient wc = new WebClient();
                var json = wc.DownloadString("https://www.mysalvemini.me/api/orari/siri/" + classe);
                string[] response = JsonConvert.DeserializeObject<string[]>(json);

                //Success, send to siri
                completion(OrarioIntentResponse.SuccessIntentResponseWithGiorno(response[0], response[1]));

            }
            catch
            {
                completion(OrarioIntentResponse.FailureIntentResponseWithGiorno("domani"));

            }
        }

    }
}




