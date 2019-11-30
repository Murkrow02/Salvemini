using System;
namespace SalveminiApp.RestApi.Models
{
    public class Materie
    {
        public int id { get; set; }
        public string desMateria { get; set; }
		public string Materia { get; set; }

        public string sureMateria { get { return string.IsNullOrEmpty(Materia) ? desMateria.ToLower().FirstCharToUpper() : Materia; } }
	}
}
