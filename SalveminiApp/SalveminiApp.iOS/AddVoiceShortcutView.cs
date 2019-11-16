using System;
using Foundation;
using Intents;
using IntentsUI;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.iOS
{
    public class AddVoiceShortcutButton : INUIAddVoiceShortcutButtonDelegate
    {
       
        public int station;
        public bool direction;

        public AddVoiceShortcutButton( int station_, bool direction_)
        {
            station = station_;
            direction = direction_;
        }

        //Push to add new shortcut
        public override void PresentAddVoiceShortcut(INUIAddVoiceShortcutViewController addVoiceShortcutViewController, INUIAddVoiceShortcutButton addVoiceShortcutButton)
        {
           // UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(false);
            addVoiceShortcutViewController.Delegate = new AddVoiceShortcutView(station,direction);
            addVoiceShortcutViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController.PresentViewController(addVoiceShortcutViewController, animated: true, null);
        }
    
        //Push to edit existing shortcut
        public override void PresentEditVoiceShortcut(INUIEditVoiceShortcutViewController editVoiceShortcutViewController, INUIAddVoiceShortcutButton addVoiceShortcutButton)
        {
            //UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(true);
            editVoiceShortcutViewController.Delegate = new EditVoiceShortcutView(station, direction);
            editVoiceShortcutViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController.PresentViewController(editVoiceShortcutViewController, animated: true, null);
        }

    }

    public class AddVoiceShortcutView : INUIAddVoiceShortcutViewControllerDelegate
    {
        public int station;
        public bool direction;

        public AddVoiceShortcutView( int station_, bool direction_)
        {
            station = station_;
            direction = direction_;
        }

        //User cancelled, remove popup
        public override void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(true);
        }

        //User added successfully a new shortcut, save values
        public override void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            //Save values for siri intent
            //var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
            //defaults.AddSuite("group.com.codex.SalveminiApp");
            //defaults.SetInt(station, new NSString("savedStation"));
            //defaults.SetBool(direction, new NSString("savedDirection"));

            //Close page
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(true);
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(true);

            //Display response to user
            Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Comando aggiunto!", "Prova a dire \"Ehi Siri, " + voiceShortcut.InvocationPhrase + "\"", "K");

        }
    }


    public class EditVoiceShortcutView : INUIEditVoiceShortcutViewControllerDelegate
    {
        public int station;
        public bool direction;

        public EditVoiceShortcutView(int station_, bool direction_)
        {
            station = station_;
            direction = direction_;
        }

        //User cancelled, remove popup
        public override void DidCancel(INUIEditVoiceShortcutViewController controller)
        {
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(true);
        }

        //User deleted existing shortcut
        public override void DidDelete(INUIEditVoiceShortcutViewController controller, NSUuid deletedVoiceShortcutIdentifier)
        {
            //Close page
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(true);
        }

        //User cancelled, remove popup
        public override void DidUpdate(INUIEditVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            //Close page
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissModalViewController(true);

            //Display response to user
            Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Comando aggiornato!", "Prova a dire \"Ehi Siri, " + voiceShortcut.InvocationPhrase + "\"", "K");

        }
    }
}
