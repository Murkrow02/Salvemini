using System;
using Forms9Patch;
using Xamarin.Forms;

namespace SalveminiApp.Helpers
{
    public class PopOvers
    {
       public BubblePopup compitiPopOver = new BubblePopup(null);

        public PopOvers()
        {
            compitiPopOver.PointerDirection = PointerDirection.Up;
            compitiPopOver.PreferredPointerDirection = PointerDirection.Up;
            compitiPopOver.PointerLength = 10;
            compitiPopOver.PointerTipRadius = 3;
            compitiPopOver.HasShadow = false;
            compitiPopOver.IsAnimationEnabled = true;
            compitiPopOver.Animation = new Rg.Plugins.Popup.Animations.ScaleAnimation();
            compitiPopOver.BorderRadius = 10;
        }

           
    }
}
