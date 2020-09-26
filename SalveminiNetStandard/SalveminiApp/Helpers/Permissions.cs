using System;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Plugin.Media;
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
                
                
                bool openSettings = await currentPage.DisplayAlert("Errore", "Non ci hai concesso di accedere alla tua posizione, apri l'app impostazioni del tuo telefono e consenti l'accesso per Salvemini", "Impostazioni", "Chiudi");
                if (openSettings)
                    CrossPermissions.Current.OpenAppSettings();
                return false;
            }
            return true;

        }

        public static async Task<string> positionValidity(Xamarin.Essentials.Location location)
        {
            //if (location.Accuracy > 30)
            //    return "La tua posizione non è abbastanza accurata, riprova";
            if (location.IsFromMockProvider)
                return "Cooooooosa? Qualcuno sta cercando di imbrogliare qua? Non è stato possibile verificare l'autenticità della tua posizione, disattiva i servizi di localizzazione falsi";
            return "";
        }

        public static async Task<bool> checkPermissions()
        {
            var currentPage = Application.Current.MainPage;

            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                cameraStatus = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                storageStatus = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                bool openSettings = await currentPage.DisplayAlert("Errore", "Non ci hai concesso di accedere alla fotocamera ed alle tue foto, apri le impostazioni per continuare", "Impostazioni", "Chiudi");
                if (openSettings)
                    CrossPermissions.Current.OpenAppSettings();

                return false;
            }
        }
    }
}
