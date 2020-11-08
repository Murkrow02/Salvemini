using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SalveminiApp.SecondaryViews
{
    public partial class ProfileView : ContentPage
    {
        public RestApi.Models.Utenti utente;

        public ProfileView(RestApi.Models.Utenti utente_)
        {
            InitializeComponent();

            //Set colors
            TimetableExpandButtonContainer.BackgroundColor = Color.FromRgba(0, 0, 0, 120);

            //Load initial interface
            userInfo.User = utente_;
            utente = utente_;

            //Set formsheet
            DependencyService.Get<IPlatformSpecific>().SetFormSheet(this);

            DependencyService.Get<IPlatformSpecific>().SetSafeArea(this);
            if (DependencyService.Get<IPlatformSpecific>().HasBottomBar())
                mainLayout.Padding = new Thickness(25, 20, 25, 0);


            //compleannoLbl.Text = utente.Compleanno.ToString("dd MMMM");
            //comuneLbl.Text = utente.Residenza;
            if(utente.Stato > 0)
            {
                roleLayout.IsVisible = true;
                switch (utente.Stato)
                {
                    case 1:roleLbl.Text = "Studente";break;
                    case 2:roleLbl.Text = "Rappresentante d'istituto";break;
                    case 3:roleLbl.Text = "CEO della SalveminiApp"; break;
                }
            }
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Run((Action)getOrario);
        }

        void closePage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        string TimeTableUrl = "";
        void getOrario()
        {
            //Get stored class
            var classe = utente.Classe + utente.Corso;

            //Add final m for classes of 6 chars
            if (classe.Substring(classe.Length - 2).ToLower() == "ca")
            {
                classe = classe.Remove(classe.Length - 2) + "cam";
            }
            //Low "cam" string due case sensitive website
            if (classe.Contains("CAM"))
            {
                classe = classe.Remove(classe.Length - 3) + "cam";
            }

            //Bool for last timetable found
            bool found = false;
            for (int p = 6; p < 20; p++)
            {
                try
                {
                    //Try getting result from link
                    System.Net.WebRequest request = System.Net.WebRequest.Create($"https://www.salvemini.edu.it/orario/2020_21/p{p}/Classi/{classe}.jpg");
                    //Only get <Head> tag for low data request
                    request.Method = "HEAD";
                    request.GetResponse();
                    //Reach this only if website is found
                    found = true;
                }
                catch
                {
                    //First not found
                    if (found)
                    {
                        //Set final url
                        TimeTableUrl = $"https://www.salvemini.edu.it/orario/2020_21/p{p - 1}/Classi/{classe}.jpg";
                        break;
                    }
                }
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                //Check if there is a timetable
                if (string.IsNullOrEmpty(TimeTableUrl))
                {
                    //No => show placeholder
                    NoTimetableLayout.IsVisible = true;
                    TimetableImage.IsVisible = false;
                    TimetableButtonsLayout.IsVisible = false;
                    return;
                }
                else
                {
                    //Yes => Hide placeholder
                    NoTimetableLayout.IsVisible = false;
                    TimetableImage.IsVisible = true;
                    TimetableButtonsLayout.IsVisible = true;
                }

                //Set timetable image source
                TimetableImage.Source = TimeTableUrl;
            });
        }

        void TimetableExpandButtonContainer_Clicked(System.Object sender, System.EventArgs e)
        {
            //Create stormlion image list
            var imageList = new List<PhotoBrowser.Photo>();
            imageList.Add(new PhotoBrowser.Photo { Title = "Orario", URL = TimeTableUrl });
            var imageViewer = new PhotoBrowser.PhotoBrowser();
            imageViewer.Photos = imageList;
            //Display timetable
            imageViewer.Show();
        }
    }
}
