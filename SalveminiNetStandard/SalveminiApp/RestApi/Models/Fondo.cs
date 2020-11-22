using System;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public partial class FondoStudentesco
    {
        public long Id { get; set; }
        public string Causa { get; set; }
        public DateTime Data { get; set; }
        public decimal Importo { get; set; }

        public Color ColoreImporto
        {
            get
            {
                return Importo > 0 ? Color.FromHex("#4CDC7A") : Color.FromHex("#F95050");
            }
        }

        public string FullImporto
        {
            get
            {
                return Importo > 0 ? "+" + Importo.ToString() : Importo.ToString();
            }
        }

        public string FormattedDate
        {
            get
            {
                return Data.ToString("dd/MM/yyyy");
            }
        }
    }
}
