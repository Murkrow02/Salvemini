using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.OneSignal;
using Plugin.CurrentActivity;
using Plugin.Toasts;
using PanCardView.Droid;
using Acr.UserDialogs;
using Xamarin.Forms;
using System.Diagnostics;
using Plugin.Permissions;
using Stormlion.PhotoBrowser.Droid;

namespace SalveminiApp.Droid
{
    [Activity(Label = "Salvemini", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            // Name of the MainActivity theme you had there before.
            // Or you can use global::Android.Resource.Style.ThemeHoloLight
            base.SetTheme(SalveminiApp.Droid.Resource.Style.MainTheme);

            //Get Screen Size
            var metrics = Resources.DisplayMetrics;
            App.ScreenHeight = ConvertPixelsToDp(metrics.HeightPixels);
            App.ScreenWidth = ConvertPixelsToDp(metrics.WidthPixels);

            TabLayoutResource = SalveminiApp.Droid.Resource.Layout.Tabbar;
            ToolbarResource = SalveminiApp.Droid.Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //Register OneSignal License
            OneSignal.Current.StartInit("a85553ca-c1fe-4d93-a02f-d30bf30e2a2a").EndInit();

            Xamarin.Forms.Forms.SetFlags(new string[] { "Shapes_Experimental" });



            //Init Popups
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //Initialize Components
            DependencyService.Register<ToastNotification>();
            ToastNotification.Init(this, new PlatformOptions() { SmallIconDrawable = Android.Resource.Drawable.IcDialogInfo });
            //Plugin.Iconize.Iconize.Init(Android.Resource.Id.toolbar, Resource.Id.sliding_tabs);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            DependencyService.Register<ToastNotification>(); //Register your dependency
            ToastNotification.Init(this);
            UserDialogs.Init(this);
            Forms9Patch.Droid.Settings.Initialize(this);
            CardsViewRenderer.Preserve();
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Stormlion.PhotoBrowser.Droid.Platform.Init(this);

            // Register the notification dependency.
            //DependencyService.Register<NotificationScheduler>();
            DependencyService.Register<PhotoBrowserImplementation>();

            LoadApplication(new App());
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);


            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

