using System;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Note
    {
		public string flgVisualizzata { get; set; }
		public string datNota { get; set; }
		public int prgAnagrafe { get; set; }
		public int prgNota { get; set; }
		public DateTime oraNota { get; set; }
		public string desNota { get; set; }
		public string docente { get; set; }

        public bool SeparatorVisibility { get; set; } = true;
        public Thickness CellPadding { get; set; } = new Thickness(10);

        public string formattedTeacher
        {
            get
            {
                return docente.Substring(7).Replace(")", "");
            }
        }

        public string formattedDate
        {
            get
            {
                return Convert.ToDateTime(datNota).ToString("dddd, dd MMMM yyyy");
            }
        }

        public bool checkVisible
        {
            get
            {
                return flgVisualizzata == "S";
            }
        }
    }
}
