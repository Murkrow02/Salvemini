using System;
using System.Collections.ObjectModel;

namespace SalveminiApp
{
    public class MaterieViewModel
    {
        private ObservableCollection<string> materie;
        public ObservableCollection<string> Materie
        {
            get { return materie; }
            set { materie = value; }
        }
        public MaterieViewModel()
        {
            materie = new ObservableCollection<string>();
            materie.Add("Italiano");
            materie.Add("Latino");
        }

    }
}
