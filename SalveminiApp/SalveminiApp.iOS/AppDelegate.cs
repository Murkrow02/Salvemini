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
            //Init Popups
            Rg.Plugins.Popup.Popup.Init();
            //Forms.SetFlags("CollectionView_Experimental");
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
            SegmentedControlRenderer.Initialize();
            CardsViewRenderer.Preserve();
            AnimationViewRenderer.Init();
            new SfCarouselRenderer();
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

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            var intent = userActivity.GetInteraction()?.Intent as TrainIntent;
            if (!(intent is null))
            {
                HandleIntent(intent);
                return true;
            }
            else if (userActivity.ActivityType == TrainKit.Support.NSUserActivityHelper.ViewMenuActivityType)
            {
                HandleUserActivity();
                return true;
            }
            return false;
        }

        


        void HandleIntent(TrainIntent intent)
        {
            var handler = new TrainKit.TrainIntentHandler();
            handler.HandleTrain(intent, (response) => {
                if (response.Code != TrainIntentResponseCode.Success)
                {
                    Console.WriteLine("Quantity must be greater than 0 to add to order");
                }
            });
        }

        void HandleUserActivity()
        {
            var rootViewController = Window?.RootViewController as UINavigationController;
            //var orderHistoryViewController = rootViewController?.ViewControllers?.FirstOrDefault() as OrderHistoryTableViewController;
            //if (orderHistoryViewController is null)
            //{
            //    Console.WriteLine("Failed to access OrderHistoryTableViewController.");
            //    return;
            //}
            //var segue = OrderHistoryTableViewController.SegueIdentifiers.SoupMenu;
            //orderHistoryViewController.PerformSegue(segue, null);
        }
    }
}
