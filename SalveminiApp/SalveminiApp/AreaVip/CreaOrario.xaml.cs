using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Plugin.Segmented.Control;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class CreaOrario : ContentPage
    {
        public List<string> materie = new List<string>();
        public List<RestApi.Models.newOrario> lezioni = new List<RestApi.Models.newOrario>();
        public List<RestApi.Models.Lezione> callOrario = null;
        public List<string> availableDays = new List<string>();
        public IList<SegmentedControlOption> sedi = new List<SegmentedControlOption>();
        public int daySkipped = 0;

        public CreaOrario(string classe = "")
        {
            InitializeComponent();

            //Fill materie list todo
            materie.Add("Italiano");
            materie.Add("Inglese");

            //Fill picker with days of week
            giornoLibero.ItemsSource = Costants.getDays();

            //Add sedi source to segmented controls
            var centrale = new SegmentedControlOption { Text = "Centrale" };
            var succursale = new SegmentedControlOption { Text = "Succursale" };
            sedi.Add(centrale);
            sedi.Add(succursale);

            //Detect if super vip or restrict to class
            if (!string.IsNullOrEmpty(classe))
            {
                classEntry.Text = classe;
                classEntry.IsEnabled = false;
            }

        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Check internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Costants.showToast("connection");
                return;
            }



            //Auto fill entries
            if (string.IsNullOrEmpty(classEntry.Text))
                return;

            var classeCorso = classEntry.Text;
            var orario = await App.Orari.GetOrario(classeCorso);
            if (orario.Data != null)
            {
                callOrario = (orario.Data as List<RestApi.Models.Lezione>).OrderBy(x => x.Giorno).ToList();

                //Get freeday
                var freeday = Costants.Giorni[((DayOfWeek)callOrario.FirstOrDefault(x => x.Materia == "Libero").Giorno).ToString()];

                //Select freeday
                giornoLibero.SelectedItem = freeday;

                //Fill layout with old values
                fillLayout(callOrario);

                giornoLibero.IsEnabled = true;
            }
            else
            {
                giornoLibero.IsEnabled = true;
            }
        }


        void fillLayout(List<RestApi.Models.Lezione> orario = null)
        {
            //get int of day skipped
            daySkipped = Costants.getDays().IndexOf(giornoLibero.SelectedItem.ToString()) + 1;

            //Show save button
            saveBtn.IsVisible = true;

            //Remove free day from daylist
            availableDays = Costants.getDays();
            availableDays.Remove(giornoLibero.SelectedItem.ToString());

            //Generate 5 view, one for each day
            layout.Children.Clear();
            foreach (string giorno in availableDays)
            {
                //Add day label
                layout.Children.Add(new Label { Text = giorno, FontSize = 25, FontAttributes = FontAttributes.Bold, TextColor = Styles.TextColor });
                //Add segmented control
                layout.Children.Add(new SegmentedControl { Children = sedi, SelectedTextColor = Color.Black, TintColor = Color.White });

                //Create 7 autocomplete entries
                for (int i = 1; i <= 7; i++)
                {
                   
                    //Get int of the day

                    //var dayInt = Convert.ToInt32((DayOfWeek)Enum.Parse(typeof(DayOfWeek), Costants.Giorni.FirstOrDefault(x => x.Value == giorno).Key));

                    var dayInt = Costants.Giorni.Values.ToList().IndexOf(giorno);

                    //Initialize list
                    List<RestApi.Models.Lezione> list = new List<RestApi.Models.Lezione>();
                    if (orario != null)
                    {
                        //Get list of subjects
                        list = orario.Where(x => x.Giorno == dayInt).OrderBy(x => x.Ora).ToList();
                    }

                    layout.Children.Add(new Label { Text = i.ToString() + "a ora" });
                    layout.Children.Add(new Syncfusion.SfAutoComplete.XForms.SfAutoComplete { Text = orario != null && list.ElementAtOrDefault(i - 1) != null ? list[i - 1].Materia : null, AutoCompleteMode = Syncfusion.SfAutoComplete.XForms.AutoCompleteMode.Append, DataSource = materie });

                }
            }
        }

        //When the picker loses focus generate a new layout
        private void picker_Unfocused(object sender, FocusEventArgs e)
        {
            //No day selected, return
            if (string.IsNullOrEmpty(giornoLibero.SelectedItem?.ToString()))
            {
                saveBtn.IsVisible = false;
                return;
            }

            fillLayout();
        }

        public async void saveOrario_Clicked(object sender, EventArgs e)
        {
            //Ask confirmation
            bool sure = await DisplayAlert("Attenzione", "Questo orario sovrascriverà quello precedente, sei sicuro di voler procedere?", "Procedi", "Annulla");
            if (!sure)
                return;

            //Initialize values
            int giorno = 1; //Current day scanned
            int ora = 1; //Current hour scanned
            string sede = "";
            int segmentedIndex = 1;
            int oreGiorno = 0; //Save how many hours added for that day

            //If his freeday is monday start from martedi
            if (daySkipped == 1)
                giorno = 2;

            //Get values from each autocomplete entry
            foreach (var input in layout.Children.ToList())
            {
                try
                {
                    //Skip free day
                    if (giorno == daySkipped)
                        continue;

                    //Get value from segmented
                    if (input == layout.Children.ToList()[segmentedIndex])
                    {
                        var segmented = input as SegmentedControl;
                        sede = segmented.Children[segmented.SelectedSegment].Text;
                        continue;
                    }

                    //Get autocomplete view
                    var autoComplete = input as Syncfusion.SfAutoComplete.XForms.SfAutoComplete;

                    //Get value from autocomplete
                    var materia = autoComplete.Text;

                    //Add lezione to list
                    var lezione = new RestApi.Models.newOrario { Ora = ora, Giorno = giorno, Materia = materia, Sede = sede };
                    lezioni.Add(lezione);

                    //Save that hour is not empty for this day
                    if(!string.IsNullOrEmpty(materia))
                    oreGiorno++;

                    //If is last hour skip to next day
                    if (ora == 7)
                    {
                        if(oreGiorno < 3) //Ha messo meno di tre ore in un giorno
                        {
                            await DisplayAlert("Attenzione","Non puoi inserire un giorno con meno di 3 ore","Ok");
                            return;
                        }

                        giorno++;
                        ora = 1;
                        oreGiorno = 0;
                        segmentedIndex += 15;
                    }
                    else
                        ora++; //Else skip to next hour
                }
                catch
                {
                    continue;
                }
            }

            //Checks



            //Add fake free day hour to list
            var freeDay = new RestApi.Models.newOrario { Ora = 1, Materia = "Libero", Sede = "", Giorno = daySkipped };
            lezioni.Add(freeDay);
            //Upload new orario
            var classeCorso = Preferences.Get("Classe", 0) + Preferences.Get("Corso", "");
            lezioni = lezioni.OrderBy(x => x.Giorno).ToList();
            var success = await App.Orari.UploadOrario(classeCorso, lezioni);
            //Show result
            await DisplayAlert(success[0], success[1], "Ok");
            //Success, close page
            if (success[0] == "Grazie!")
                await Navigation.PopAsync();
            lezioni.Clear();
        }

    }
}
