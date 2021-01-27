using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
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
        public static Helpers.CustomNavigationPage iCringe = new Helpers.CustomNavigationPage(new iCringe.Home())
        {
            BarTextColor = Styles.BGColor,
            BarBackgroundColor = Styles.SecretsPrimary,
            Title = "Salvemini",
            IconImageSource = "iCringe.png"
        };


        public TabPage(int selectedPage = 1)
        {
            InitializeComponent();

            //Configure pages
            DependencyService.Get<IPlatformSpecific>().SetTabBar(this);

            //Initialize Bar
            BarTextColor = Styles.TextColor;

            //Add Children
            //Children.Add(iCringe);
            Children.Add(Home);
            //Children.Add(Argo);

            CurrentPage = Children[selectedPage];
            //ColorSelected(selectedPage);

            if (Device.RuntimePlatform == Device.iOS)
                SelectedTabColor = Styles.TextColor;
            else
            {
                BarTextColor = Styles.TextColor;
                BarBackgroundColor = Color.White;
                SelectedTabColor = Styles.TextColor;

            }

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
                        if (Device.RuntimePlatform == Device.iOS)
                            MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar2.jpg");
                        else
                            MessagingCenter.Send((App)Application.Current, "RefreshPosts");

                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "iCringeFill.png";
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "tabBarArgo.png";
                        break;
                    case 1:
                        //Home
if(Device.RuntimePlatform == Device.iOS)
                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar.jpg");
                        else
                        {
                            //if(MainPage.appearedTimes > 0)
                            //((Children[0] as Helpers.CustomNavigationPage).RootPage as MainPage).AndroidFix();
                        }
                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "iCringe.png";
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarHome.png";
                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "tabBarArgo.png";
                        break;

                    case 2:
                        //Argo
if(Device.RuntimePlatform == Device.iOS)
                        MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "changeBg", "bbar.jpg");
                        (Children[0] as Helpers.CustomNavigationPage).IconImageSource = "iCringe.png";
                        (Children[1] as Helpers.CustomNavigationPage).IconImageSource = "tabBarHome.png";
                        (Children[2] as Helpers.CustomNavigationPage).IconImageSource = "fillTabBarArgo.png";
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

