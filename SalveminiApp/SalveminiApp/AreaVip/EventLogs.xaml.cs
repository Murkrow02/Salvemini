using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SalveminiApp.AreaVip
{
    public partial class EventLogs : ContentPage
    {
        public EventLogs()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var consoleEvents = await App.Analytics.GetConsole();

            if(consoleEvents != null && consoleEvents.Count > 0)
            {
                labelLayout.Children.Clear();

                int max = 200;
                int count = 0;
                foreach(RestApi.Models.EventLog evento in consoleEvents)
                {
                    //Evita che il telefono implode e mette 2000 label
                    count++;
                    if (count > max)
                        return;

                    //Crea label per ogni informazione
                    var label = new Label();
                    var fs = new FormattedString();
                    fs.Spans.Add(new Span { Text = "[" + evento.Data.ToString("dd MMMM") + "] ", ForegroundColor = Color.FromHex("04e035"), Font = Font.SystemFontOfSize(15) });
                    fs.Spans.Add(new Span { Text = evento.Evento, ForegroundColor = Color.White, Font = Font.SystemFontOfSize(15) });
                    fs.Spans.Add(new Span { Text = Environment.NewLine });
                    label.FormattedText = fs;
                    labelLayout.Children.Add(label);
                }
            }
        }
    }
}
