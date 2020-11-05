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

        //    //Show page as a formsheet (ios >= 13 )
        //    public void SetFormSheet(Xamarin.Forms.Page page)
        //    {
        //        page.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
        //    }

        //    //Used for frame over keyboard
        //    public void AnimateKeyboard(Frame frame, Syncfusion.ListView.XForms.SfListView list, Frame topFrame)
        //    {
        //        double storedHeight = 0;
        //        bool keyboardDidShow = false; ;
        //        UIKit.UIKeyboard.Notifications.ObserveWillShow((s, e) =>
        //        {
        //            //Check keyboard size
        //            var r = UIKit.UIKeyboard.FrameEndFromNotification(e.Notification);
        //            if (r.Height < 1 || keyboardDidShow)
        //            {
        //                return;
        //            }
        //            frame.TranslateTo(0, -r.Height, (uint)(e.AnimationDuration * 1000));
        //            storedHeight = 0; storedHeight += list.Height;
        //            var h = App.ScreenHeight - frame.Height - r.Height - topFrame.Height;
        //            list.ScaleHeightTo(h / 10, (uint)(e.AnimationDuration * 1000));
        //            Console.WriteLine(list.Height);
        //        });

        //        UIKit.UIKeyboard.Notifications.ObserveDidShow((s, e) =>
        //        {
        //            keyboardDidShow = true;
        //        });

        //        UIKit.UIKeyboard.Notifications.ObserveWillHide((s, e) =>
        //        {
        //            var r = UIKit.UIKeyboard.FrameBeginFromNotification(e.Notification);
        //            frame.TranslateTo(0, 0, (uint)(e.AnimationDuration * 1000));
        //            list.ScaleHeightTo(storedHeight, (uint)(e.AnimationDuration * 1000));
        //        });

        //        UIKit.UIKeyboard.Notifications.ObserveDidHide((s, e) =>
        //        {
        //            keyboardDidShow = false;
        //        });
        //    }

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