using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SalveminiApp.RestApi;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
namespace SalveminiApp.Controls
{
	public partial class Commento : ContentView
	{

		//Commento
		public static readonly BindableProperty CommentProperty = BindableProperty.Create(nameof(Comment), typeof(RestApi.Models.Commenti), typeof(Commento), default(RestApi.Models.Commenti), Xamarin.Forms.BindingMode.OneWay);
		public RestApi.Models.Commenti Comment
		{
			get
			{
				return (RestApi.Models.Commenti)GetValue(CommentProperty);
			}

			set
			{
				SetValue(CommentProperty, value);
			}
		}

		//NoPadding
		public static readonly BindableProperty NoPaddingProperty = BindableProperty.Create(nameof(NoPadding), typeof(bool), typeof(Commento), default(bool), Xamarin.Forms.BindingMode.OneWay);
		public bool NoPadding
		{
			get
			{
				return (bool)GetValue(NoPaddingProperty);
			}

			set
			{
				SetValue(NoPaddingProperty, value);
			}
		}

		public Commento()
		{
			InitializeComponent();

			var tapped = new TapGestureRecognizer();
			tapped.Tapped += Tapped_Tapped;
			this.GestureRecognizers.Add(tapped);
		}

		//Push to profile
		private async void Tapped_Tapped(object sender, EventArgs e)
		{
			var parentPage = this.GetParentPage();
			if (parentPage == null)
				return;

			if (Comment.Utenti != null)
			{
				iCringe.Commenti.Pushed = true;
				//Create push
				var modalPush = new SecondaryViews.ProfileView(Comment.Utenti);

				//Modal figo
#if __IOS__
                modalPush.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
				await parentPage.Navigation.PushModalAsync(modalPush);

				iCringe.Commenti.Pushed = false;
			}
		}

		//Update values
		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			try
			{
				//Title
				if (propertyName == CommentProperty.PropertyName)
				{
					userName.Text = Comment.UserName;
					userImg.Source = Comment.UserImage;
					commentLbl.Text = Comment.Commento;
				}

				//Title
				if (propertyName == NoPaddingProperty.PropertyName)
				{
					if (NoPadding)
						mainLayout.Margin = new Thickness(10, 2, 10, 2);
					else
						mainLayout.Margin = new Thickness(10);
				}
			}
			catch
			{
				//Boh per sicurezza a volte fa cose strane
				return;
			}
		}
	}
}
