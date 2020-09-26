using System;
using BigTed2;

namespace XHUD2
{
	public enum MaskType
	{
		None = 1,
		Clear,
		Black,
		Gradient
	}

	public static class HUD
	{
		public static void Show(string message, int progress = -1, MaskType maskType = MaskType.None)
		{
			float p = (float)progress / 100f;
			BTProgressHUD2.Show(message, p, (ProgressHUD2.MaskType)maskType);
		}

		public static void Dismiss()
		{
			BTProgressHUD2.Dismiss();
		}

		public static void ShowToast(string message, bool showToastCentered = true, double timeoutMs = 1000)
		{
			BTProgressHUD2.ShowToast(message, showToastCentered, timeoutMs);
		}

		public static void ShowToast(string message, MaskType maskType, bool showToastCentered = true, double timeoutMs = 1000)
		{
			BTProgressHUD2.ShowToast(message, (ProgressHUD2.MaskType)maskType, showToastCentered, timeoutMs);
		}
	}
}