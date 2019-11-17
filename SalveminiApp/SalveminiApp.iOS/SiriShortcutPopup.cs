using System;
using Intents;
using IntentsUI;
using UIKit;
using Xamarin.Forms.Platform.iOS;

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


        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            UIView.Animate(0.3, () =>
            {
                var frame = View.Frame;
                var yComponent = UIScreen.MainScreen.Bounds.Height - 300;
                View.Frame = new CoreGraphics.CGRect(0, yComponent, frame.Width, frame.Height);
            });
        }




        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Layer.CornerRadius = 20;

            var layout = new UIStackView();
            layout.AddArrangedSubview(new UIView());

            var label = new UILabel { Text = "Comandi di Siri", TextColor = SalveminiApp.Styles.TextColor.ToUIColor(), Font = UIFont.BoldSystemFontOfSize(30), TextAlignment = UITextAlignment.Center };
            layout.AddArrangedSubview(label);

            var type = new UILabel { Text = tipo, TextColor = SalveminiApp.Styles.TextGray.ToUIColor(), Font = UIFont.SystemFontOfSize(20), TextAlignment = UITextAlignment.Center };
            layout.AddArrangedSubview(type);

            //Create siri button
            var siriButton = new INUIAddVoiceShortcutButton(INUIAddVoiceShortcutButtonStyle.WhiteOutline);
            siriButton.Shortcut = shortcut;

            //Connect actions
            siriButton.Delegate = new SalveminiApp.iOS.AddVoiceShortcutButton(tipo); //Passa ultimi 2 valori solo se deve aggiungere shortcut del treno

            layout.AddArrangedSubview(siriButton);

            layout.Axis = UILayoutConstraintAxis.Vertical;
            layout.Spacing = 20;
            layout.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(layout);

            label.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            label.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            type.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            siriButton.LeftAnchor.ConstraintEqualTo(layout.LeftAnchor, 80).Active = true;
            siriButton.RightAnchor.ConstraintEqualTo(layout.RightAnchor, -80).Active = true;

            layout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            layout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;


           

        }
    }
}
