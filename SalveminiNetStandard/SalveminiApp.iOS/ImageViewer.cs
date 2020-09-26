using Ricardo.LibMWPhotoBrowser.iOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace SalveminiApp.iOS
{
	public class MyMWPhotoBrower : MWPhotoBrowserDelegate
	{
		protected SalveminiApp.PhotoBrowser.PhotoBrowser _photoBrowser;

		protected List<MWPhoto> _photos = new List<MWPhoto>();

		public MyMWPhotoBrower(SalveminiApp.PhotoBrowser.PhotoBrowser photoBrowser)
		{
			_photoBrowser = photoBrowser;
		}

		public void Show()
		{
			_photos = new List<MWPhoto>();

			foreach (SalveminiApp.PhotoBrowser.Photo p in _photoBrowser.Photos)
			{
				MWPhoto mp = MWPhoto.FromUrl(new Foundation.NSUrl(p.URL));

				if (!string.IsNullOrWhiteSpace(p.Title))
					mp.Caption = p.Title;

				_photos.Add(mp);
			}

			MWPhotoBrowser browser = new MWPhotoBrowser(this)
			{
				EnableGrid = _photoBrowser.EnableGrid,

				BrowserBackgroundColor = _photoBrowser.BackgroundColor.ToUIColor(),

				DisplayActionButton = _photoBrowser.ActionButtonPressed != null,

				ZoomPhotosToFill = _photoBrowser.iOS_ZoomPhotosToFill

			};


			browser.SetCurrentPhoto((nuint)_photoBrowser.StartIndex);


			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;
			while (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
			}

			vc.PresentModalViewController(new UINavigationController(browser), true);
		}

		public override MWPhoto GetPhoto(MWPhotoBrowser photoBrowser, nuint index) => _photos[(int)index];

		public override nuint NumberOfPhotosInPhotoBrowser(MWPhotoBrowser photoBrowser) => (nuint)_photos.Count;


		public override void OnActionButtonPressed(MWPhotoBrowser photoBrowser, nuint index)
		{
			_photoBrowser.ActionButtonPressed?.Invoke((int)index);
		}

		public override void DidDisplayPhoto(MWPhotoBrowser photoBrowser, nuint index)
		{
			_photoBrowser.DidDisplayPhoto?.Invoke((int)index);
		}


		public void Close()
		{
			UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
		}
	}

    public class PhotoBrowserImplementation : SalveminiApp.PhotoBrowser.IPhotoBrowser
    {
        protected static MyMWPhotoBrower _mainBrowser;

        public void Show(SalveminiApp.PhotoBrowser.PhotoBrowser photoBrowser)
        {
            _mainBrowser = new MyMWPhotoBrower(photoBrowser);
            _mainBrowser.Show();
        }

        public void Close()
        {
            if (_mainBrowser != null)
            {
                _mainBrowser.Close();
                _mainBrowser = null;
            }
        }
    }
}