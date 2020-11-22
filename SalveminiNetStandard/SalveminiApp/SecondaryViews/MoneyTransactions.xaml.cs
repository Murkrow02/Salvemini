using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class MoneyTransactions : ContentPage
    {
        public List<RestApi.Models.FondoStudentesco> Transactions = new List<RestApi.Models.FondoStudentesco>();

        public MoneyTransactions(string balanceValue)
        {
            InitializeComponent();

            //Set formsheet
            DependencyService.Get<IPlatformSpecific>().SetFormSheet(this);

            BalanceLabel.Text = balanceValue;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }

            Transactions = await App.Fondo.GetTransactions();

            if (Transactions != null)
            {
                if (Transactions.Count > 0)
                {
                    HistoryList.ItemsSource = Transactions;

                    HistoryList.IsVisible = true;
                    PlaceholderLayout.IsVisible = false;
                }
                else
                {
                    HistoryList.IsVisible = false;
                    PlaceholderLayout.IsVisible = true;
                }
            }
            else
            {
                Costants.showToast("Si è verificato un errore");
            }
        }

        void HistoryList_Refreshing(System.Object sender, System.EventArgs e)
        {
            OnAppearing();
        }

        void IconButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
