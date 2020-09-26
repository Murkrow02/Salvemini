﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using DLToolkit.Forms.Controls;
using MonkeyCache.SQLite;
using Plugin.Toasts;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Globalization;
namespace SalveminiApp
{
    public partial class App : Xamarin.Forms.Application
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
        public static RestApi.SondaggiManager Sondaggi { get; private set; }
        public static RestApi.AdsManager Ads { get; private set; }
        public static RestApi.AgendaManager Agenda { get; private set; }
        public static RestApi.CringeManager Cringe { get; private set; }
        public static RestApi.ItemManagerCoins Coins { get; private set; }
        public static RestApi.ItemManagerFlappy Flappy { get; private set; }

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTMwMTY3QDMxMzcyZTMyMmUzMEVkVmVDYmhQM1JMS1g5alVuZSs1SVlVbGEvdktLZVc3a1l1MU5DNGpVTzQ9");

            InitializeComponent();

            //Initialize FlowListView
            FlowListView.Init();

            //Initialize Iconize
            Plugin.Iconize.Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeProBrandsModule()).With(new Plugin.Iconize.Fonts.FontAwesomeProLightModule()).With(new Plugin.Iconize.Fonts.FontAwesomeProRegularModule()).With(new Plugin.Iconize.Fonts.FontAwesomeProSolidModule()).With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule()).With(new Plugin.Iconize.Fonts.FontAwesomeProBrandsModule());

            //Cache Folder
            Barrel.ApplicationId = "com.codex.salveminiapp";

            //Remove expired cache
            Barrel.Current.EmptyExpired();


            //Register OneSignal License
            OneSignal.Current.StartInit("a85553ca-c1fe-4d93-a02f-d30bf30e2a2a").InFocusDisplaying(OSInFocusDisplayOption.None)
                .HandleNotificationReceived(HandleNotificationReceived)
                .HandleNotificationOpened(HandleNotificationOpened)
                .EndInit();

            //Fix if upgraded from salveminiapp 2.0
            if (Preferences.Get("veryFirstTime", true)) { Preferences.Clear(); Preferences.Set("isFirstTime", true); Preferences.Set("veryFirstTime", false); }


            var stopwatch = new Stopwatch(); stopwatch.Start();
            //Set MainPage
            if (Preferences.Get("isFirstTime", true)) //First time
            {
                MainPage = new Xamarin.Forms.NavigationPage(new FirstAccess.WelcomePage());
            }
            else if (Preferences.Get("Token", "") == "" || Preferences.Get("UserId", 0) == 0) //Not logged
            {
                MainPage = new FirstAccess.Login();
            }
            else //Logged
            {
                MainPage = new TabPage();
            }

            //var navigationPage = new NavigationPage(new iCringe.Home());
            //navigationPage.BackgroundColor = Styles.SecretsPrimary;
            //MainPage = navigationPage;

            stopwatch.Stop();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);

            //Set italian as default culture
            CultureInfo.CurrentCulture = new CultureInfo("it-IT");

            //Reload Calls
            refreshCalls();
        }

        public static async Task refreshCalls()
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
            Sondaggi = new RestApi.SondaggiManager(new RestApi.RestServiceSondaggi());
            Ads = new RestApi.AdsManager(new RestApi.RestServiceAds());
            Agenda = new RestApi.AgendaManager(new RestApi.RestServiceAgenda());
            Coins = new RestApi.ItemManagerCoins(new RestApi.RestServiceCoins());
            Cringe = new RestApi.CringeManager(new RestApi.RestServiceCringe());
            Flappy = new RestApi.ItemManagerFlappy(new RestApi.RestServiceFlappy());
        }

        private static async void HandleNotificationReceived(OSNotification notification)
        {
            try
            {

                OSNotificationPayload payload = notification.payload;
                //  Dictionary<string, object> additionalData = payload.additionalData;
                string message = payload.body;
                //Route from push to inApp notification
                var notificator = DependencyService.Get<IToastNotificator>();
                var options = new NotificationOptions()
                {
                    Description = message.Replace(", apri l'app per saperne di più!", "").Replace(", apri l'app per votare", "")
                };

                var result = await notificator.Notify(options);
            }
            catch
            {
                Debug.WriteLine("Error handling push notification");
            }
        }

        private static async void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            try
            {
                //Get payload
                OSNotificationPayload payload = result.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;

                //No push info
                if (additionalData == null)
                    return;

                string pushValue = "";
                if (additionalData.ContainsKey("tipo") && additionalData.ContainsKey("id"))
                {
                    if (additionalData["tipo"].ToString() == "push")
                        pushValue = additionalData["id"].ToString();
                }

                //string pushPage = "";
                //if (additionalData != null)
                //{
                //    if (additionalData.ContainsKey("id"))
                //    {
                //        idValue = additionalData["id"].ToString());
                //    }
                //}
                switch (pushValue)
                {
                    case "iCringe":
                        Xamarin.Forms.Application.Current.MainPage = new TabPage(0);
                        break;
                    case "Avvisi":
                        Xamarin.Forms.Application.Current.MainPage = new TabPage(1);
                        var avvisiPage = new SecondaryViews.Avvisi();
                        SalveminiApp.MainPage.NotificationPage = avvisiPage;
                        break;
                }
            }
            catch { }
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