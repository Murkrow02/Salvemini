using System;
using CoreGraphics;
using Foundation;
using Intents;
using IntentsUI;
using TrainKit.Data;
using UIKit;
using SalveminiApp;
using CoreFoundation;

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
            

            var NextTrain = await Treni.GetNextTrain(0, true);

            if (NextTrain != null)
            {
                var a = new NSMutableAttributedString();
                a.Append(new NSAttributedString("Il prossimo treno per ", new UIStringAttributes { Font = UIFont.SystemFontOfSize(15) }));
                a.Append(new NSAttributedString("Napoli", new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(15) }));
                a.Append(new NSAttributedString(" parte alle ", new UIStringAttributes { Font = UIFont.SystemFontOfSize(15) }));
                a.Append(new NSAttributedString(NextTrain.Partenza, new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(15) }));
                a.Append(new NSAttributedString(" da ", new UIStringAttributes { Font = UIFont.SystemFontOfSize(15) }));
                a.Append(new NSAttributedString("Sorrento", new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(15) }));

                osgu.AttributedText = a;
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
    }
}
