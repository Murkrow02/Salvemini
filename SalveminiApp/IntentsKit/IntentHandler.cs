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

namespace IntentsKit
{
    public class TrenoHandler: TrenoIntentHandling
    {
        public override void ConfirmTreno(TrenoIntent intent, Action<TrenoIntentResponse> completion)
        {
            completion(new TrenoIntentResponse(TrenoIntentResponseCode.Ready, null));
            //base.ConfirmTreno(intent, completion);
        }

        public override void HandleTreno(TrenoIntent intent, Action<TrenoIntentResponse> completion)
        {
            completion(TrenoIntentResponse.SuccessIntentResponseWithCitta("Sorrento","12:50"));
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
            completion(OrarioIntentResponse.SuccessIntentResponseWithGiorno("Domani", "Letteratura, Matematica, Fisica, Filosofia"));
        }

    }
}




