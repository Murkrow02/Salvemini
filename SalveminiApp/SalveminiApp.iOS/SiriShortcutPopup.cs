using System;
using Intents;
using IntentsUI;
using UIKit;
namespace SalveminiApp.iOS
{
    public class SiriShortcutPopup : UIViewController
    {
        public SiriShortcutPopup()
        {
            ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
            View.BackgroundColor = UIColor.White;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var intent = new TrenoIntent();
            intent.SuggestedInvocationPhrase = "Prossimo treno"; // da " + Costants.Stazioni[station] + " ";
            INShortcut trainShortcut = new INShortcut(intent);

            //Create siri button
            var siriButton = new INUIAddVoiceShortcutButton(INUIAddVoiceShortcutButtonStyle.WhiteOutline);
            siriButton.Shortcut = trainShortcut;

            //Connect actions
            siriButton.Delegate = new SalveminiApp.iOS.AddVoiceShortcutButton( 2, true); //Passa ultimi 2 valori solo se deve aggiungere shortcut del treno
            View.AddSubview(siriButton);
            //Constraints
            siriButton.TranslatesAutoresizingMaskIntoConstraints = false;
            View.CenterXAnchor.ConstraintEqualTo(siriButton.CenterXAnchor).Active = true;
            View.CenterYAnchor.ConstraintEqualTo(siriButton.CenterYAnchor).Active = true;
        }
    }
}
