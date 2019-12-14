using Android.App;
using Android.OS;
using Android.Support.V7.App;
using SalveminiApp.Droid;
using System.Threading;

namespace SalveminiApp
{
    [Activity(Label = "SalveminiApp", Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }
}