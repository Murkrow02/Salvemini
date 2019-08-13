using System;
using Com.OneSignal;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SalveminiApp
{
    public partial class App : Application
    {
        //Screen Size
        public static double ScreenWidth = 0;
        public static double ScreenHeight = 0;

        //RestService Call
        public static RestApi.ItemManagerLogin Login { get; private set; }

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTMwMTY3QDMxMzcyZTMyMmUzMEVkVmVDYmhQM1JMS1g5alVuZSs1SVlVbGEvdktLZVc3a1l1MU5DNGpVTzQ9");

            InitializeComponent();

            //Set MainPage
            if (Preferences.Get("isFirstTime", true))
            {
                MainPage = new NavigationPage(new FirstAccess.WelcomePage());
            }
            else
            {
                MainPage = new TabPage();
            }

            //Register OneSignal License
            OneSignal.Current.StartInit("a85553ca-c1fe-4d93-a02f-d30bf30e2a2a").EndInit();

            //Initialize Iconize
            Plugin.Iconize.Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule()).With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule()).With(new Plugin.Iconize.Fonts.FontAwesomeSolidModule());

            //Reload Calls
            refreshCalls();
        }

        public static void refreshCalls()
        {
            Login = new RestApi.ItemManagerLogin(new RestApi.RestServiceLogin());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
