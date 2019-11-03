using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Diagnostics;

namespace SalveminiApp.Helpers
{
    public partial class CountDown : ContentView
    {

        public static List<DateTime> datesToSkip = new List<DateTime>
        {
            new DateTime(2019, 12, 25)
        };

        public CountDown()
        {
            InitializeComponent();

            //Get estimated time
            dateToStart = getEstimatedTime();

            updateTime();
        }

        public static TimeSpan dateToStart;

        public TimeSpan getEstimatedTime()
        {
            //Get dates
            var startDate = DateTime.Now;
            var endDate = new DateTime(2020, 6, 10, 13, 40, 0);
            var tempToSkip = new List<DateTime>();
            tempToSkip.AddRange(datesToSkip);
            for (var i = 0; i <= (endDate - startDate).Days; i++)
            {
                //Check if is freeday or sunday
                if (startDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday || startDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday)
                {
                    //Check if daystoskip already contains this day
                    if (!tempToSkip.Contains(startDate.AddDays(i)))
                    {
                        //Add day to days to skip
                        tempToSkip.Add(startDate.AddDays(i));
                    }
                }
            }

            //Return the estimate time to the end or 0 if the school is ended
            return (endDate - startDate.AddDays(tempToSkip.Count())).Days < 0 ? new TimeSpan(0) : endDate - startDate.AddDays(tempToSkip.Count());
        }
        string data = "";
        string d1val = "0";
        string d2val = "0";
        string d3val = "0";
        string h1val = "0";
        string h2val = "0";
        string m1val = "0";
        string m2val = "0";
        string s1val = "0";
        string s2val = "0";



        async void updateTime()
        {
            //Set Default Values
            d1.Text = "0";
            d2.Text = "0";
            d3.Text = "0";
            h1.Text = "0";
            h2.Text = "0";
            m1.Text = "0";
            m2.Text = "0";
            s1.Text = "0";
            s2.Text = "0";

            //Speed cycle
            uint speedCicle = 200;

            //Ciclo meglio sostituirlo con un bel timer
            for (int i = 0; ; i++)
            {
                //Stopwatch to elapse 1 second
                Stopwatch watch = new Stopwatch();

                //Start Watch
                watch.Start();

                await Task.Delay(600);

                //Subtract one second each cycle
                var tempSpan = dateToStart.Subtract(TimeSpan.FromSeconds(i));
                //var tempSpan = (new TimeSpan(100, 0, 0, 10)).Subtract(TimeSpan.FromSeconds(i));

                //Convert your value to string
                data = tempSpan.ToString("ddd") + tempSpan.ToString("hh") + tempSpan.ToString("mm") + tempSpan.ToString("ss");

                //Updates
                if (d1val != data[0].ToString())
                {
                    await Task.WhenAll(d1.TranslateTo(0, -10, speedCicle), d1.FadeTo(0, speedCicle), d2.TranslateTo(0, -10, speedCicle), d2.FadeTo(0, speedCicle), d3.TranslateTo(0, -10, speedCicle), d3.FadeTo(0, speedCicle), h1.TranslateTo(0, -10, speedCicle), h1.FadeTo(0, speedCicle), h2.TranslateTo(0, -10, speedCicle), h2.FadeTo(0, speedCicle), m1.TranslateTo(0, -10, speedCicle), m1.FadeTo(0, speedCicle), m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle), s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(0);
                    await Task.WhenAll(d1.TranslateTo(0, 0, speedCicle), d1.FadeTo(1, speedCicle), d2.TranslateTo(0, 0, speedCicle), d2.FadeTo(1, speedCicle), d3.TranslateTo(0, 0, speedCicle), d3.FadeTo(1, speedCicle), h1.TranslateTo(0, 0, speedCicle), h1.FadeTo(1, speedCicle), h2.TranslateTo(0, 0, speedCicle), h2.FadeTo(1, speedCicle), m1.TranslateTo(0, 0, speedCicle), m1.FadeTo(1, speedCicle), m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle), s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (d2val != data[1].ToString())
                {
                    await Task.WhenAll(d2.TranslateTo(0, -10, speedCicle), d2.FadeTo(0, speedCicle), d3.TranslateTo(0, -10, speedCicle), d3.FadeTo(0, speedCicle), h1.TranslateTo(0, -10, speedCicle), h1.FadeTo(0, speedCicle), h2.TranslateTo(0, -10, speedCicle), h2.FadeTo(0, speedCicle), m1.TranslateTo(0, -10, speedCicle), m1.FadeTo(0, speedCicle), m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle), s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(1);
                    await Task.WhenAll(d2.TranslateTo(0, 0, speedCicle), d2.FadeTo(1, speedCicle), d3.TranslateTo(0, 0, speedCicle), d3.FadeTo(1, speedCicle), h1.TranslateTo(0, 0, speedCicle), h1.FadeTo(1, speedCicle), h2.TranslateTo(0, 0, speedCicle), h2.FadeTo(1, speedCicle), m1.TranslateTo(0, 0, speedCicle), m1.FadeTo(1, speedCicle), m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle), s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (d3val != data[2].ToString())
                {
                    await Task.WhenAll(d3.TranslateTo(0, -10, speedCicle), d3.FadeTo(0, speedCicle), h1.TranslateTo(0, -10, speedCicle), h1.FadeTo(0, speedCicle), h2.TranslateTo(0, -10, speedCicle), h2.FadeTo(0, speedCicle), m1.TranslateTo(0, -10, speedCicle), m1.FadeTo(0, speedCicle), m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle), s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(2);
                    await Task.WhenAll(d3.TranslateTo(0, 0, speedCicle), d3.FadeTo(1, speedCicle), h1.TranslateTo(0, 0, speedCicle), h1.FadeTo(1, speedCicle), h2.TranslateTo(0, 0, speedCicle), h2.FadeTo(1, speedCicle), m1.TranslateTo(0, 0, speedCicle), m1.FadeTo(1, speedCicle), m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle), s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (h1val != data[3].ToString())
                {
                    await Task.WhenAll(h1.TranslateTo(0, -10, speedCicle), h1.FadeTo(0, speedCicle), h2.TranslateTo(0, -10, speedCicle), h2.FadeTo(0, speedCicle), m1.TranslateTo(0, -10, speedCicle), m1.FadeTo(0, speedCicle), m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle), s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(3);
                    await Task.WhenAll(h1.TranslateTo(0, 0, speedCicle), h1.FadeTo(1, speedCicle), h2.TranslateTo(0, 0, speedCicle), h2.FadeTo(1, speedCicle), m1.TranslateTo(0, 0, speedCicle), m1.FadeTo(1, speedCicle), m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle), s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (h2val != data[4].ToString())
                {
                    await Task.WhenAll(h2.TranslateTo(0, -10, speedCicle), h2.FadeTo(0, speedCicle), m1.TranslateTo(0, -10, speedCicle), m1.FadeTo(0, speedCicle), m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle), s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(4);
                    await Task.WhenAll(h2.TranslateTo(0, 0, speedCicle), h2.FadeTo(1, speedCicle), m1.TranslateTo(0, 0, speedCicle), m1.FadeTo(1, speedCicle), m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle), s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (m1val != data[5].ToString())
                {
                    await Task.WhenAll(m1.TranslateTo(0, -10, speedCicle), m1.FadeTo(0, speedCicle), m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle), s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(5);
                    await Task.WhenAll(m1.TranslateTo(0, 0, speedCicle), m1.FadeTo(1, speedCicle), m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle), s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (m2val != data[6].ToString())
                {
                    await Task.WhenAll(m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle), s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(6);
                    await Task.WhenAll(m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle), s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (s1val != data[7].ToString())
                {
                    await Task.WhenAll(s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle), s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(7);
                    await Task.WhenAll(s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle), s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    continue;
                }

                if (s2val != data[8].ToString())
                {
                    await Task.WhenAll(s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                    setStrings(8);
                    await Task.WhenAll(s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                }

            }
        }

        void setStrings(int id)
        {
            if (id >= 0)
            {
                if (id <= 8)
                {
                    s2.Text = data[8].ToString();
                    s2val = data[8].ToString();
                    s2.TranslationY = 10;
                }
                if (id <= 7)
                {
                    s1.Text = data[7].ToString();
                    s1val = data[7].ToString();
                    s1.TranslationY = 10;
                }
                if (id <= 6)
                {
                    m2.Text = data[6].ToString();
                    m2val = data[6].ToString();
                    m2.TranslationY = 10;
                }
                if (id <= 5)
                {
                    m1.Text = data[5].ToString();
                    m1val = data[5].ToString();
                    m1.TranslationY = 10;
                }
                if (id <= 4)
                {
                    h2.Text = data[4].ToString();
                    h2val = data[4].ToString();
                    h2.TranslationY = 10;
                }
                if (id <= 3)
                {
                    h1.Text = data[3].ToString();
                    h1val = data[3].ToString();
                    h1.TranslationY = 10;
                }
                if (id <= 2)
                {
                    d3.Text = data[2].ToString();
                    d3val = data[2].ToString();
                    d3.TranslationY = 10;
                }
                if (id <= 1)
                {
                    d2.Text = data[1].ToString();
                    d2val = data[1].ToString();
                    d2.TranslationY = 10;
                }
                if (id <= 0)
                {
                    d1.Text = data[0].ToString();
                    d1val = data[0].ToString();
                    d1.TranslationY = 10;
                }
            }
        }
    }
}

