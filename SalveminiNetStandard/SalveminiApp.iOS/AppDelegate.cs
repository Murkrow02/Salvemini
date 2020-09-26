using System;
using System.Collections.Generic;
using System.Linq;
using Com.OneSignal;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using Foundation;
using Intents;
using Plugin.Segmented.Control.iOS;
using UIKit;
using UserNotifications;
using Plugin.Toasts;
using Xamarin.Forms;
using PanCardView.iOS;
using System.Threading.Tasks;
using IntentsUI;
using Google.MobileAds;

namespace SalveminiApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static bool HasNotch = false;


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //Init Plugins
            Rg.Plugins.Popup.Popup.Init();
            new Syncfusion.XForms.iOS.ComboBox.SfComboBoxRenderer();

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
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();
            Syncfusion.XForms.iOS.Expander.SfExpanderRenderer.Init();
            SegmentedControlRenderer.Initialize();
            CardsViewRenderer.Preserve();
            //AnimationViewRenderer.Init();
            DependencyService.Register<PhotoBrowserImplementation>();
            // Register the notification dependency.  Don't forget to do this.
            //DependencyService.Register<NotificationScheduler>();

            //Initialize ads
            MobileAds.SharedInstance.Start(CompletionHandler);


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
		private void CompletionHandler(InitializationStatus status) { }



        //public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        //{
        //    try
        //    {
        //        var intent = userActivity.GetInteraction()?.Intent;
        //        if (intent.GetType() == new TrenoIntent().GetType())
        //        {
        //            Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(new SecondaryViews.Trasporti.Treni());
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return false;
        //}



        public static async void hapticVibration()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var impact = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
                impact.Prepare();
                impact.ImpactOccurred();
                await Task.Delay(200);
                impact.ImpactOccurred();
            });

        }

		// The following Exports are needed to run OneSignal in the iOS Simulator.
		//   The simulator doesn't support push however this prevents a crash due to a Xamarin bug
		[Export("oneSignalApplicationDidBecomeActive:")]
		public void OneSignalApplicationDidBecomeActive(UIApplication application)
		{
			// Remove line if you don't have a OnActivated method.
			OnActivated(application);
		}

		[Export("oneSignalApplicationWillResignActive:")]
		public void OneSignalApplicationWillResignActive(UIApplication application)
		{
			// Remove line if you don't have a OnResignActivation method.
			OnResignActivation(application);
		}

		[Export("oneSignalApplicationDidEnterBackground:")]
		public void OneSignalApplicationDidEnterBackground(UIApplication application)
		{
			// Remove line if you don't have a DidEnterBackground method.
			DidEnterBackground(application);
		}

		[Export("oneSignalApplicationWillTerminate:")]
		public void OneSignalApplicationWillTerminate(UIApplication application)
		{
			// Remove line if you don't have a WillTerminate method.
			WillTerminate(application);
		}

		// Note: Similar exports are needed if you add other AppDelegate overrides.
	}
}
