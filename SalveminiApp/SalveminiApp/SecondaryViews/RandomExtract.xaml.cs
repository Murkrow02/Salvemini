using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using Rg.Plugins.Popup.Extensions;
using MonkeyCache.SQLite;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
namespace SalveminiApp.SecondaryViews
{
    public partial class RandomExtract : ContentPage
    {

        List<ExtractUser> utenti = new List<ExtractUser>();


        public RandomExtract()
        {
            InitializeComponent();

            //Safe area on ios
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
#endif
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //Show loading
            utentiListCtrl.IsRefreshing = true;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //Download users
                var utentiList = await App.Utenti.GetUtenti(true);

                if (utentiList.Count < 1)
                {
                    bool decision = await DisplayAlert("Attenzione", "Non è stato possibile caricare gli utenti", "Riprova", "Annulla");
                    if (decision)
                        OnAppearing();
                    else
                    {
                        await Navigation.PopModalAsync();
                        return;
                    }
                }
                utentiList.OrderBy(x => x.Nome);

                //Create simple list
                utenti.Clear();
                foreach (var utente in utentiList) { utenti.Add(new ExtractUser { Nome = utente.cognomeNome, Immagine = utente.Immagine }); }
                //Get custom from cache
                var utentiSalvati = CacheHelper.GetCache<List<ExtractUser>>("extractCustomUsers");
                if (utentiSalvati != null)
                    foreach (var utente in utentiSalvati) { utenti.Add(new ExtractUser { Nome = utente.Nome }); }
                UpdateList();
                startBtn.IsEnabled = true;
            }
            else
            {
                await DisplayAlert("Attenzione", "Non sei connesso ad internet!", "Ok");
                await Navigation.PopModalAsync();
            }

            //Stop loading
            utentiListCtrl.IsRefreshing = false;
        }

        //Add to list
        public void addUser_Clicked(object sender, EventArgs e)
        {
            //If null return
            if (string.IsNullOrEmpty(customUser.Text))
                return;

            //Insert new user at top
            var newUser = new ExtractUser { Nome = customUser.Text };
            utenti.Insert(0, newUser);
            UpdateList();

            //Add user custom to cache
            var utentiSalvati = CacheHelper.GetCache<List<ExtractUser>>("extractCustomUsers");
            if (utentiSalvati == null) utentiSalvati = new List<ExtractUser>();
            utentiSalvati.Add(newUser);
            Barrel.Current.Add<List<ExtractUser>>("extractCustomUsers", utentiSalvati, TimeSpan.FromDays(1000));

            //Clear text
            customUser.Text = "";
        }

        //Remove from list
        async void userSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //Get user selcted
            var data = (sender as Xamarin.Forms.ListView).SelectedItem as ExtractUser;

            //Deselect cell
            if (e.SelectedItem == null)
                return;
            utentiListCtrl.SelectedItem = null;

            //Remove from list
            bool confirm = await DisplayAlert("Sicuro?", "Vuoi rimuovere questo utente?", "Si", "No");
            if (confirm) utenti.Remove(data);
            UpdateList();


            //Remove user custom from cache
            var utentiSalvati = CacheHelper.GetCache<List<ExtractUser>>("extractCustomUsers");
            if (utentiSalvati == null) return;
            utentiSalvati.RemoveAll(x => x.Nome == data.Nome);
            Barrel.Current.Add<List<ExtractUser>>("extractCustomUsers", utentiSalvati, TimeSpan.FromDays(1000));

        }


        public void UpdateList()
        {
            //Update list and count
            utentiListCtrl.ItemsSource = null;
            utentiListCtrl.ItemsSource = utenti.OrderBy(x => x.Nome);
            utentiCountLbl.Text = utenti.Count.ToString();
        }

        public void start_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ExtractingPage(utenti));
        }

    }

    public class ExtractUser
    {
        public string Nome { get; set; }
        public string Immagine { get; set; }
    }
}
