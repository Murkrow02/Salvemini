using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if __IOS__
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SalveminiApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabPage : TabbedPage
    {
        //Home
        public static Helpers.CustomNavigationPage Home = new Helpers.CustomNavigationPage(new MainPage())
        {
            BarTextColor = Styles.PrimaryColor,
            BarBackgroundColor = Color.White,
            Title = "Home"
        };
        public static Helpers.CustomNavigationPage Argo = new Helpers.CustomNavigationPage(new ArgoPage())
        {
            BarTextColor = Styles.PrimaryColor,
            BarBackgroundColor = Color.White,
            Title = "Argo"
        };

        public TabPage()
        {
            InitializeComponent();

            //Initialize Bar
            BarTextColor = Styles.PrimaryColor;

            //Add Children
            Children.Add(Home);
            Children.Add(Argo);
        }
    }
}

