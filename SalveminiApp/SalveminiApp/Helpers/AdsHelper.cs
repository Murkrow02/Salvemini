using System;
namespace SalveminiApp
{
	public class AdsHelper
	{

		public static string RewardId()
		{
#if DEBUG
            return "ca-app-pub-3940256099942544/5224354917";
#else
#if __IOS__
            return "ca-app-pub-2688730930606353/4691822196";
#endif
#if __ANDROID__
            return "ca-app-pub-2688730930606353/7086530178";
#endif
#endif

		}

		public static string BannerId()
		{
#if DEBUG
            return "ca-app-pub-3940256099942544/2934735716";
#else
#if __IOS__
            return "ca-app-pub-2688730930606353/6777004996";
#endif
#if __ANDROID__
            return "ca-app-pub-2688730930606353/7874464247";
#endif
#endif

		}

		public static string InterstitialId()
		{
#if DEBUG
            return "ca-app-pub-3940256099942544/5135589807";
#else
#if __IOS__
            return "ca-app-pub-2688730930606353~1053541819";
#endif
#if __ANDROID__
            return "ca-app-pub-2688730930606353/9730471390";
#endif
#endif

		}


	}
}
