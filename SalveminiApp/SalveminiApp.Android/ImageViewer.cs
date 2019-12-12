
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SalveminiApp.PhotoBrowser;
using Xamarin.Forms;
using Stormlion.PhotoBrowser;
namespace SalveminiApp.Droid
{
    public class PhotoBrowserImplementation : PhotoBrowser.IPhotoBrowser
    {

        public void Show(SalveminiApp.PhotoBrowser.PhotoBrowser photoBrowser)
        {
            var a = new Stormlion.PhotoBrowser.PhotoBrowser();
            var photos = new List<Stormlion.PhotoBrowser.Photo>();
            foreach (var foto in photoBrowser.Photos) { photos.Add(new Stormlion.PhotoBrowser.Photo { Title = foto.Title, URL = foto.URL }); }
            a.Photos = photos;
            a.Show();
        }

        public void Close()
        {

        }


    }

    public class Platform
    {
        public static Context Context { get; set; }

        public static void Init(Context context)
        {
            Context = context;
            DependencyService.Register<PhotoBrowserImplementation>();
        }
    }

}
