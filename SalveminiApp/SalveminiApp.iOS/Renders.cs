using System;
using SalveminiApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using CoreAnimation;

[assembly: ExportRenderer(typeof(SalveminiApp.Helpers.CustomNavigationPage), typeof(CustomNavigationPage))]
[assembly: ExportRenderer(typeof(SalveminiApp.ShadowFrame), typeof(ShadowFrame))]
[assembly: ExportRenderer(typeof(SalveminiApp.TransparentGradient), typeof(TransparentGradient))]
[assembly: ExportRenderer(typeof(SalveminiApp.GradientFrame), typeof(GradientFrame))]

namespace SalveminiApp.iOS
{
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
            NavigationBar.ShadowImage = new UIImage();
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

    public class GradientFrame : FrameRenderer
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            SalveminiApp.GradientFrame stack = (SalveminiApp.GradientFrame)this.Element;
            CGColor startColor = stack.StartColor.ToCGColor();
            CGColor endColor = stack.EndColor.ToCGColor();
            #region for Vertical Gradient  
            //var gradientLayer = new CAGradientLayer();     
            #endregion
            #region for Horizontal Gradient  
            var gradientLayer = new CAGradientLayer()
            {
                StartPoint = new CGPoint(0, 0.5),
                EndPoint = new CGPoint(1, 0.5)
            };
            #endregion
            gradientLayer.Frame = rect;
            gradientLayer.Colors = new CGColor[] {
                startColor,
                endColor
            };
            NativeView.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}
