using System;
using Intents;
using IntentsUI;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms.Platform.iOS;

namespace SalveminiApp.iOS
{
    public class SiriShortcutPopup : UIViewController
    {
        INShortcut shortcut;
        string tipo;
        bool fromSettings;

        public SiriShortcutPopup(INShortcut shortcut_, string tipo_, bool fromSettings_ = true)
        {
            shortcut = shortcut_;
            tipo = tipo_;
            fromSettings = fromSettings_;
            ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
            View.BackgroundColor = UIColor.White;
            //ParentViewController.View.BackgroundColor = UIColor.Red;
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

            var dontshowButton = new UIButton(UIButtonType.System) { TintColor = new UIColor(red: 0.52f, green: 0.52f, blue: 0.52f, alpha: 1.0f) };
            dontshowButton.SetTitle("Non mostrare più", UIControlState.Normal);
            dontshowButton.TouchUpInside += DontshowButton_Clicked;

            if (!fromSettings)
            {
                layout.AddArrangedSubview(dontshowButton);
            }

            layout.Axis = UILayoutConstraintAxis.Vertical;
            layout.Spacing = 20;
            layout.TranslatesAutoresizingMaskIntoConstraints = false;
            var closeButton = new UIButton(new CoreGraphics.CGRect(View.Bounds.X + 5, View.Bounds.Y + 5, 30, 30));
            closeButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
            closeButton.SetImage(new UIImage("close_image"), UIControlState.Normal);
            closeButton.TouchUpInside += Close_Clicked;

            layout.Add(closeButton);

            View.AddSubview(layout);

            label.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            label.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            type.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            siriButton.LeftAnchor.ConstraintEqualTo(layout.LeftAnchor, 80).Active = true;
            siriButton.RightAnchor.ConstraintEqualTo(layout.RightAnchor, -80).Active = true;

            layout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            layout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;


        }

        private void DontshowButton_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("DontShowSiriWidget", true);
            DismissModalViewController(true);
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            DismissModalViewController(true);
        }
    }
}
