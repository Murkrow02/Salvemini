using System;
using SalveminiApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using System.Drawing;

[assembly: ExportRenderer(typeof(SalveminiApp.Helpers.CustomNavigationPage), typeof(CustomNavigationPage))]
[assembly: ExportRenderer(typeof(SalveminiApp.ShadowFrame), typeof(ShadowFrame))]
[assembly: ExportRenderer(typeof(SalveminiApp.TransparentGradient), typeof(TransparentGradient))]
[assembly: ExportRenderer(typeof(SalveminiApp.DashedBorderFrame), typeof(DashedBorderFrame))]
[assembly: ExportRenderer(typeof(SalveminiApp.ChatEntry), typeof(ChatEntryViewRenderer))]
[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]
[assembly: ExportRenderer(typeof(WebView), typeof(CustomWebViewRenderer))]

namespace SalveminiApp.iOS
{
    public class ChatEntryViewRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            Control.InputAccessoryView = null;
        }
    }

    public static class ImageExtensions
    {
        public static UIImage Crop(this UIImage image, int x, int y, int width, int height)
        {
            var imgSize = image.Size;

            UIGraphics.BeginImageContext(new SizeF(width, height));
            var imgToCrop = UIGraphics.GetCurrentContext();

            var croppingRectangle = new CGRect(0, 0, width, height);
            imgToCrop.ClipToRect(croppingRectangle);

            var drawRectangle = new CGRect(-x, -y, imgSize.Width, imgSize.Height);

            image.Draw(drawRectangle);
            var croppedImg = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();
            return croppedImg;
        }
    }

    public class CustomWebViewRenderer : WebViewRenderer
    {

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            var view = (UIWebView)NativeView;
            view.ScrollView.ScrollEnabled = true;
            view.ScalesPageToFit = true;
            
        }

    }

    public class DashedBorderFrame : FrameRenderer
    {
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CAShapeLayer viewBorder = new CAShapeLayer();
            viewBorder.StrokeColor = Xamarin.Forms.Color.FromHex("#BCBCBC").ToCGColor();
            viewBorder.FillColor = null;
            viewBorder.LineDashPattern = new NSNumber[] { new NSNumber(10), new NSNumber(10) };
            viewBorder.Frame = NativeView.Bounds;
            viewBorder.Path = UIBezierPath.FromRect(NativeView.Bounds).CGPath;
            Layer.AddSublayer(viewBorder);

            // If you don't want the shadow effect
            Element.HasShadow = false;
        }
    }

    public class TabbedPageRenderer : TabbedRenderer
    {
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                TabBar.BarStyle = UIBarStyle.Black;
            }
            else
            {
                TabBar.ShadowImage = new UIImage();
            }

            TabBar.UnselectedItemTintColor = UIColor.Black;
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CropBg("bbar.jpg");
            MessagingCenter.Subscribe<App, string>(this, "changeBg", (sender, image) =>
            {
                CropBg(image);
            });
        }

        public void CropBg(string image_)
        {
            var image = UIImage.FromBundle(image_);
            image = image.Crop(0,
                (int)(image.CGImage.Height - TabBar.Frame.Height),
                (int)image.CGImage.Width,
                (int)TabBar.Frame.Height);
            image = image.Scale(new CGSize(TabBar.Frame.Width, TabBar.Frame.Height));
            TabBar.BackgroundImage = image;
        }
    }

    public class CustomNavigationPage : NavigationRenderer
    {
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
                    Xamarin.Forms.Color.White.ToCGColor(),
                    Xamarin.Forms.Color.Transparent.ToCGColor()
                },
                CornerRadius = 0f
            };

            NativeView.Layer.BackgroundColor = UIColor.Clear.CGColor;
            NativeView.Layer.InsertSublayer(gl, 0);
            base.Draw(rect);
        }
    }


}
