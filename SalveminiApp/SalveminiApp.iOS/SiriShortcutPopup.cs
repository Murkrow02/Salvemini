using System;
using UIKit;
namespace SalveminiApp.iOS
{
    public class SiriShortcutPopup : UIViewController
    {
        
        public SiriShortcutPopup()
        {
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            //var a = new ModalViewController
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            DefinesPresentationContext = true;

            var view = new UIView { BackgroundColor = UIColor.White };
            view.Frame = new CoreGraphics.CGRect { X = View.Frame.Width / 2, Y = View.Frame.Height / 2, Width = 300, Height = 300 };
            view.Center = View.Center;


            
            var blurEffectView = new UIView();
            blurEffectView.BackgroundColor = UIColor.Black;
            blurEffectView.Alpha = 0.6f;
            blurEffectView.Frame = View.Frame;
            View.InsertSubview(blurEffectView, 0);

            View.AddSubview(view);

        }
    }
}
