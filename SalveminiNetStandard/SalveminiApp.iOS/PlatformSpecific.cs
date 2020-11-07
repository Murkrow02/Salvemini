using System;
using Foundation;
using Plugin.Iconize;
using SalveminiApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformSpecific))]
namespace SalveminiApp.iOS
{
    public class PlatformSpecific : SalveminiApp.IPlatformSpecific
    {
        public PlatformSpecific() { }

        public void SavePictureToDisk(string filename, byte[] imageData)
        {
            var chartImage = new UIImage(NSData.FromArray(imageData));
            chartImage.SaveToPhotosAlbum((image, error) =>
            {
                //you can retrieve the saved UI Image as well if needed using  
                //var i = image as UIImage;  
                if (error != null)
                {
                    Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Errore", "Non è stato possibile salvare l'orario", "Ok");

                    Console.WriteLine(error.ToString());
                }
                else
                {
                    Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Successo!", "L'orario è stato salvato", "Ok");
                }
            });
        }


        public void SendToast(string message)
        {
            //Show toast
            BigTed2.BTProgressHUD2.ShowToast(message, BigTed2.ProgressHUD2.MaskType.None, false, 3000);

            //Vibrate
            AppDelegate.hapticVibration();
        }

        //Detect homebar
        public bool HasBottomBar()
        {
            try
            {
                return UIApplication.SharedApplication.Delegate.GetWindow().SafeAreaInsets.Bottom > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //    //Open device settings
        //    public void OpenSettings()
        //    {
        //        UIApplication.SharedApplication.OpenUrl(new NSUrl($"app-settings:"));
        //    }

        //    //Keep focus on keyboard dismiss
        //    public void CanUnfocus(bool status)
        //    {
        //       // SecondaryViews.TrainerChat.canUnfocus = status;
        //    }

        //    //Dismiss keyboard
        //    public void HideKeyboard()
        //    {
        //        UIApplication.SharedApplication.KeyWindow.EndEditing(true);
        //    }

        //    //Blur the element
        //    public void BlurLayout(Xamarin.Forms.StackLayout view)
        //    {
        //        view.On<Xamarin.Forms.PlatformConfiguration.iOS>().UseBlurEffect(BlurEffectStyle.Light);
        //    }

        //Set safe area
        public void SetSafeArea(Xamarin.Forms.ContentPage page)
        {
            page.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        //Show page as a formsheet (ios >= 13 )
        public void SetFormSheet(Xamarin.Forms.Page page)
        {
            page.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
        }

        //Used for frame over keyboard
        public void AnimateKeyboard(Frame frame)
        {
            UIKit.UIKeyboard.Notifications.ObserveWillShow((s, e) =>
            {
                var r = UIKit.UIKeyboard.FrameEndFromNotification(e.Notification);
                frame.TranslateTo(0, -r.Height, (uint)(e.AnimationDuration * 1000));
            });

            UIKit.UIKeyboard.Notifications.ObserveWillHide((s, e) =>
            {
                var r = UIKit.UIKeyboard.FrameBeginFromNotification(e.Notification);
                frame.TranslateTo(0, 0, (uint)(e.AnimationDuration * 1000));
            });
        }

        public void SetTabBar(Xamarin.Forms.TabbedPage tabpage)
           { }

        //    public float GetBottomSafeAreInset()
        //    {
        //        try
        //        {
        //            return (float)UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom;
        //        }
        //        catch (Exception ex)
        //        {
        //            Costants.SendCrash(ex, "PlatformSpecific.cs", "GetBottomSafeAreaInset");
        //            return 0;
        //        }
        //    }
    }

}