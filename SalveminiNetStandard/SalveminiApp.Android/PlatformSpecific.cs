using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using Android.Widget;
using Java.IO;
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

        public async Task SavePictureToDisk(string filename, byte[] imageData)  
        {
            //Check permission
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
               await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Attenzione", "Non ci hai permesso di accedere alla tua galleria, pertanto non è stato possibile salvare l'orario", "Ok");
            }



            var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);  
            var pictures = dir.AbsolutePath;  
            //adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name  
            string name = filename + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";  
            string filePath = System.IO.Path.Combine(pictures, name);  
            try  
            {  
                System.IO.File.WriteAllBytes(filePath, imageData);  
                //mediascan adds the saved image into the gallery  
                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);  
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new File(filePath)));  
                Xamarin.Forms.Forms.Context.SendBroadcast(mediaScanIntent);

                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Successo!", "L'orario è stato salvato nelle tue foto", "Ok");
            }
            catch (System.Exception e)  
            {  
                System.Console.WriteLine(e.ToString());  
            }  

        }  

        public void SendToast(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();

            try
            {
               Xamarin.Essentials.Vibration.Vibrate();

                var duration = TimeSpan.FromMilliseconds(200);
                Xamarin.Essentials.Vibration.Vibrate(duration);
            }
            catch
            {
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
        public void SetFormSheet(Page page)
        {
            //Lol ti pare
        }
    //public float GetBottomSafeAreInset()
    //{
    //    //Lol ti pare
    //    return 0;
    //}
    //public void AnimateKeyboard(Xamarin.Forms.Frame frame, Syncfusion.ListView.XForms.SfListView list) { }
    public void SetTabBar(Xamarin.Forms.TabbedPage tabpage)
        {
            tabpage.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            tabpage.On<Xamarin.Forms.PlatformConfiguration.Android>().DisableSwipePaging();
        }

        public void AnimateKeyboard(Frame frame)
        {

        }
    }
}
