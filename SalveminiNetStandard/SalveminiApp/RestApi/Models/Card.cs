using System;
namespace SalveminiApp.RestApi.Models
{
	public partial class Offerte
    {
		public int id { get; set; }
		public string Nome { get; set; }
		public string Descrizione { get; set; }
		public string Immagine { get; set; }

        public string FullImmagine
        {
            get
            {
                if (!string.IsNullOrEmpty(Immagine))
                {
                        return Costants.Uri("images/card/")+ Immagine;
                    
                }

                return "";
            }
        }
    }
}
