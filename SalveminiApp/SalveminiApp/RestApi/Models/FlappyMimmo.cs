using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class FlappySkinReturn : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int id { get; set; }
        public string Nome { get; set; }
        public List<string> Immagini { get; set; }
        public int Costo { get; set; }
        public bool Comprata { get; set; }

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

    public class SkinDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalSkinTemplate { get; set; }

        public DataTemplate OwnedSkinTemplate { get; set; }

        public DataTemplate SelectedSkinTemplate { get; set; }

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
