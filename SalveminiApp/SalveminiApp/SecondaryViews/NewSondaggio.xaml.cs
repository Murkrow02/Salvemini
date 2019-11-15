using System;
using System.Collections.Generic;
using SalveminiApp.RestApi.Models;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNet.SignalR.Client;
using System.Linq;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using UIKit;
#endif
namespace SalveminiApp.SecondaryViews
{
    public partial class NewSondaggio : ContentPage
    {
        //Save the selected poll
        public Sondaggi sondaggio = new Sondaggi();

        //Save poll results
        public List<SondaggiResult> Risultati = new List<SondaggiResult>();

        //Save signalR connection class
        SignalR.SondaggiHub connection = new SignalR.SondaggiHub();

        //Save if frame voti is visible
        bool showingResults = false;

        public NewSondaggio(Sondaggi sondaggio_)
        {
            InitializeComponent();

            //Safe area
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && !iOS.AppDelegate.HasNotch)
            {
                mainLayout.Padding = new Thickness(0, 20, 0, 0);
            }
#endif

            //Fill sondaggio values
            sondaggio = sondaggio_;

            //Save that user viewed last sondaggio
           // Preferences.Set("LastSondaggio", sondaggio.id);

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
        }

        private async void VotaBtn_Clicked(object sender, EventArgs e)
        {
            //Disable votes
            widgetCollection.IsEnabled = false;
            widgetCollection.Opacity = 0.6;
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

                    //Remove vote btn
                    try
                    {
                       var layouts = widgetsLayout.Children.ToList();
                        foreach(var stack in layouts)
                        {
                           var stack_ = stack as StackLayout;
                            try {
                                var buttonVote = stack_.Children[1] as Button;
                                stack_.Children.Remove(buttonVote);
                            }
                            catch { continue; }
                        }
                    }
                    catch { }

                    //Show results
                    showResults();
                }
            }
            catch
            {
                await DisplayAlert("Errore", "Non è stato possibile inviare il tuo voto, riprova più tardi", "Ok");
            }

            //Enable votes
            widgetCollection.IsEnabled = true;
            widgetCollection.Opacity = 1;
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
                Preferences.Set("skipPoll" + sondaggio.id, true);
                await Navigation.PopModalAsync();

            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Status bar color
#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            }
#endif

            //Already voted, show results
            if (Preferences.Get("voted" + sondaggio.id, false))
            {
                showResults();
            }

            //Connect to SignalR hub
            connection.InitilizeHub();

            //Handle new voto
            connection.hubProxy.On("UpdateVoti", () =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (showingResults)
                            showResults();
                    });
                });
        }

        public async void showResults()
        {
            //Show loading label
            if(!showingResults)
            await loadingLbl.FadeTo(1,200);

            //Get new results
            Risultati = await App.Sondaggi.ReturnRisultati(sondaggio.id);

            //Return if no voti
            if (Risultati == null || Risultati.Count < 1)
                return;

            //Show label
            headerDesc.Text = "I risultati sono aggiornati in tempo reale";

            //Remove previous results
            resultsLayout.Children.Clear();

            //Foreach result add a custom control
            for (int i = 0; i < Risultati.Count; i++)
            {
                //If 0 votes hide bar
                string hideVotes = "no";
                if (Risultati[i].Voti == 0)
                    hideVotes = "si";

                resultsLayout.Children.Add(new Controls.PercentageBar { Title = "<p><strong><span>" + Risultati[i].NomeOpzione + ":</span>&nbsp;</strong><span>" + Risultati[i].Voti + " voti</span></p>", HideVotes = hideVotes, Percentage = Risultati[i].Percentuale, BgColor = Costants.Colors[i].Replace("#", "") });
            }

            //Hide loading label
            if(!showingResults)
                await loadingLbl.FadeTo(0, 200);

            //Show frame
            if (!showingResults)
            await Task.WhenAll(resultsFrame.FadeTo(1, 1000, Easing.CubicInOut), resultsFrame.TranslateTo(0, 0, 1500, Easing.CubicOut));
            showingResults = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

#if __IOS__
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
#endif

            //Disconnect from SignalR
            connection.Disconnect();
            connection = new SignalR.SondaggiHub();

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
