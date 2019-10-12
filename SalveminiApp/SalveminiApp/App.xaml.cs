using System;
using System.IO;
using Com.OneSignal;
using DLToolkit.Forms.Controls;
using MonkeyCache.SQLite;
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
        public static RestApi.ItemManagerOrari Orari { get; private set; }
        public static RestApi.ItemManagerIndex Index { get; private set; }
        public static RestApi.ItemManagerArgo Argo { get; private set; }
        public static RestApi.ItemManagerTreni Treni { get; private set; }
        public static RestApi.AvvisiManager Avvisi { get; private set; }
        public static RestApi.ImageManager Immagini { get; private set; }
        public static RestApi.AnalytiscManager Analytics { get; private set; }
        public static RestApi.ItemManagerUtenti Utenti { get; private set; }
        public static RestApi.ItemManagerCard Card { get; private set; }

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTMwMTY3QDMxMzcyZTMyMmUzMEVkVmVDYmhQM1JMS1g5alVuZSs1SVlVbGEvdktLZVc3a1l1MU5DNGpVTzQ9");

            InitializeComponent();

            //Initialize FlowListView
            FlowListView.Init();

            //Cache Folder
            Barrel.ApplicationId = "com.codex.salveminiapp";

            //Set MainPage
            if (Preferences.Get("isFirstTime", true))
            {
                MainPage = new NavigationPage(new FirstAccess.WelcomePage());
            }
            else
            {
                MainPage = new TabPage();
            }

            //MainPage = new NavigationPage(new AreaVip.VipMenu());

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
            Orari = new RestApi.ItemManagerOrari(new RestApi.RestServiceOrari());
            Index = new RestApi.ItemManagerIndex(new RestApi.RestServiceIndex());
            Argo = new RestApi.ItemManagerArgo(new RestApi.RestServiceArgo());
            Treni = new RestApi.ItemManagerTreni(new RestApi.RestServiceTreni());
            Avvisi = new RestApi.AvvisiManager(new RestApi.RestServiceAvvisi());
            Immagini = new RestApi.ImageManager(new RestApi.RestServiceImmagini());
            Analytics = new RestApi.AnalytiscManager(new RestApi.RestServiceAnalytics());
            Utenti = new RestApi.ItemManagerUtenti(new RestApi.RestServiceUtenti());
            Card = new RestApi.ItemManagerCard(new RestApi.RestServiceCard());

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
