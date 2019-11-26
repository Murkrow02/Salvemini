using System;
using System.Collections.Generic;
#if __ANDROID__
using Java.IO;
using Android.Content;
using Android.Widget;
using Plugin.CurrentActivity;
#endif
using Plugin.Permissions;
using System.Net;
using Plugin.Permissions.Abstractions;
#if __IOS__
using UIKit;
using Foundation;
#endif
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using Syncfusion.SfCarousel.XForms;

namespace SalveminiApp.Helpers
{
    public partial class ImageViewer : ContentPage
    {
        string FullUrl = "";
        List<string> Images = new List<string>();

        public ImageViewer(List<string> Urls)
        {
            InitializeComponent();
            ImageCarousel.ItemsSource = Urls;
            Images = Urls;
            FullUrl = Urls[0];
            MainPage.isSelectingImage = true;
        }

        private void Image_SelectionChanged(object sender, Syncfusion.SfCarousel.XForms.SelectionChangedEventArgs e)
        {
            FullUrl = Images[e.SelectedIndex];
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void ImageSwipedLeft(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Left)
            {
                if (Images.ElementAtOrDefault(ImageCarousel.SelectedIndex + 1) != null)
                {
                    ImageCarousel.SelectedIndex++;
                }
            }
        }

        void ImageSwipedRight(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Right)
            {
                if (Images.ElementAtOrDefault(ImageCarousel.SelectedIndex - 1) != null)
                {
                    ImageCarousel.SelectedIndex--;
                }
            }
        }

        async void Save_Clicked(object sender, EventArgs e)
        { 
            if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                await DisplayAlert(null, "Connettiti ad internet per salvare le foto", "Ok");
                return;
            }




        //ACCESS CAMERA
        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);              if (status != PermissionStatus.Granted)             {                 if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))                 { 
#if __IOS__                     new UIAlertView("Errore", "Non abbiamo il permesso di accedere alle foto",null, "OK",null).Show();

#endif                 }

    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);                 //Best practice to always check that the key exists                 if (results.ContainsKey(Permission.Camera))                 {                     status = results[Permission.Camera];                     System.Console.WriteLine("Success");                 }                 }


#if __IOS__                     var someImage = UIImageFromUrl(FullUrl);
UIImage UIImageFromUrl(string uri)
{
    using (var url = new NSUrl(uri))
    using (var data = NSData.FromUrl(url))
        return UIImage.LoadFromData(data);
}

someImage.SaveToPhotosAlbum((image, error) => {                         var o = image as UIImage;                         if(error == null) {                              new UIAlertView("Successo", "L'immagine è stata aggiunta al tuo rullino foto", null, "Ok", null).Show();                         }                         else {                             new UIAlertView("Errore", "Non è stato possibile scaricare l'immagine, controlla di averci concesso i permessi di accedere al tuo rullino", null, "Ok", null).Show();                          }                     });

#endif 
#if __ANDROID__              //ACCESS CAMERA             var status1 = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);              if (status1 != PermissionStatus.Granted)             {                 if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))                 { 
#if __IOS__                     new UIAlertView("Errore", "Non abbiamo il permesso di accedere alle foto",null, "OK",null).Show(); 
#endif                 }                  var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);                 //Best practice to always check that the key exists                 if (results.ContainsKey(Permission.Storage))                 {                     status1 = results[Permission.Storage];                     System.Console.WriteLine("Success");                 }                 }            string filename = "cityFix";
                string Url = FullUrl;                 byte[] imageData; 
                using (var webClient = new WebClient())
                {
                   imageData = webClient.DownloadData(Url);
                } 
                {               var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);               var pictures = dir.AbsolutePath;               //adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name               string name = filename + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";               string filePath = System.IO.Path.Combine(pictures, name);               try               {                   System.IO.File.WriteAllBytes(filePath, imageData);                   //mediascan adds the saved image into the gallery                   var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);                   mediaScanIntent.SetData(Android.Net.Uri.FromFile(new File(filePath)));                   Xamarin.Forms.Forms.Context.SendBroadcast(mediaScanIntent);                 Toast.MakeText(CrossCurrentActivity.Current.Activity, "Foto salvata nella galleria", ToastLength.Long).Show(); 
                }               catch(System.Exception ex)               {
                    Toast.MakeText(CrossCurrentActivity.Current.Activity, "Non è stato possibile salvare la foto, controlla di averci permesso di accedere ai tuoi file dalle impostazioni del tuo dispositivo", ToastLength.Long).Show();

                }

            }  
#endif 
          

        }          protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //iOS 13 bug
            MainPage.isSelectingImage = false;
        } 
    }
}
