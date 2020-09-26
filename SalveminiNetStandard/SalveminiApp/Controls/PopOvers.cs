using System;
using Forms9Patch;
using Xamarin.Forms;

namespace SalveminiApp.Helpers
{
    public class PopOvers
    {
       public BubblePopup defaultPopOver = new BubblePopup(null);

        public PopOvers()
        {
            defaultPopOver.PointerLength = 10;
            defaultPopOver.PointerTipRadius = 3;
            defaultPopOver.HasShadow = false;
            defaultPopOver.IsAnimationEnabled = true;
            //defaultPopOver.Animation = (Forms9Patch.Elements.Popups.Core.IPopupAnimation)new Rg.Plugins.Popup.Animations.ScaleAnimation();
            defaultPopOver.BorderRadius = 10;
        }

           
    }
}
