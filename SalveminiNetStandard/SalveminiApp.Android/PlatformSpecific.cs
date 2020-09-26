using System;
using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using Android.Widget;
using Plugin.CurrentActivity;
using Plugin.Iconize;
using SalveminiApp.Droid;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecific))]
namespace SalveminiApp.Droid
{
    public class PlatformSpecific : SalveminiApp.IPlatformSpecific
    {
        public PlatformSpecific() { }

        public void SendToast(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();

            try
            {
                Vibration.Vibrate();

                var duration = TimeSpan.FromMilliseconds(200);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                Console.WriteLine("Not supported");
                //Costants.SendCrash(ex, "PlatformSpecific.cs", "SendToast");
            }
        }

        //Detect homebar
        public bool HasBottomBar()
        {
            return false;
        }

        //Open device settings
        //public void OpenSettings()
        //{
        //    Android.App.Application.Context.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings));
        //}

        ////Keep focus on keyboard dismiss
        //public void CanUnfocus(bool status)
        //{
        //    Ulysse.SecondaryViews.TrainerChat.canUnfocus = status;
        //}

        ////Disimiss keyboard
        //public void HideKeyboard()
        //{
        //    try
        //    {
        //        Ulysse.SecondaryViews.TrainerChat.canUnfocus = false;
        //        InputMethodManager imm = InputMethodManager.FromContext(CrossCurrentActivity.Current.Activity.ApplicationContext);
        //        imm.HideSoftInputFromWindow(
        //            CrossCurrentActivity.Current.Activity.Window.DecorView.WindowToken, HideSoftInputFlags.NotAlways);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Costants.SendCrash(ex, "PlatformSpecific.cs", "HideKeyboard");
        //    }
        //}

        ////Blur the element
        //public void BlurLayout(Xamarin.Forms.StackLayout view) { }

        //solo ios :/
        public void SetSafeArea(ContentPage page)
        {
            //Lol ti pare
        }
        //public void SetFormSheet(Page page)
        //{
        //    //Lol ti pare
        //}
        //public float GetBottomSafeAreInset()
        //{
        //    //Lol ti pare
        //    return 0;
        //}
        //public void AnimateKeyboard(Xamarin.Forms.Frame frame, Syncfusion.ListView.XForms.SfListView list) { }
        //public void SetTabBar(IconTabbedPage tabpage)
        //{
        //    tabpage.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        //    tabpage.On<Xamarin.Forms.PlatformConfiguration.Android>().DisableSwipePaging();
        //}

        //public void AnimateKeyboard(Frame frame, SfListView list, Frame topFrame)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
