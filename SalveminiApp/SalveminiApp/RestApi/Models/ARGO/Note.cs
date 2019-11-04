using System;
namespace SalveminiApp.RestApi.Models.ARGO
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
	}
}
