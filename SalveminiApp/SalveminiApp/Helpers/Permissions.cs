using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace SalveminiApp.Helpers
{
    public class Permissions
    {
        public static async Task<bool> locationPermission()
        {
            var currentPage = Application.Current.MainPage;

            //ACCESS POSITION
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Location))
                {
                    return true;
                }

                await currentPage.DisplayAlert("Errore", "Non ci hai concesso di accedere alla tua posizione, apri l'app impostazioni del tuo telefono e consenti l'accesso per Salvemini", "OK");
                return false;
            }
            return true;

        }

        public static async Task<string> positionValidity(Xamarin.Essentials.Location location)
        {
            //if (location.Accuracy > 30)
            //    return "La tua posizione non è abbastanza accurata, riprova";
            //   if (location.IsFromMockProvider)
            //    return "Cooooooosa? Qualcuno sta cercando di imbrogliare qua? Non è stato possibile verificare l'autenticità della tua posizione, disattiva i servizi di localizzazione falsi";
            return "";
        }

        public static async Task<bool> checkPermissions()
        {
            var currentPage = Application.Current.MainPage;

            //ACCESS CAMERA
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

            if (status != PermissionStatus.Granted)
            {
                //if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                //{
                //}

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Camera))
                {
                    status = results[Permission.Camera];
                }
                else
                {
                    await currentPage.DisplayAlert("Errore", "Non abbiamo il permesso di accedere alla fotocamera", "Ok");
                    return false;
                }
            }

            //ACCESS GALLERY
            var status2 = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);

            if (status2 != PermissionStatus.Granted)
            {
                //if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                //{
                //    await currentPage.DisplayAlert("Errore", "Non abbiamo il permesso di accedere alle tue foto", "OK");
                //}

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Photos))
                {
                    status2 = results[Permission.Photos];
                }
                else
                {
                    await currentPage.DisplayAlert("Errore", "Non abbiamo il permesso di accedere alle tue foto", "Ok");
                    return false;
                }

            }
#if __ANDROID__
            try
            {
                //ACCESS CAMERA
                var status1 = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

                if (status1 != PermissionStatus.Granted)
                {
                    //if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    //{
                    //}

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Storage))
                    {
                        status1 = results[Permission.Storage];

                    }
                    else
                    {
                        await currentPage.DisplayAlert("Errore", "Non abbiamo il permesso di accedere alle tue foto", "OK");
                        return false;
                    }
                }
            }
            catch { }
           
#endif
           
            return true;
        }
    }
}
