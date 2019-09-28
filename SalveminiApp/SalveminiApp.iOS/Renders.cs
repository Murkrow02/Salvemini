using System;
using SalveminiApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using CoreAnimation;
using Foundation;

[assembly: ExportRenderer(typeof(SalveminiApp.Helpers.CustomNavigationPage), typeof(CustomNavigationPage))]
[assembly: ExportRenderer(typeof(SalveminiApp.ShadowFrame), typeof(ShadowFrame))]
[assembly: ExportRenderer(typeof(SalveminiApp.TransparentGradient), typeof(TransparentGradient))]
[assembly: ExportRenderer(typeof(SalveminiApp.DashedBorderFrame), typeof(DashedBorderFrame))]

namespace SalveminiApp.iOS
{

    public class DashedBorderFrame : FrameRenderer
    {
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CAShapeLayer viewBorder = new CAShapeLayer();
            viewBorder.StrokeColor = Color.FromHex("#BCBCBC").ToCGColor();
            viewBorder.FillColor = null;
            viewBorder.LineDashPattern = new NSNumber[] { new NSNumber(10), new NSNumber(10) };
            viewBorder.Frame = NativeView.Bounds;
            viewBorder.Path = UIBezierPath.FromRect(NativeView.Bounds).CGPath;
            Layer.AddSublayer(viewBorder);

            // If you don't want the shadow effect
            Element.HasShadow = false;
        }
    }
    
    public class CustomNavigationPage : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var att = new UITextAttributes();
                att.Font = UIFont.FromName("Monserrat-Medium.otf", 24);

                UINavigationBar.Appearance.SetTitleTextAttributes(att);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //NavigationBar.ShadowImage = new UIImage();
            NavigationBar.Translucent = false;
           
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }

    public class ShadowFrame : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            Layer.ShadowOffset = new CGSize(new CGPoint(0, 8));
            Layer.ShadowOpacity = 0.15f;
            Layer.ShadowRadius = 10f;

        }
    }

    public class TransparentGradient : FrameRenderer
    {
        public override void Draw(CGRect rect)
        {
            var gl = new CAGradientLayer
            {
                StartPoint = new CGPoint(0, 0.5),
                EndPoint = new CGPoint(1, 0.5),
                Frame = rect,
                Colors = new CGColor[]
                {
                    Color.White.ToCGColor(),
                    Color.Transparent.ToCGColor()
                },
                CornerRadius = 0f
            };

            NativeView.Layer.BackgroundColor = UIColor.Clear.CGColor;
            NativeView.Layer.InsertSublayer(gl, 0);
            base.Draw(rect);
        }
    }


}
