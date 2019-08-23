using System;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Promemoria
    {
        public string desAnnotazioni { get; set; }
        public string datGiorno { get; set; }
        public string desMittente { get; set; }
        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);

        public string Data
        {
            get
            {
                return Convert.ToDateTime(datGiorno).ToString("dddd, dd MMMM yyyy");
            }
        }
    }
}