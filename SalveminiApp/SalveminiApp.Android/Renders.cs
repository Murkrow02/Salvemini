using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SalveminiApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer_Droid))]
[assembly: ExportRenderer(typeof(Picker), typeof(CustomPickerRenderer_Droid))]
namespace SalveminiApp.Droid
{
    public class CustomEntryRenderer_Droid : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

    }

    public class CustomPickerRenderer_Droid : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

    }

   
    }
}
