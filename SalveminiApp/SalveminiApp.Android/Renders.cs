using System;
using Android.Content;
using SalveminiApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SalveminiApp.GradientFrame), typeof(GradientFrame))]

namespace SalveminiApp.Droid
{
    public class GradientFrame : VisualElementRenderer<Frame>
    {
        public GradientFrame(Context context) : base(context)
        {
        }

        private Color StartColor
        {
            get;
            set;
        }
        private Color EndColor
        {
            get;
            set;
        }
        protected override void DispatchDraw(global::Android.Graphics.Canvas canvas)
        {
            #region for Vertical Gradient  
            //var gradient = new Android.Graphics.LinearGradient(0, 0, 0, Height,      
            #endregion
            #region  for Horizontal Gradient  
            var gradient = new Android.Graphics.LinearGradient(0, 0, Width, 0,
            #endregion
            this.StartColor.ToAndroid(),
            this.EndColor.ToAndroid(),
            Android.Graphics.Shader.TileMode.Mirror);
            var paint = new Android.Graphics.Paint()
            {
                Dither = true,
            };
            paint.SetShader(gradient);
            canvas.DrawPaint(paint);
            base.DispatchDraw(canvas);
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }
            try
            {
                var stack = e.NewElement as SalveminiApp.GradientFrame;
                this.StartColor = stack.StartColor;
                this.EndColor = stack.EndColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR:", ex.Message);
            }
        }
    }
}
