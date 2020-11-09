using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.Controls
{
    public partial class OrarioFoto : ContentView
    {
        public OrarioFoto()
        {
            InitializeComponent();

            //Set colors
            TimetableExpandButtonContainer.BackgroundColor = Color.FromRgba(0, 0, 0, 120);
            TimetableDownloadButtonContainer.BackgroundColor = Color.FromRgba(0, 0, 0, 120);

        }



        public static readonly BindableProperty ClasseCorsoProperty = BindableProperty.Create(nameof(ClasseCorso), typeof(string), typeof(Orario), default(string), Xamarin.Forms.BindingMode.TwoWay);
        public string ClasseCorso
        {
            get
            {
                return (string)GetValue(ClasseCorsoProperty);
            }

            set
            {
                SetValue(ClasseCorsoProperty, value);
            }
        }


        //Update values
        protected override async void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            try
            {
                if (propertyName == ClasseCorsoProperty.PropertyName)
                {
                    myClass = ClasseCorso == Preferences.Get("Classe", 0).ToString() + Preferences.Get("Corso", "");

                    //Hide download button if not my class and fast load image
                    if (myClass && !string.IsNullOrEmpty(Preferences.Get("TimeTableUrl", "")))
                    {
                        TimetableDownloadButtonContainer.IsVisible = true;

                        TimetableImage.Source = Preferences.Get("TimeTableUrl", "");

                    }
                    await Task.Run((Action)DownloadOrario);
                }
            }
            catch (Exception ex){ }
        }


        public void DownloadOrario()
        {
            //Add final m for classes of 6 chars
            if (ClasseCorso.Substring(ClasseCorso.Length - 2).ToLower() == "ca")
            {
                ClasseCorso = ClasseCorso.Remove(ClasseCorso.Length - 2) + "cam";
            }
            //Low "cam" string due case sensitive website
            if (ClasseCorso.Contains("CAM"))
            {
                ClasseCorso = ClasseCorso.Remove(ClasseCorso.Length - 3) + "cam";
            }

            //Bool for last timetable found
            bool found = false;
            for (int p = 6; p < 20; p++)
            {
                try
                {
                    //Try getting result from link
                    System.Net.WebRequest request = System.Net.WebRequest.Create($"https://www.salvemini.edu.it/orario/2020_21/p{p}/Classi/{ClasseCorso}.jpg");
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
                        TimeTableUrl = $"https://www.salvemini.edu.it/orario/2020_21/p{p - 1}/Classi/{ClasseCorso}.jpg";
                        if (myClass)
                            Preferences.Set("TimeTableUrl", TimeTableUrl);
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


        string TimeTableUrl = "";
        bool myClass;

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

        void TimetableDownloadButtonContainer_Clicked(System.Object sender, System.EventArgs e)
        {
            DependencyService.Get<IPlatformSpecific>().SavePictureToDisk("Orario", TimetableImage.GetImageAsJpgAsync().Result);
        }

        void getOrario()
        {

        }
    }
}
