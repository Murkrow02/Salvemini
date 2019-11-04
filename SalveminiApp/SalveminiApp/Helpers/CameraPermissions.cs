using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace SalveminiApp.Helpers
{
    public class CameraPermissions
    {
        public CameraPermissions()
        {
        }

        public static async Task<bool> checkPermissions()
        {
            var currentPage = Application.Current.MainPage;

            //ACCESS CAMERA
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await currentPage.DisplayAlert("Errore", "Non abbiamo il permesso di accedere alla fotocamera", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Camera))
                {
                    status = results[Permission.Camera];
                    Console.WriteLine("Success");
                    return true;
                }
            }
            else
            {
                return true;
            }

            //ACCESS GALLERY
            var status2 = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);

            if (status2 != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                {
                    await currentPage.DisplayAlert("Errore", "Non abbiamo il permesso di accedere alle tue foto", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Photos))
                {
                    status2 = results[Permission.Photos];
                    Console.WriteLine("Success");
                    return true;
                }

            }
            else
            {
                return true;
            }
#if __ANDROID__
             //ACCESS CAMERA
            var status1 = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (status1 != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                {
                 await currentPage.DisplayAlert("Errore", "Non abbiamo il permesso di accedere alle tue foto", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Storage))
                {
                    status1 = results[Permission.Storage];
                    System.Console.WriteLine("Success");
                                        return true;

                }

                } else
            {
                return true;
            }
#endif
            return false;
        }
    }
}
