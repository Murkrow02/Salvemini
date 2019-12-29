using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class FlappySkinReturn 
    {

      

        public int id { get; set; }
        public string Nome { get; set; }
        public List<string> Immagini { get; set; }
        public int Costo { get; set; }
        public bool Comprata { get; set; }

        public bool isSelected
        {
            get
            {
                return Preferences.Get("flappySkin", "classicMimmo") == Immagini[0].Remove(Immagini[0].Length - 1);
            }
            set
            {

            }
        }

        public Color BgColor
        {
            get
            {
                return isSelected ? Color.FromHex("#5A99FF") : Color.White;
            }
            set
            {

            }
        }

        public List<string> FullImmagini
        {
            get
            {
                var list = new List<string>();
                foreach(string item in Immagini)
                {
                    list.Add(Costants.Uri("images/flappyskin/") + item);
                }
                return list;
            }
        }

        public string DisplayImage
        {
            get
            {
                return FullImmagini[0];
            }
        }
    }

    public class FlappyMoneteReturn
    {
        public int id { get; set; }
        public string Descrizione { get; set; }
        public int Costo { get; set; }
        public int Valore { get; set; }
        public bool Comprata { get; set; }
    }

    public class UtentiClassifica
    {
        public string NomeCognome { get; set; }
        public string Image { get; set; }
        public int Punteggio { get; set; }
        public int id { get; set; }

        public string FullImmagine
        {
            get
            {
                return Costants.Uri("images/users/") + Image;
            }
        }
    }

    public class SkinDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalSkinTemplate { get; set; }

        public DataTemplate OwnedSkinTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            //return ((RestApi.Models.Message)item).Testo == "!$Richiesta_Scheda$!" ? SheetRequestTemplate : MyMessageTemplate;
            if (((FlappySkinReturn)item).Comprata)
            {
                return OwnedSkinTemplate;
            }
            else
            {
                return NormalSkinTemplate;
            }
        }
    }
}
