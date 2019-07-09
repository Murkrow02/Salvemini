using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalveminiApi.Models;

namespace SalveminiApi.BookMarket
{
    public partial class Login : System.Web.UI.Page
    {
       private SalveminiEntities db = new SalveminiEntities();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RegisterClick(object sender, EventArgs e)
        {
            try
            {
                //Everything ok?;
                if (string.IsNullOrEmpty(NomeInput.Text) || string.IsNullOrEmpty(CognomeInput.Text) || string.IsNullOrEmpty(PasswordInputR.Text) || string.IsNullOrEmpty(CellInputR.Text))
                {
                    //Non ha riempito tutto
                    descLabel.Text = "Completa tutti i campi!";
                    descLabel.ForeColor = Color.Red;
                    return;

                }

                //IP check
                string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
                var alreadyIp = db.BookUtenti.Where(x => x.Ip == pubIp);
                if (alreadyIp.Count() >= 1)
                {
                    //Troppi con lo stesso ip
                    descLabel.Text = "Hai raggiunto il numero massimo di account per questo indirizzo ip";
                    descLabel.ForeColor = Color.Red;
                    return;
                }

                //Cell check
                var cellConf = db.BookUtenti.Where(x => x.Telefono == CellInputR.Text).ToList();
                if (cellConf.Count() > 0)
                {
                    //Conflitto
                    descLabel.Text = "Esiste già un account registrato con questo numero";
                    descLabel.ForeColor = Color.Red;
                    return;
                }


                //Reset old values
                descLabel.Text = "Effettua l'accesso prima di registrare i tuoi libri";
                descLabel.ForeColor = Color.Gray;


                //Create new user
                var newUser = new BookUtenti();
                newUser.Nome = NomeInput.Text.Replace(" ", string.Empty);
                newUser.Cognome = CognomeInput.Text.Replace(" ", string.Empty);
                newUser.Password = PasswordInputR.Text;
                newUser.Ip = pubIp;
                newUser.Telefono = CellInputR.Text.Replace(" ", string.Empty);
                db.BookUtenti.Add(newUser);
            }
            catch
            {
                //Errore sconosciuto
                descLabel.Text = "Si è verificato un errore sconosciuto, riprova più tardi";
                descLabel.ForeColor = Color.Red;
                return;
            }
        }
        protected void AccediClick(object sender, EventArgs e)
        {
            //Find user
            var utente = db.BookUtenti.Where(x => x.Telefono == CellInputA.Text).ToList();
            if(utente.Count == 0 || utente == null)
            {

            }

            //Check pasword
        }
    }
}