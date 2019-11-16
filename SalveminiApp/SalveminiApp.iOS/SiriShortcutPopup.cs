using System;
using Intents;
using IntentsUI;
using UIKit;
namespace SalveminiApp.iOS
{
    public class SiriShortcutPopup : UIViewController
    {
        INShortcut shortcut;
        string tipo;

        public SiriShortcutPopup(INShortcut shortcut_, string tipo_)
        {
            shortcut = shortcut_;
            tipo = tipo_;
            ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
            View.BackgroundColor = UIColor.White;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();



            //Create siri button
            var siriButton = new INUIAddVoiceShortcutButton(INUIAddVoiceShortcutButtonStyle.WhiteOutline);
            siriButton.Shortcut = shortcut;

            //Connect actions
            siriButton.Delegate = new SalveminiApp.iOS.AddVoiceShortcutButton(tipo); //Passa ultimi 2 valori solo se deve aggiungere shortcut del treno
            View.AddSubview(siriButton);

            //Constraints
            siriButton.TranslatesAutoresizingMaskIntoConstraints = false;
            View.CenterXAnchor.ConstraintEqualTo(siriButton.CenterXAnchor).Active = true;
            View.CenterYAnchor.ConstraintEqualTo(siriButton.CenterYAnchor).Active = true;
        }
    }
}
