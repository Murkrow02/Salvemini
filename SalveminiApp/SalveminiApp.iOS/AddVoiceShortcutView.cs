using System;
using Foundation;
using Intents;
using IntentsUI;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.iOS
{
    public class AddVoiceShortcutView : UIViewController, IINUIAddVoiceShortcutViewControllerDelegate, IINUIEditVoiceShortcutViewControllerDelegate
    {
        public ContentPage page;
        public int station;
        public bool direction;

        public AddVoiceShortcutView(ContentPage page_, int station_, bool direction_)
        {
            page = page_;
            station = station_;
            direction = direction_;
        }

        public IntPtr Handle => throw new NotImplementedException();

        #region INUIEditVoiceShortcutViewControllerDelegate
        public void DidUpdate(INUIEditVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            if (!(error is null))
            {
                Console.WriteLine($"error updating voice shortcut", error);
                return;
            }
            // UpdateVoiceShortcuts();
        }

        public void DidDelete(INUIEditVoiceShortcutViewController controller, NSUuid deletedVoiceShortcutIdentifier)
        {
            // UpdateVoiceShortcuts();
        }

        public void DidCancel(INUIEditVoiceShortcutViewController controller)
        {
            page.Navigation.PopModalAsync();
        }
        #endregion

        #region INUIAddVoiceShortcutViewControllerDelegate
        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            
            //Save values for siri intent
            var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
            defaults.AddSuite("group.com.codex.SalveminiApp");
            defaults.SetInt(station, new NSString("savedStation" + voiceShortcut.Shortcut.Intent.IdentifierString.Description));
            defaults.SetBool(direction, new NSString("savedDirection" + voiceShortcut.Shortcut.Intent.IdentifierString.Description));

            if (!(error is null)) //Error occourred
            {
                page.DisplayAlert("Errore", "Non è stato possibile aggiungere l'azione", "Chiudi");
            }
            else //All went good
            {
                page.DisplayAlert("Successo", "Prova a dire \"Ehi Siri, " + voiceShortcut.InvocationPhrase + "\"", "K");
            }

            //Close page
            page.Navigation.PopModalAsync();
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            page.Navigation.PopModalAsync();
        }

        public void Dispose()
        {

        }
        #endregion
    }
}
