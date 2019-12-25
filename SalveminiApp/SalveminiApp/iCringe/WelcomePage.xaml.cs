using System;
using System.Collections.Generic;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Xamarin.Essentials;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#endif
using Xamarin.Forms;

namespace SalveminiApp.iCringe
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();

            privacyLabel.Text = "\niCringe è un social interamente basato sull’anonimato degli utenti. \n Una volta creata una domanda nessun utente potrà sapere che sei stato tu a farla e non potranno risalire a te in nessun modo. \n \n Le domande verranno approvate dai VIP prima di essere pubblicate, che comunque NON potranno risalire al creatore della domanda nel caso in cui sia stata creata in forma anonima. \n \n I creatori dell’app (Marco Coppola e Valerio de Nicola) non si assumono in nessun modo la responsabilità dei contenuti postati in questa piattaforma, che verranno controllati assiduamente e rimossi in caso di contenuti inappropriati. \n \n Uno spam continuo di post o commenti può portare ad una sospensione del tuo account (gestita automaticamente dal nostro server). \n \n Non verranno approvati contenuti ritenuti disinformanti, gravemente offensivi o di spam.";

            //Set Safearea
#if __IOS__
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
#endif
        }

        public void close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Subscribe to push
            OneSignal.Current.SendTag("Secrets", Preferences.Get("UserId", 0).ToString());
            Preferences.Set("iCringePush", true);

            //non pushare piu qua
            Preferences.Set("firstTimeCringe", false);

        }
    }
}
