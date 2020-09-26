using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SalveminiApp.PhotoBrowser
{
	public class PhotoBrowser
	{
		public List<Photo> Photos { get; set; }

		public Action<int> ActionButtonPressed { get; set; }
		public Action<int> DidDisplayPhoto { get; set; }

		public int StartIndex { get; set; } = 0;

		public bool EnableGrid { get; set; }

		public void Show()
		{
			DependencyService.Get<IPhotoBrowser>().Show(this);
		}

		public static void Close()
		{
			DependencyService.Get<IPhotoBrowser>().Close();
		}

		Color? _BackgroundColor;
		public Color BackgroundColor
		{
			get => _BackgroundColor ?? Color.Black;
			set => _BackgroundColor = value;
		}


		public int Android_ContainerPaddingPx = 0;

		public bool iOS_ZoomPhotosToFill = true;
	}

    public interface IPhotoBrowser
    {
        void Show(PhotoBrowser photoBrowser);

        void Close();
    }
    public class Photo
    {
        public string URL { get; set; }

        public string Title { get; set; }
    }
}