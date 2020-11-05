using System;
using Plugin.Iconize;

namespace SalveminiApp
{
    public interface IPlatformSpecific
    {
        void SendToast(string message);
        bool HasBottomBar();
        //void OpenSettings();
        //void CanUnfocus(bool status);
        //void HideKeyboard();
        void SetSafeArea(Xamarin.Forms.ContentPage page);
        //void SetFormSheet(Xamarin.Forms.Page page);
        //void AnimateKeyboard(Xamarin.Forms.Frame frame, Syncfusion.ListView.XForms.SfListView list, Xamarin.Forms.Frame topFrame);
        //void BlurLayout(Xamarin.Forms.StackLayout view);
        void SetTabBar(Xamarin.Forms.TabbedPage tabpage);
        //float GetBottomSafeAreInset();
    }

}
