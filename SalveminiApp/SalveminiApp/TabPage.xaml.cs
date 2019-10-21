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
        public static CarouselPage CarouselHome = new CarouselPage();
        public static Helpers.CustomNavigationPage Home = new Helpers.CustomNavigationPage(new MainPage())
        {
            BarTextColor = Styles.PrimaryColor,
            BarBackgroundColor = Styles.BGColor,
            Title = "Home",
            IconImageSource = "tabBarHome.png"
        };
        public static Helpers.CustomNavigationPage Argo = new Helpers.CustomNavigationPage(new ArgoPage())
        {
            BarTextColor = Styles.PrimaryColor,
            BarBackgroundColor = Color.White,
            Title = "Argo",
            IconImageSource = "tabBarArgo.png"
        };
        public static Helpers.CustomNavigationPage User = new Helpers.CustomNavigationPage(new UserPage())
        {
            BarTextColor = Styles.PrimaryColor,
            BarBackgroundColor = Color.White,
            Title = "Profilo",
            IconImageSource = "tabBarProfile.png"
        };

        public TabPage()
        {
            InitializeComponent();

            //Init Carousel
            CarouselHome.Children.Add(new MainPage());
            CarouselHome.Children.Add(new SecondHome());
            CarouselHome.Title = "Home";

            //Initialize Bar
            BarTextColor = Styles.TextColor;


            //Add Children
            Children.Add(Home);
            Children.Add(Argo);
            Children.Add(User);


#if __IOS__
            SelectedTabColor = Styles.TextColor;
#endif
#if __ANDROID__
            BarTextColor = Color.White;
            BarBackgroundColor = Styles.PrimaryColor;
            SelectedTabColor = Color.White;
#endif
        }
    }
}

