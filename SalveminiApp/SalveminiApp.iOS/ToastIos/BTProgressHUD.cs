using System;

#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif

namespace BigTed2
{
	public static class BTProgressHUD2
	{
		public static void Show(string status = null, float progress = -1, ProgressHUD2.MaskType maskType = ProgressHUD2.MaskType.None)
		{
			ProgressHUD2.Shared.Show(status, progress, maskType);
		}

		public static void Show(string cancelCaption, Action cancelCallback, string status = null, float progress = -1, ProgressHUD2.MaskType maskType = ProgressHUD2.MaskType.None)
		{
			ProgressHUD2.Shared.Show(cancelCaption, cancelCallback, status, progress, maskType);
		}

		public static void ShowContinuousProgress(string status = null, ProgressHUD2.MaskType maskType = ProgressHUD2.MaskType.None)
		{
			ProgressHUD2.Shared.ShowContinuousProgress(status, maskType);
		}

		public static void ShowToast(string status, bool showToastCentered = false, double timeoutMs = 1000)
		{
			ShowToast(status, showToastCentered ? ProgressHUD2.ToastPosition.Center : ProgressHUD2.ToastPosition.Bottom, timeoutMs);
		}

		public static void ShowToast(string status, ProgressHUD2.ToastPosition toastPosition = ProgressHUD2.ToastPosition.Center, double timeoutMs = 1000)
		{
			ProgressHUD2.Shared.ShowToast(status, ProgressHUD2.MaskType.None, toastPosition, timeoutMs);
		}

		public static void ShowToast(string status, ProgressHUD2.MaskType maskType = ProgressHUD2.MaskType.None, bool showToastCentered = true, double timeoutMs = 1000)
		{
			ProgressHUD2.Shared.ShowToast(status, maskType, showToastCentered ? ProgressHUD2.ToastPosition.Center : ProgressHUD2.ToastPosition.Bottom, timeoutMs);
		}

		public static void SetStatus(string status)
		{
			ProgressHUD2.Shared.SetStatus(status);
		}

		public static void ShowSuccessWithStatus(string status, double timeoutMs = 1000)
		{
			ProgressHUD2.Shared.ShowSuccessWithStatus(status, timeoutMs);
		}

		public static void ShowErrorWithStatus(string status, double timeoutMs = 1000)
		{
			ProgressHUD2.Shared.ShowErrorWithStatus(status, timeoutMs);
		}

		public static void ShowImage(UIImage image, string status, double timeoutMs = 1000)
		{
			ProgressHUD2.Shared.ShowImage(image, status, timeoutMs);
		}

		public static void Dismiss()
		{
			ProgressHUD2.Shared.Dismiss();
		}

		public static bool IsVisible
		{
			get
			{
				return ProgressHUD2.Shared.IsVisible;
			}
		}

		public static bool ForceiOS6LookAndFeel
		{
			get
			{
				return ProgressHUD2.Shared.ForceiOS6LookAndFeel;
			}
			set
			{
				ProgressHUD2.Shared.ForceiOS6LookAndFeel = value;
			}
		}
	}
}

