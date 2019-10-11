using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.OneSignal;
using Lottie.Forms.Droid;
using SegmentedControl.FormsPlugin.Android;
using Plugin.CurrentActivity;

namespace SalveminiApp.Droid
{
    [Activity(Label = "SalveminiApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Get Screen Size
            var metrics = Resources.DisplayMetrics;
            App.ScreenHeight = ConvertPixelsToDp(metrics.HeightPixels);
            App.ScreenWidth = ConvertPixelsToDp(metrics.WidthPixels);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //Register OneSignal License
            OneSignal.Current.StartInit("a85553ca-c1fe-4d93-a02f-d30bf30e2a2a").EndInit();

            //Init Popups
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //Initialize Components
            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();
            Plugin.Iconize.Iconize.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);
            UserDialogs.Init(this);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            AnimationViewRenderer.Init();

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

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}