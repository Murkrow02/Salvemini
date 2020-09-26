using System;
namespace SalveminiApp.RestApi.Models
{
    public class Treno
    {
        public string Partenza { get; set; }

        //(D = Diretto, DD = Direttissimo)
        public string Importanza { get; set; }

        //(FER = Feriale, FES = Festivo)
        public string Variazioni { get; set; } = "";

        //(0 = Sorrento, 1 = Sant'Agnello)
        public int Stazione { get; set; }

        //(true = Napoli, false = Sorrento)
        public bool Direzione { get; set; }

        public string DirectionString
        {
            get
            {
                return Direzione ? "Napoli" : "Sorrento";
            }
        }

        public DateTime LeaveTime
        {
            get
            {
                var a = Partenza.Split(':');
                return new DateTime(1, 1, 1, Convert.ToInt32(a[0]), Convert.ToInt32(a[1]), 0);
            }
        }

        public bool DDVisibility
        {
            get
            {
                return Importanza == "DD";
            }
        }

        public string FullVariazioni
        {
            get
            {
                if (Importanza == "EXP")
                {
                    return "Express";
                }

                if (!string.IsNullOrEmpty(Variazioni))
                {
                    return Variazioni == "FER" ? "Feriale" : "Festivo";
                }
                else
                {
                    return "";
                }
            }
        }

        public Xamarin.Forms.Color VariazioniColor
        {
            get
            {
                return Importanza == "EXP" ? Xamarin.Forms.Color.Red : Xamarin.Forms.Color.FromHex("#D3D3D3");
            }
        }
    }

    
}
