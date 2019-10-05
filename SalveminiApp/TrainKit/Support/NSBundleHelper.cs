using System;
using Foundation;
using TrainKit.Data;
using ObjCRuntime;
namespace TrainKit.Support
{
    public static class NSBundleHelper
    {
        public static NSBundle TrainKitBundle {
            get {
                return NSBundle.MainBundle;
            }
        }
    }
}
