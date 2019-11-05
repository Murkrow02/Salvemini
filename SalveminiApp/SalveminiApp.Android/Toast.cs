using System;
using Android.App;
using Android.Widget;

namespace SalveminiApp.Droid
{
    public class ShowToast
    {
        public static void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

    }
}
