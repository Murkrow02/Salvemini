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
        public List<string> availableDays = new List<string>();
        public IList<SegmentedControlOption> sedi = new List<SegmentedControlOption>();
        public int daySkipped = 0;

        public CreaOrario()
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

        }

        //When the picker loses focus generate a new layout
        private void picker_Unfocused(object sender, FocusEventArgs e)
        {
            //No day selected, return
            if (string.IsNullOrEmpty(giornoLibero.SelectedItem.ToString()))
            {
                saveBtn.IsVisible = false;
                return;
            }

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
                layout.Children.Add(new SegmentedControl { Children = sedi, SelectedTextColor = Color.Black });

                //Create 7 autocomplete entries
                for (int i = 1; i <= 7; i++)
                {
                    layout.Children.Add(new Label { Text = i.ToString() + "a ora" });
                    layout.Children.Add(new Syncfusion.SfAutoComplete.XForms.SfAutoComplete { AutoCompleteMode = Syncfusion.SfAutoComplete.XForms.AutoCompleteMode.Append, DataSource = materie });

                }
            }
        }

        public async void saveOrario_Clicked(object sender, EventArgs e)
        {
            //Initialize values
            int giorno = 1;
            int ora = 1;
            string sede = "";
            int segmentedIndex = 1;

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

                    //Skip ora if is null
                    if (string.IsNullOrEmpty(materia) || string.IsNullOrWhiteSpace(materia))
                    {
                        //Last hour, can skip to other day
                        if (ora == 7 || ora == 6 || ora == 5)
                        {
                            giorno++;
                            ora = 1;
                            segmentedIndex += 15;
                            continue;
                        }
                        else
                        {
                            //1,2,3,4 ora non possono essere vuote
                            await DisplayAlert("Attenzione", "Le ore dalla prima alla quarta non possono essere lasciate vuote", "Ok");
                            return;
                        }
                    }

                    //Add lezione to list
                    var lezione = new RestApi.Models.newOrario { Ora = ora, Giorno = giorno, Materia = materia, Sede = sede };
                    lezioni.Add(lezione);

                    //If is last hour skip to next day
                    if (ora == 7)
                    {
                        giorno++;
                        ora = 1;
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

            //Add fake free day hour to list
            var freeDay = new RestApi.Models.newOrario { Ora = 1, Materia = "Libero", Sede = "", Giorno = daySkipped };
            lezioni.Add(freeDay);
            //Upload new orario
            var classeCorso = Preferences.Get("Classe", 0) + Preferences.Get("Corso", "");
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
