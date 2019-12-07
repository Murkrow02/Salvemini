using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.Controls
{
    public partial class UserImageNameClass : ContentView
    {
        public UserImageNameClass()
        {
            InitializeComponent();

            //Set dimensions
            userImg.WidthRequest = App.ScreenWidth / 3.5;
        }

        //User
        public static readonly BindableProperty UserProperty = BindableProperty.Create(nameof(User), typeof(RestApi.Models.Utente), typeof(UserImageNameClass), default(RestApi.Models.Utente), Xamarin.Forms.BindingMode.OneWay);
        public RestApi.Models.Utente User
        {
            get
            {
                return (RestApi.Models.Utente)GetValue(UserProperty);
            }

            set
            {
                SetValue(UserProperty, value);
            }
        }

        //Cached user
        public static readonly BindableProperty CachedUserIdProperty = BindableProperty.Create(nameof(CachedUserId), typeof(int), typeof(UserImageNameClass), default(int), Xamarin.Forms.BindingMode.OneWay);
        public int CachedUserId
        {
            get
            {
                return (int)GetValue(CachedUserIdProperty);
            }

            set
            {
                SetValue(CachedUserIdProperty, value);
            }
        }

        //Update values
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            try
            {
                //Title
                if (propertyName == UserProperty.PropertyName)
                {
                    if (User != null)
                    {
                        //Display name image and class
                        nameLbl.Text = User.nomeCognome;
                        classLbl.Text = User.classeCorso;
                        userImg.Source = User.Immagine;
                    }
                }

                //Get cached
                if (propertyName == CachedUserIdProperty.PropertyName)
                {
                    var cachedUtente = CacheHelper.GetCache<RestApi.Models.Utente>("utente" + CachedUserId);
                    if (cachedUtente != null)
                    {
                        nameLbl.Text = cachedUtente.nomeCognome;
                        classLbl.Text = cachedUtente.classeCorso;
                        userImg.Source = cachedUtente.Immagine;
                    }

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
