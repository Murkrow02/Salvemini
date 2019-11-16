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


            completion(TrenoIntentResponse.SuccessIntentResponseWithCitta("Sorrento", "12:50"));
        }

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




