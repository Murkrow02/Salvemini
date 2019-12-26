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

        //Hide Trash
        public static readonly BindableProperty HideTrashProperty = BindableProperty.Create(nameof(HideTrash), typeof(bool), typeof(Commento), default(bool), Xamarin.Forms.BindingMode.OneWay);
        public bool HideTrash
        {
            get
            {
                return (bool)GetValue(HideTrashProperty);
            }

            set
            {
                SetValue(HideTrashProperty, value);
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

        public async void deleteComment_Clicked(object sender, EventArgs e)
        {
            //Conferma
            bool confirm = await this.GetParentPage().DisplayAlert("Sei sicuro?", "Vuoi eliminare il tuo commento da questo post?", "Elimina", "Annulla");
            if (!confirm)
                return;

            //Elimina
            var successo = await App.Cringe.DeleteCommento(Comment.id);
            if(successo[0] == "Successo")
            {
                //Refresh list
                MessagingCenter.Send<App,RestApi.Models.Commenti>((App)Xamarin.Forms.Application.Current, "removeCommento", Comment);
            }
            Costants.showToast(successo[1]);
        }

        //Update values
        protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			try
			{
				//Commento
				if (propertyName == CommentProperty.PropertyName)
				{
					userName.Text = Comment.UserName;
					userImg.Source = Comment.UserImage;
					commentLbl.Text = Comment.Commento;
                    elapsed.Text = Costants.SpanString(DateTime.Now - Comment.Creazione);

                }

				//No padding
				if (propertyName == NoPaddingProperty.PropertyName)
				{
					if (NoPadding)
						mainLayout.Margin = new Thickness(10, 2, 10, 2);
					else
						mainLayout.Margin = new Thickness(10);
				}

                //Hide trash
                if (propertyName == HideTrashProperty.PropertyName)
                {
                    if (HideTrash)
                        trash.IsVisible = false;
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
