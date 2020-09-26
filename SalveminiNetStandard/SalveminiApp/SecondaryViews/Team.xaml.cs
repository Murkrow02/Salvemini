using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class Team : ContentPage
    {
        public Team()
        {
            InitializeComponent();

            layout.Children.Add(new Controls.TeamPersonFrame { Icon = "safariLogo", Image = "codexLogo.png", Title = "Codex Development", SubTitle = "Applicazioni mobile, siti web e molto altro", IconUrl = "http://codexdevelopment.net" });
            layout.Children.Add(new Controls.TeamPersonFrame { Icon = "instaLogo", Image = Costants.Uri("images/fastuser/2106"), Title = "Marco Coppola", SubTitle = "Backend e Frontend Developer", IconUrl = "https://www.instagram.com/_murkrow_/" });
            layout.Children.Add(new Controls.TeamPersonFrame { Icon = "instaLogo", Image = Costants.Uri("images/fastuser/2125"), Title = "Valerio de Nicola", SubTitle = "Frontend Developer, UI e UX Designer", IconUrl = "https://www.instagram.com/vale_denicola/" });
            layout.Children.Add(new Controls.TeamPersonFrame { Icon = "instaLogo", Image = Costants.Uri("images/fastuser/2186"), Title = "Ilaria Pontecorvo", SubTitle = "Graphic & UI Designer", IconUrl = "https://www.instagram.com/jlarjia/" });

        }
    }
}
