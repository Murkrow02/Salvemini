using System;
using System.Collections.Generic;
using SalveminiApp.Helpers;
using Xamarin.Forms;

namespace SalveminiApp
{
    public partial class TabPage : TabbedPage
    {
        //Home
        public static Xamarin.Forms.NavigationPage Home = new Xamarin.Forms.NavigationPage(new Home())
        {
            BarBackgroundColor = Styles.PrimaryGym,
            BarTextColor = Color.White,
            Title = "Home"
        };
        public static Xamarin.Forms.NavigationPage ArgoMenu = new Xamarin.Forms.NavigationPage(new TabPages.ArgoMenu())
        {
            BarBackgroundColor = Styles.PrimaryColor,
            BarTextColor = Color.White,
            Title = "Registro"
        };

        public TabPage()
        {

            InitializeComponent();
            //Configure pages
#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                Palestra.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersLargeTitles(true);
                Home.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersLargeTitles(true);
                BarBackgroundColor = Color.White;
            }
#endif

            //Add Children
            Children.Add(Home);
            Children.Add(ArgoMenu);

            //Initial Page
            SelectedItem = Children[1];
        }
    }
}
