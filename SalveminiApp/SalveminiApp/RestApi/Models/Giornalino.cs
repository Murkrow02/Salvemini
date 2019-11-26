using System;
namespace SalveminiApp.RestApi.Models
{
    public class Giornalino
    {
        public int id { get; set; }
        public System.DateTime Data { get; set; }
        public string Url
        {
            get
            {
                return Costants.Uri("giornalino/" + id.ToString());
            }
        }
    }
}
