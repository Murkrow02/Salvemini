using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Linq;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif

namespace SalveminiApp.FlappyMimmo
{
    public partial class Classifica : ContentPage
    {
        public List<RestApi.Models.UtentiClassifica> Scores = new List<RestApi.Models.UtentiClassifica>();

        public Classifica()
        {
            InitializeComponent();

            //Modal presentation style
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
#endif
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Check connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            Scores = await App.Flappy.GetScores();
            if (Scores != null)
            {
                var removedScores = new List<RestApi.Models.UtentiClassifica>();
                removedScores.AddRange(Scores);

                //1 place user
                if (Scores.ElementAtOrDefault(0) != null)
                {
                    firstImage.Source = Scores[0].FullImmagine;
                    firstName.Text = Scores[0].NomeCognome.Remove(Scores[0].NomeCognome.IndexOf(' '));
                    firstScore.Text = Scores[0].Punteggio + "pt";
                    //Remove from total list
                    removedScores.RemoveAt(0);
                }

                //2 place user
                if (Scores.ElementAtOrDefault(1) != null)
                {
                    secondImage.Source = Scores[1].FullImmagine;
                    secondName.Text = Scores[1].NomeCognome.Remove(Scores[1].NomeCognome.IndexOf(' '));
                    secondScore.Text = Scores[1].Punteggio + "pt";
                    //Remove from total list
                    removedScores.RemoveAt(0);

                }

                //3 place user
                if (Scores.ElementAtOrDefault(2) != null)
                {
                    thirdImage.Source = Scores[2].FullImmagine;
                    thirdName.Text = Scores[2].NomeCognome.Remove(Scores[2].NomeCognome.IndexOf(' '));
                    thirdScore.Text = Scores[2].Punteggio + "pt";
                    //Remove from total list
                    removedScores.RemoveAt(0);
                }

                //Fill list
                scoreList.ItemsSource = removedScores;
            }
        }

        void Close_Tapped(object sender, EventArgs e)
        {
            //Close page
            canClose = false;
            Navigation.PopModalAsync();
        }

        bool canClose = true;
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //iOS 13 Bug
            try
            {
                if (canClose)
                {
                    Navigation.PopModalAsync();
                    canClose = false;
                }
            }
            catch
            {
                //fa nient
            }
        }
    }
}
