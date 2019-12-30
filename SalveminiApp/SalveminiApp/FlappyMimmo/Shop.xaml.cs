using System;
using System.Net;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Extensions;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SalveminiApp.FlappyMimmo
{
    public partial class Shop : PopupPage
    {
        public ObservableCollection<RestApi.Models.FlappySkinReturn> Skins = new ObservableCollection<RestApi.Models.FlappySkinReturn>();
        public RestApi.Models.FlappyMoneteReturn Potenziamento = new RestApi.Models.FlappyMoneteReturn();

        public Shop()
        {
            InitializeComponent();
        }

      

        private async void Skin_Selected(object sender, SelectedItemChangedEventArgs e)
        {
            //Check connection
            if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            var data = e.SelectedItem as RestApi.Models.FlappySkinReturn;
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!data.Comprata)
            {
                string buy = await App.Flappy.BuySkin(data.id);
                if (buy != null)
                {
                    //Failed
                    Costants.showToast(buy);
                }
                else
                {
                    //Success
                    //Download image
                    isLoading = true;
                    for (int i = 0; i < 3; i++)
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(data.FullImmagini[i], Path.Combine(documentsPath, data.Immagini[i]) + ".png");
                        }
                    }

                    Skins[Skins.IndexOf(data)].Comprata = true;
                    skinsList.ItemsSource = null;
                    skinsList.ItemsSource = Skins;
                    isLoading = false;
                }
            }
            else
            {
                if (!File.Exists(Path.Combine(documentsPath, data.Immagini[0] + ".png")) || !File.Exists(Path.Combine(documentsPath, data.Immagini[1] + ".png")) || !File.Exists(Path.Combine(documentsPath, data.Immagini[2] + ".png")))
                {
                    //Download skin if is not downloaded
                    isLoading = true;

                    for (int i = 0; i < 3; i++)
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(data.FullImmagini[i], Path.Combine(documentsPath, data.Immagini[i]) + ".png");
                        }
                    }
                    isLoading = false;

                }

                Preferences.Set("flappySkin", data.Immagini[0].Remove(data.Immagini[0].Length - 1));
                var previous = Skins.FirstOrDefault(x => x.isSelected);
                Skins[Skins.IndexOf(data)].isSelected = true;
                Skins[Skins.IndexOf(data)].BgColor = Color.FromHex("#5A99FF");
                Skins[Skins.IndexOf(previous)].isSelected = false;
                Skins[Skins.IndexOf(previous)].BgColor = Color.White;
                skinsList.ItemsSource = null;
                skinsList.ItemsSource = Skins;
                //skinsList.ItemsSource = skin
                //await Navigation.PopPopupAsync();
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Check connection
            if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            //Get Skins
            Skins = await App.Flappy.GetSkins();
            if (Skins != null)
            {
                skinsList.ItemsSource = Skins;
            }

            //Get current multiplier
            Potenziamento = await App.Flappy.GetUpgrade();
            if (Potenziamento != null)
            {
                //Setup interface
                multiplierValue.Text = "x" + Potenziamento.Valore;
                multiplierPrice.Text = Potenziamento.Costo.ToString();
            }
            else
            {
                //No powerups avaiable
                priceFrame.IsVisible = false;
                multiplierValue.IsVisible = false;
                powerLabel.Text = "Hai comprato tutti i potenziamenti";
            }
        }

        async void BuyMultiplier_Tapped(object sender, EventArgs e)
        {
            //Check connection
            if (Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            if (Potenziamento != null)
            {
                //Buy new power up
                string buy = await App.Flappy.Upgrade(Potenziamento.id);
                if (buy != null)
                {
                    //Failed
                    Costants.showToast(buy);
                }
                else
                {
                    //Get new power up
                    Potenziamento = await App.Flappy.GetUpgrade();
                    if (Potenziamento != null)
                    {
                        //Setup interface
                        multiplierValue.Text = "x" + Potenziamento.Valore;
                        multiplierPrice.Text = Potenziamento.Costo.ToString();
                    }
                    else
                    {
                        //No powerups avaiable
                        priceFrame.IsVisible = false;
                        multiplierValue.IsVisible = false;
                        powerLabel.Text = "Hai comprato tutti i potenziamenti";
                    }
                }
            }
        }

        bool isLoading = false;
        void Close_Tapped(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                Navigation.PopPopupAsync();
            }
        }
    }
}
