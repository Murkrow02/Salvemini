using System;
using System.Collections.Generic;
using System.Linq;
using Com.OneSignal;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using Foundation;
using Intents;
using Lottie.Forms.iOS.Renderers;
using Plugin.Segmented.Control.iOS;
using Syncfusion.SfCarousel.XForms.iOS;
using UIKit;
using UserNotifications;
using Plugin.Toasts;
using Xamarin.Forms;
using PanCardView.iOS;
using System.Threading.Tasks;
using IntentsUI;
//using Google.MobileAds;

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
            //Init Plugins
            Rg.Plugins.Popup.Popup.Init();
            new Syncfusion.SfAutoComplete.XForms.iOS.SfAutoCompleteRenderer();

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

            //User defaults playground
            var defaults = new NSUserDefaults("group.com.codex.SalveminiApp");
            defaults.SetValueForKey(new NSString("asd"), new NSString("pedo"));

            var asd = defaults.StringForKey(new NSString("pedo"));

            //Initialize Processes
            DependencyService.Register<ToastNotification>();
            ToastNotification.Init();
            CachedImageRenderer.Init();
            Forms9Patch.iOS.Settings.Initialize(this);
            var ignore = typeof(SvgCachedImage);
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();
            Syncfusion.XForms.iOS.TextInputLayout.SfTextInputLayoutRenderer.Init();
            Syncfusion.XForms.iOS.PopupLayout.SfPopupLayoutRenderer.Init();
            Syncfusion.XForms.iOS.BadgeView.SfBadgeViewRenderer.Init();
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();
            Syncfusion.XForms.iOS.Expander.SfExpanderRenderer.Init();
            SegmentedControlRenderer.Initialize();
            CardsViewRenderer.Preserve();
            AnimationViewRenderer.Init();
            new SfCarouselRenderer();


            //Initialize ads
            //MobileAds.SharedInstance.Start(CompletionHandler);


            LoadApplication(new App());

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Request Permissions
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>
                {
                    // Do something if needed
                });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                 UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                    );

                app.RegisterUserNotificationSettings(notificationSettings);
            }


         


            return base.FinishedLaunching(app, options);
        }

        //Ads stuff
        //private void CompletionHandler(InitializationStatus status) { }



        //public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        //{
        //    var intent = userActivity.GetInteraction()?.Intent as TrainIntent;
        //    if (!(intent is null))
        //    {
        //        HandleIntent(intent);
        //        return true;
        //    }
        //    else if (userActivity.ActivityType == TrainKit.Support.NSUserActivityHelper.ViewMenuActivityType)
        //    {
        //        HandleUserActivity();
        //        return true;
        //    }
        //    return false;
        //}



        public static async void hapticVibration()
        {
            Device.BeginInvokeOnMainThread(async() =>
            {
                var impact = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
                impact.Prepare();
                impact.ImpactOccurred();
                await Task.Delay(200);
                impact.ImpactOccurred();
            });
            
        }
    }
}
