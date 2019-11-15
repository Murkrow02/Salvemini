using System;
using CoreGraphics;
using Foundation;
using Intents;
using IntentsUI;
using TrainKit.Data;
using UIKit;
using SalveminiApp;
using CoreFoundation;
using Xamarin.Essentials;
using System.Diagnostics;
using System.Collections.Generic;

namespace SalveminiAppIntentUI
{
    // As an example, this extension's Info.plist has been configured to handle interactions for INSendMessageIntent.
    // You will want to replace this or add other intents as appropriate.
    // The intents whose interactions you wish to handle must be declared in the extension's Info.plist.

    // You can test this example integration by saying things to Siri like:
    // "Send a message using <myApp>"
    public partial class IntentViewController : UIViewController, IINUIHostedViewControlling
    {
        public static RestApi.ItemManagerTreni Treni { get; private set; }
        protected IntentViewController(IntPtr handle) : base(handle)
        {
            Treni = new RestApi.ItemManagerTreni(new RestApi.RestServiceTreni());
            // Note: this .ctor should not contain any initialization logic.
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //GetTrains
            var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
            var stazione = (int)defaults.IntForKey("savedStation");
            var direzione = defaults.BoolForKey("savedDirection");

            Debug.WriteLine(stazione);

            var NextTrain = await Treni.GetNextTrain(stazione, direzione);
            if (NextTrain != null)
            {
                var a = new NSMutableAttributedString();
                a.Append(new NSAttributedString("Il prossimo treno per ", new UIStringAttributes { Font = UIFont.SystemFontOfSize(15) }));
                a.Append(new NSAttributedString(NextTrain.DirectionString, new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(15) }));
                a.Append(new NSAttributedString(" parte alle ", new UIStringAttributes { Font = UIFont.SystemFontOfSize(15) }));
                a.Append(new NSAttributedString(NextTrain.Partenza, new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(15) }));
                a.Append(new NSAttributedString(" da ", new UIStringAttributes { Font = UIFont.SystemFontOfSize(15) }));
                a.Append(new NSAttributedString(Stazioni[stazione], new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(15) }));

                osgu.AttributedText = a;
            }
            else
            {
                osgu.Text = "Errore nel caricamento del treno";
            }
            // Do any required interface initialization here.
        }
        
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public void Configure(INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
        {
            // Do configuration here, including preparing views and calculating a desired size for presentation.

            if (completion != null)
                completion(new CGSize(this.ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320, 150));
        }

        CGSize DesiredSize()
        {
            return ExtensionContext.GetHostedViewMaximumAllowedSize();
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


}
