using System;
using System.Collections.Generic;
using SalveminiApp.RestApi.Models;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
namespace SalveminiApp.SecondaryViews
{
    public partial class NewSondaggio : ContentPage
    {
        public Sondaggi sondaggio = new Sondaggi();
        public List<SondaggiResult> Risultati = new List<SondaggiResult>(); 

        public NewSondaggio(Sondaggi sondaggio_)
        {
            InitializeComponent();

            //Safe area
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
#endif

            //Fill sondaggio values
            sondaggio = sondaggio_;

            //Save that user viewed last sondaggio
            Preferences.Set("LastSondaggio", sondaggio.id);

            //Add initial space
            widgetsLayout.Children.Add(new Xamarin.Forms.ContentView { WidthRequest = 5 });

            //Fill layout with sondaggio options
            foreach (var opzione in sondaggio.OggettiSondaggi)
            {
                //Create new layout to add a vote button under the option
                var layout = new StackLayout { Spacing = 2, VerticalOptions = LayoutOptions.FillAndExpand };
                var votaBtn = new Button { Text = "Vota", BackgroundColor = Color.Transparent, FontSize = 15, Margin = 0, VerticalOptions = LayoutOptions.Start };
                votaBtn.Clicked += VotaBtn_Clicked;

                //Detect images
                string image = "no";
                if (!string.IsNullOrEmpty(opzione.Immagine))
                    image = opzione.FullImmagine;

                //Create new poll option
                var widget = new Helpers.PollOption { Title = opzione.Nome, Image = image, optionId = opzione.id, VerticalOptions = LayoutOptions.CenterAndExpand };
                layout.Children.Add(widget);

                //Already voted, hide vote btn
                if (!Preferences.Get("voted" + sondaggio.id, false))
                    layout.Children.Add(votaBtn);

                widgetsLayout.Children.Add(layout);
            }

            //Add final space
            widgetsLayout.Children.Add(new Xamarin.Forms.ContentView { WidthRequest = 5 });

            //Set question label and creation label
            questionLbl.Text = sondaggio.Nome;
            creatorLbl.Text = "Creato da " + sondaggio.Utenti.nomeCognome;

            //Subscribe to messaging center
            MessagingCenter.Subscribe<App>(this, "updateResults", (sender) =>
            {
                showResults();
            });

        }

        private async void VotaBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Get id scelta
                var button = sender as Button; var layout = button.Parent as StackLayout; var widget = layout.Children[0] as Helpers.PollOption;
                var scelta = widget.optionId;

                //Create new voto with id scelta
                var voto = new VotoSondaggio { idSondaggio = sondaggio.id, Utente = Preferences.Get("UserId", 0), Voto = scelta };

                //post the voto
                var response = await App.Sondaggi.PostVoto(voto);

                //Notify the user success or failure
                await DisplayAlert(response[0], response[1], "Ok");

                //Success
                if (response[0] == "Grazie!")
                {
                    Preferences.Set("voted" + sondaggio.id, true);
                    MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "RemoveBadge", "Sondaggi");

                    //Show results
                    showResults();
                }
            }
            catch
            {
                await DisplayAlert("Errore", "Non è stato possibile inviare il tuo voto, riprova più tardi", "Ok");
            }
        }

        async void closePage(object sender, EventArgs e)
        {
            //Already voted
            if (Preferences.Get("voted" + sondaggio.id, false))
            {
                await Navigation.PopModalAsync();
                return;
            }

            //Not voted
            bool choice = await DisplayAlert("Aspetta!", "La tua opinione è importante, sei sicuro di non voler votare? Se cambi idea puoi sempre tornare su questa pagina cliccando la sezione 'Sondaggi' alla home!", "Ho cambiato idea", "No mi scoccio");
            if (choice) //He chenged his mind
                return;
            else //He decided not to vote :(
            {
                Preferences.Set("voted" + sondaggio.id, true);
                await Navigation.PopModalAsync();

            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Already voted, show results
            if (Preferences.Get("voted" + sondaggio.id, false))
            {
                showResults();
            }

        }

        public async void showResults(bool fromSignalR = false)
        {
            //Get new results
            if(!fromSignalR)
            Risultati = await App.Sondaggi.ReturnRisultati(sondaggio.id);

            //Remove previous results
            resultsLayout.Children.Clear();

            //Return if no voti
            if (Risultati == null || Risultati.Count < 1)
                return;

            //Foreach result add a custom control
            for (int i = 0; i < Risultati.Count; i++)
            {
                //If 0 votes hide bar
                string hideVotes = "no";
                if (Risultati[i].Voti == 0)
                    hideVotes = "si";

                resultsLayout.Children.Add(new Controls.PercentageBar { Title = "<p><strong><span>" + Risultati[i].NomeOpzione + ":</span>&nbsp;</strong><span>" + Risultati[i].Voti + " voti</span></p>", HideVotes = hideVotes, Percentage = Risultati[i].Percentuale, BgColor = Costants.Colors[i].Replace("#", "") });
            }

            //Show frame
            await Task.WhenAll(resultsFrame.FadeTo(1, 1000, Easing.CubicInOut), resultsFrame.TranslateTo(0,0,1500, Easing.CubicOut));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Ios 13 bug
            try
            {
                Navigation.PopModalAsync();
            }
            catch
            {
                //fa nient
            }

        }
    }
}
