using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class NewTransaction : ContentPage
    {
        public NewTransaction()
        {
            InitializeComponent();
        }

        void Plus_Clicked(System.Object sender, System.EventArgs e)
        {
            ((sender as Button).Parent as Frame).BackgroundColor = Color.FromHex("#007aff");
            ((((sender as Button).Parent as Frame).Parent as StackLayout).Children[1] as Frame).BackgroundColor = Styles.ObjectGray;
            if (!string.IsNullOrEmpty(ImportEntry.Text))
            {
                if (ImportEntry.Text[0] == '+' || ImportEntry.Text[0] == '-')
                {
                    ImportEntry.Text = "+" + ImportEntry.Text.Substring(1);
                }
                else
                {
                    ImportEntry.Text = "+" + ImportEntry.Text;
                }
            }
        }

        void Minus_Clicked(System.Object sender, System.EventArgs e)
        {
            ((sender as Button).Parent as Frame).BackgroundColor = Color.FromHex("#007aff");
            ((((sender as Button).Parent as Frame).Parent as StackLayout).Children[0] as Frame).BackgroundColor = Styles.ObjectGray;
            if (!string.IsNullOrEmpty(ImportEntry.Text))
            {
                if (ImportEntry.Text[0] == '+' || ImportEntry.Text[0] == '-')
                {
                    ImportEntry.Text = "-" + ImportEntry.Text.Substring(1);
                }
                else
                {
                    ImportEntry.Text = "-" + ImportEntry.Text;
                }
            }
        }

        void ImportEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                if (PlusFrame.BackgroundColor == Color.FromHex("#007aff"))
                {
                    ImportEntry.Text = "+";
                }
                else
                {
                    ImportEntry.Text = "-";
                }
            }

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (!e.NewTextValue.Contains("+") && !e.NewTextValue.Contains("-"))
                {
                    if (PlusFrame.BackgroundColor == Color.FromHex("#007aff"))
                    {
                        ImportEntry.Text = "+" + ImportEntry.Text.Substring(1);
                    }
                    else
                    {
                        ImportEntry.Text = "-" + ImportEntry.Text.Substring(1);
                    }
                }
            }

        }

        async void Publish_Clicked(System.Object sender, System.EventArgs e)
        {
            var model = new RestApi.Models.FondoStudentesco();
            if (!string.IsNullOrEmpty(ImportEntry.Text))
            {
                if (!ImportEntry.Text.Contains("+") || !ImportEntry.Text.Contains("-"))
                {
                    if (PlusFrame.BackgroundColor == Color.FromHex("#007aff"))
                    {
                        ImportEntry.Text = "+" + ImportEntry.Text.Substring(1);
                    }
                    else
                    {
                        ImportEntry.Text = "-" + ImportEntry.Text.Substring(1);
                    }
                }

                if (ImportEntry.Text.Length > 1)
                {
                    try
                    {
                        model.Importo = Convert.ToDecimal(ImportEntry.Text);
                    }
                    catch
                    {
                        await DisplayAlert("Errore", "Inserisci un importo valido", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Errore", "Inserisci un importo valido", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Errore", "Inserisci un importo valido", "Ok");
            }

            if (!string.IsNullOrEmpty(ReasonEntry.Text))
            {
                model.Causa = ReasonEntry.Text;
            }
            else
            {
                await DisplayAlert("Errore", "Inserisci un motivo valido per la transazione", "Ok");
            }

            var success = await App.Fondo.PublishTransaction(model);

            if (!success)
            {
                await DisplayAlert("Errore", "Si è verificato un errore", "Ok");
            }
            else
            {
                await DisplayAlert("Successo", "La transazione è stata pubblicata con successo", "Ok");
                await Navigation.PopAsync();
            }
        }
    }
}
