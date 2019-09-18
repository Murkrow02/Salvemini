using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SalveminiApp.Helpers
{
    public partial class CountDown : ContentView
    {
            public CountDown()
            {
                InitializeComponent();

                updateTime();
            }






            async void updateTime()
            {
                string d1val = "0";
                string d2val = "0";
                string d3val = "0";
                string h1val = "0";
                string h2val = "0";
                string m1val = "0";
                string m2val = "0";
                string s1val = "0";
                string s2val = "0";
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
                for (; ; )
                {
                    //Crea qui data di 7 caratteri sempre
                    var data = DateTime.Now.ToString("YddHHmmss");



                    //Updates
                    if (s2val != data[8].ToString())
                    {
                        await Task.WhenAll(s2.TranslateTo(0, -10, speedCicle), s2.FadeTo(0, speedCicle));
                        s2.Text = data[8].ToString();
                        s2val = data[8].ToString();
                        s2.TranslationY = 10;
                        await Task.WhenAll(s2.TranslateTo(0, 0, speedCicle), s2.FadeTo(1, speedCicle));
                    }
                    if (s1val != data[7].ToString())
                    {
                        await Task.WhenAll(s1.TranslateTo(0, -10, speedCicle), s1.FadeTo(0, speedCicle));
                        s1.Text = data[7].ToString();
                        s1val = data[7].ToString();
                        s1.TranslationY = 10;
                        await Task.WhenAll(s1.TranslateTo(0, 0, speedCicle), s1.FadeTo(1, speedCicle));
                    }
                    if (m2val != data[6].ToString())
                    {
                        await Task.WhenAll(m2.TranslateTo(0, -10, speedCicle), m2.FadeTo(0, speedCicle));
                        m2.Text = data[6].ToString();
                        m2val = data[6].ToString();
                        m2.TranslationY = 10;
                        await Task.WhenAll(m2.TranslateTo(0, 0, speedCicle), m2.FadeTo(1, speedCicle));
                    }
                    if (m1val != data[5].ToString())
                    {
                        await Task.WhenAll(m1.TranslateTo(0, -10, speedCicle), m1.FadeTo(0, speedCicle));
                        m1.Text = data[5].ToString();
                        m1val = data[5].ToString();
                        m1.TranslationY = 10;
                        await Task.WhenAll(m1.TranslateTo(0, 0, speedCicle), m1.FadeTo(1, speedCicle));
                    }
                    if (h2val != data[4].ToString())
                    {
                        await Task.WhenAll(h2.TranslateTo(0, -10, speedCicle), h2.FadeTo(0, speedCicle));
                        h2.Text = data[4].ToString();
                        h2val = data[4].ToString();
                        h2.TranslationY = 10;
                        await Task.WhenAll(h2.TranslateTo(0, 0, speedCicle), h2.FadeTo(1, speedCicle));
                    }

                    if (h1val != data[3].ToString())
                    {
                        await Task.WhenAll(h1.TranslateTo(0, -10, speedCicle), h1.FadeTo(0, speedCicle));
                        h1.Text = data[3].ToString();
                        h1val = data[3].ToString();
                        h1.TranslationY = 10;
                        await Task.WhenAll(h1.TranslateTo(0, 0, speedCicle), h1.FadeTo(1, speedCicle));
                    }

                    if (d3val != data[2].ToString())
                    {
                        await Task.WhenAll(d3.TranslateTo(0, -10, speedCicle), d3.FadeTo(0, speedCicle));
                        d3.Text = data[2].ToString();
                        d3val = data[2].ToString();
                        d3.TranslationY = 10;
                        await Task.WhenAll(d3.TranslateTo(0, 0, speedCicle), d3.FadeTo(1, speedCicle));
                    }
                    if (d2val != data[1].ToString())
                    {
                        await Task.WhenAll(d2.TranslateTo(0, -10, speedCicle), d2.FadeTo(0, speedCicle));
                        d2.Text = data[1].ToString();
                        d2val = data[1].ToString();
                        d2.TranslationY = 10;
                        await Task.WhenAll(d2.TranslateTo(0, 0, speedCicle), d2.FadeTo(1, speedCicle));
                    }

                //TEST TOGLI COMMENTO QUANDO FAI FUNZIONE VERA
                //if (d1val != data[0].ToString())
                //{
                //    await Task.WhenAll(d1.TranslateTo(0, -10, speedCicle), d1.FadeTo(0, speedCicle));
                //    d1.Text = data[0].ToString();
                //    d1val = data[0].ToString();
                //    d1.TranslationY = 10;
                //    await Task.WhenAll(d1.TranslateTo(0, 0, speedCicle), d1.FadeTo(1, speedCicle));
                //}
                d1.Text = "1";

                    await Task.Delay(200);
                }
            }

        }
    }

