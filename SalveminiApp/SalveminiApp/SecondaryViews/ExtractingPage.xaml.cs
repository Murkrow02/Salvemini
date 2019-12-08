using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
namespace SalveminiApp.SecondaryViews
{
    public partial class ExtractingPage : ContentPage
    {
        public List<ExtractUser> daEstrarre = new List<ExtractUser>();
        public List<ExtractUser> Utenti = new List<ExtractUser>();
        public List<ExtractUser> Estratti = new List<ExtractUser>();
        public ExtractingPage(List<ExtractUser> utenti)
        {
            InitializeComponent();

            //Get utenti
            Utenti = utenti;
            daEstrarre.AddRange(Utenti);

            userImg.WidthRequest = App.ScreenWidth / 3;
            userImg.HeightRequest = App.ScreenWidth / 3;
        }



        public async void extract_Clicked(object sender, EventArgs e)
        {
            //Disable button
            var bottone = sender as Button;
            bottone.IsEnabled = false;

            //Restart
            if(bottone.Text == "Ricomincia")
            {
                bottone.Text = "Estrai";
                daEstrarre.AddRange(Utenti);
                Estratti.Clear();
            }

            //Finished
            if (daEstrarre.Count < 1)
            {
                bottone.Text = "Ricomincia";
                toExtract.Text = "Hai estratto tutti";
                return;
            }

            //Extract random user
            var random = new Random();
            var extracted = random.Next(0, daEstrarre.Count - 1);
            var extractedUser = daEstrarre[extracted];

            //Delay
            var tempo = (uint)(TimeSpan.FromSeconds((double)1 / (double)daEstrarre.Count).Milliseconds);

            //Shuffle animation
            var shuffleList = new List<ExtractUser>();
            if (daEstrarre.Count > 7) shuffleList.AddRange(daEstrarre.Take(7)); else shuffleList.AddRange(daEstrarre);
            foreach (var utente in shuffleList)
            {
                await Task.WhenAll(nameLbl.TranslateTo(App.ScreenWidth, 0, tempo), nameLbl.FadeTo(0, tempo));
                nameLbl.TranslationX = -App.ScreenWidth;
                userImg.Source = utente.Immagine;
                nameLbl.Text = utente.Nome;
                await Task.WhenAll(nameLbl.TranslateTo(0, 0, tempo), nameLbl.FadeTo(1, tempo));

            }

            //Update with the right one
            await Task.WhenAll(nameLbl.TranslateTo(App.ScreenWidth, 0, tempo), nameLbl.FadeTo(0, tempo));
            nameLbl.TranslationX = -App.ScreenWidth * 0.8;
            userImg.Source = extractedUser.Immagine;
            nameLbl.Text = extractedUser.Nome;
            await Task.WhenAll(nameLbl.TranslateTo(0, 0, 800, Easing.CubicOut), nameLbl.FadeTo(1, 800,Easing.CubicOut));

            //Save extracted user
            Estratti.Insert(0, extractedUser); utentiListCtrl.ItemsSource = null; utentiListCtrl.ItemsSource = Estratti;

            //Update count
            if (removeSwitch.IsChecked)
            {
                toExtract.Text = "Utenti estratti:" + Estratti.Count + "/" + Utenti.Count;
                daEstrarre.RemoveAt(extracted);
            }

            //Enable button
            (sender as Button).IsEnabled = true;

        }

        public void switch_Toggled(object sender, CheckedChangedEventArgs e)
        {
            if (!removeSwitch.IsChecked) //Remove from list
            {
                daEstrarre.Clear();
                daEstrarre.AddRange(Utenti);
                toExtract.Text = "Utenti estratti:";
            }
            else
            {
                daEstrarre.Clear();
                daEstrarre.AddRange(Utenti);
            }
        }
    }
}
