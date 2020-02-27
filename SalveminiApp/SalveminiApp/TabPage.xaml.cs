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
            IconImageSource = "tabBarHome.png"
        };

        //Argo
        public static Helpers.CustomNavigationPage Argo = new Helpers.CustomNavigationPage(new ArgoPage())
        {
            BarTextColor = Styles.PrimaryColor,
            BarBackgroundColor = Color.White,
            Title = "Registro",
            IconImageSource = "tabBarArgo.png"
        };

        //iCringe
        public static Helpers.CustomNavigationPage Family = new Helpers.CustomNavigationPage(new iCringe.Home())
        {
            BarTextColor = Styles.BGColor,
            BarBackgroundColor = Styles.SecretsPrimary,
            Title = "iCringe",
            IconImageSource = "iCringe.png"
        };


        public TabPage(int selectedPage = 0)
        {
            InitializeComponent();

            //Initialize Bar
            BarTextColor = Styles.TextColor;

            //Add Children
            //Children.Add(Family);
            Children.Add(Home);
            Children.Add(Argo);

            CurrentPage = Children[selectedPage];
            ColorSelected(selectedPage);

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
                        //Home
#if __IOS__
                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar.jpg");
#endif
#if __ANDROID__
                        //if(MainPage.appearedTimes > 0)
                        //((Children[0] as Helpers.CustomNavigationPage).RootPage as MainPage).AndroidFix();
#endif
                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarHome.png";
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "tabBarArgo.png";
                        break;
                    case 1:
                        //Argo
#if __IOS__
                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar.jpg");
#endif
                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarArgo.png";
                        break;



                        //                    case 0:
                        //                        //iCringe
                        //#if __IOS__
                        //                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar2.jpg");
                        //#endif
                        //#if __ANDROID__
                        //                        if(MainPage.appearedTimes > 0)
                        //                        ((Children[0] as Helpers.CustomNavigationPage).RootPage as iCringe.Home).AndroidFix();
                        //#endif
                        //                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "iCringeFill.png";
                        //                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        //                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "tabBarArgo.png";
                        //                        break;
                        //                    case 1:
                        //                        //Home
                        //#if __IOS__
                        //                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar.jpg");
                        //#endif
                        //                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "iCringe.png";
                        //                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarHome.png";
                        //                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "tabBarArgo.png";
                        //                        break;

                        //                    case 2:
                        //                        //Argo
                        //#if __IOS__
                        //                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar.jpg");
                        //#endif
                        //#if __ANDROID__
                        //                        if (MainPage.appearedTimes > 0)
                        //                            ((Children[2] as Helpers.CustomNavigationPage).RootPage as ArgoPage).AndroidFix();
                        //#endif
                        //                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "iCringe.png";
                        //                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        //                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarArgo.png";
                        //                        break;

                }
            }
            catch
            {

            }

        }

        public void ColorSelected(int i)
        {
            switch (i)
            {
                case 0:
                    (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarHome.png";
                    break;
                case 1:
                    
                    (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarArgo.png";
                    break;
                    //case 0:
                    //    (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "iCringeFill.png";
                    //    break;
                    //case 1:
                    //    (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarHome.png";

                    //    break;
                    //case 2:
                    //    (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarArgo.png";
                    //    break;
            }
        }
    }
}

