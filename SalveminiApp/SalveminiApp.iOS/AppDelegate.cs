using System;
using System.Collections.Generic;
using System.Linq;
using Com.OneSignal;
using Foundation;
using Lottie.Forms.iOS.Renderers;
using UIKit;

namespace SalveminiApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public static bool HasNotch = false;


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            //Get Screen Size
            App.ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;

            //Register OneSignal License
            OneSignal.Current.StartInit("a85553ca-c1fe-4d93-a02f-d30bf30e2a2a").EndInit();

            //DETECT NOTCH
            List<float> homeBarDevices = new List<float> { 370944, 304500 };
            try
            {
                var deviceRes = (float)(App.ScreenWidth * App.ScreenHeight);
                if (homeBarDevices.Contains(deviceRes))
                {
                    HasNotch = true;
                }
                else
                {
                    HasNotch = false;
                }

            }
            catch
            {
                HasNotch = false;
                Console.WriteLine("Error homebar");
            }

            //Initialize Processes
            Syncfusion.XForms.iOS.TextInputLayout.SfTextInputLayoutRenderer.Init();
            Syncfusion.XForms.iOS.PopupLayout.SfPopupLayoutRenderer.Init();
            AnimationViewRenderer.Init();

            LoadApplication(new App());

           
            return base.FinishedLaunching(app, options);
        }
    }
}
