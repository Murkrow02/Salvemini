using System;
using System.Collections.Generic;
using Foundation;
using Intents;


namespace SalveminiAppIntent
{
    [Register("IntentHandler")]
    public class IntentHandler : INExtension
    {
        public override NSObject GetHandler(INIntent intent)
        {
            if (intent is SalveminiApp.TrenoIntent)
            {
                return new IntentsKit.TrenoHandler();
            }
            else if (intent is SalveminiApp.OrarioIntent)
            {
                return new IntentsKit.OrarioHandler();
            }

            throw new Exception("Unhandled intent type: ${intent}");
        }

        protected IntentHandler(IntPtr handle) : base(handle) { }
    }
}

