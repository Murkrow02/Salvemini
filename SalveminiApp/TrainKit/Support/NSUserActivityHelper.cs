using System;
using Foundation;

#if __IOS__
using CoreSpotlight;
using UIKit;
using System.Linq.Expressions;
using System.Xml;
#endif

namespace TrainKit.Support
{
    public static class NSUserActivityHelper
    {
        public static class ActivityKeys
        {
            public const string MenuItems = "menuItems";
            public const string SegueId = "segueID";
        }

        static string SearchableItemContentType = "Soup Menu";

        public static string ViewMenuActivityType = "com.codex.SalveminiApp.viewMenu";

        public static NSUserActivity ViewMenuActivity {
            get
            {
                var userActivity = new NSUserActivity(ViewMenuActivityType)
                {
                    Title =  NSBundleHelper.TrainKitBundle.GetLocalizedString("TITLE", "View menu activity title"),
                    EligibleForSearch = true,
                    EligibleForPrediction = true
                };

                var attributes = new CSSearchableItemAttributeSet(NSUserActivityHelper.SearchableItemContentType)
                {
                    //ThumbnailData = UIImage.FromBundle("tomato").AsPNG(),
                    Keywords = ViewMenuSearchableKeywords,
                    DisplayName = NSBundleHelper.TrainKitBundle.GetLocalizedString("TITLE", "View menu activity title"),
                    ContentDescription = NSBundleHelper.TrainKitBundle.GetLocalizedString("ANSWER", "View menu content description")
                };
                userActivity.ContentAttributeSet = attributes;

                var phrase = NSBundleHelper.TrainKitBundle.GetLocalizedString("ANSWER", "Voice shortcut suggested phrase");
                userActivity.SuggestedInvocationPhrase = phrase;
                return userActivity;
            }
        }

        static string[] ViewMenuSearchableKeywords = new string[] {
            NSBundleHelper.TrainKitBundle.GetLocalizedString("TRAIN",  "Searchable Keyword"),
            NSBundleHelper.TrainKitBundle.GetLocalizedString("HOUR", "Searchable Keyword"),
            NSBundleHelper.TrainKitBundle.GetLocalizedString("CITY", "Searchable Keyword")
        };
    }
}
