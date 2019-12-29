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
#if __ANDROID__
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
#endif
using Xamarin.Forms.Xaml;

namespace SalveminiApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabPage : Xamarin.Forms.TabbedPage
    {
        //Home
        public Helpers.CustomNavigationPage Home = new Helpers.CustomNavigationPage(new MainPage())
        {
            BarTextColor = Styles.TextColor,
            BarBackgroundColor = Styles.BGColor,
            Title = "Home",
            IconImageSource = "fillTabBarHome.png"
        };

        //Argo
        public static Helpers.CustomNavigationPage Argo = new Helpers.CustomNavigationPage(new ArgoPage())
        {
            BarTextColor = Styles.PrimaryColor,
            BarBackgroundColor = Color.White,
            Title = "Registro",
            IconImageSource = "tabBarArgo.png"
        };

        public static Helpers.CustomNavigationPage Family = new Helpers.CustomNavigationPage(new iCringe.Home())
        {
            BarTextColor = Styles.BGColor,
            BarBackgroundColor = Styles.SecretsPrimary,
            Title = "iCringe",
            IconImageSource = "tabBarArgo.png"
        };


        public TabPage()
        {
            InitializeComponent();

            //Initialize Bar
            BarTextColor = Styles.TextColor;

            //Add Children
            Children.Add(Family);
            Children.Add(Home);
            Children.Add(Argo);

            CurrentPage = Home;

#if __IOS__
            SelectedTabColor = Styles.TextColor;
#endif
#if __ANDROID__
            BarTextColor = Styles.TextColor;
            BarBackgroundColor = Color.White;
            SelectedTabColor = Styles.TextColor;
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            On<Xamarin.Forms.PlatformConfiguration.Android>().DisableSwipePaging();
#endif
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            var index = Children.IndexOf(CurrentPage);


            try
            {
                switch (index)
                {
                    case 0:
                        //iCringe
#if __IOS__
                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar2.jpg");
#endif
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarHome.png";
                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "tabBarArgo.png";
                        break;
                    case 1:
                        //Home
#if __IOS__
                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg","bbar.jpg");
#endif
                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarHome.png";
                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "tabBarArgo.png";
                        break;

                    case 2:
                        //Argo
#if __IOS__
                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar.jpg");
#endif
                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarArgo.png";
                        break;
                }
            }
            catch
            {

            }
            
        }
    }
}

