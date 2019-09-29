/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A data manager that surfaces INVoiceShortcuts managed by INVoiceShortcutCenter.
*/

using System;
using Intents;
using System.Linq;
using SalveminiApp;

namespace TrainKit.Data
{
    public class VoiceShortcutDataManager 
    {
        INVoiceShortcut[] VoiceShortcuts;

        public VoiceShortcutDataManager()
        {
            UpdateVoiceShortcuts(null);
        }

        public INVoiceShortcut VoiceShortcutForOrder()
        {
            var voiceShortcut = VoiceShortcuts.FirstOrDefault((shortcut) =>
            {
                var intent = shortcut.Shortcut.Intent as TrainIntent;
                if (intent is null) { return false; }
                return true;
                //var orderFromIntent = Order.FromOrderSoupIntent(intent);
                //if (orderFromIntent is null) { return false; }
                //return order.IsEqual(orderFromIntent);
            });
            return voiceShortcut;
        }

        public void UpdateVoiceShortcuts(Action completion)
        {
            INVoiceShortcutCenter.SharedCenter.GetAllVoiceShortcuts((voiceShortcutsFromCenter, error) =>
            {
                if (voiceShortcutsFromCenter is null)
                {
                    if (!(error is null))
                    {
                        Console.WriteLine($"Failed to fetch voice shortcuts with error {error}");
                    }
                    return;
                }
                VoiceShortcuts = voiceShortcutsFromCenter;
                if (!(completion is null))
                {
                    completion();
                }
            }); 
        }

    }
}
