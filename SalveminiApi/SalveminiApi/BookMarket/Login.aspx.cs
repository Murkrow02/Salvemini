using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
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
                    descLabelR.Text = "Completa tutti i campi!";
                    descLabelR.ForeColor = Color.Red;
                    return;

                }

                if(PasswordInputR.Text.Count() < 8)
                {
                    //Password corta
                    descLabelR.Text = "La password deve contenere almeno 8 caratteri";
                    descLabelR.ForeColor = Color.Red;
                    return;
                }

                //IP check
                string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
                var alreadyIp = db.BookUtenti.Where(x => x.Ip == pubIp);
                if (alreadyIp.Count() >= 10)
                {
                    //Troppi con lo stesso ip
                    descLabelR.Text = "Hai raggiunto il numero massimo di account per questo indirizzo ip";
                    descLabelR.ForeColor = Color.Red;
                    return;
                }

                //Cell check
                var cellConf = db.BookUtenti.Where(x => x.Telefono == CellInputR.Text).ToList();
                if (cellConf.Count() > 0)
                {
                    //Conflitto
                    descLabelR.Text = "Esiste già un account registrato con questo numero";
                    descLabelR.ForeColor = Color.Red;
                    return;
                }


                //Reset old values
                descLabelR.Text = "Effettua l'accesso prima di registrare i tuoi libri";
                descLabelR.ForeColor = Color.Gray;


                //Create new user
                var newUser = new BookUtenti();
                newUser.Nome = NomeInput.Text.Trim();
                newUser.Cognome = CognomeInput.Text.Trim();
                newUser.Password = PasswordInputR.Text;
                newUser.Ip = pubIp;
                newUser.Telefono = CellInputR.Text.Replace(" ", string.Empty);
                newUser.Creazione = Helpers.Utility.italianTime();
                db.BookUtenti.Add(newUser);
                db.SaveChanges();

                //Success
                Request.Cookies.Add(new HttpCookie("UserId"));
                Request.Cookies["UserId"].Value = newUser.id.ToString();
                Request.Cookies["UserId"].Expires = DateTime.Now.AddDays(5);
                Response.Redirect("~/AggiungiLibro", false);
            }
            catch(Exception ex)
            {
                //Errore sconosciuto
                descLabelR.Text = "Si è verificato un errore sconosciuto, riprova più tardi";
                descLabelR.ForeColor = Color.Red;
                return;
            }
        }
        protected void AccediClick(object sender, EventArgs e)
        {
            try
            {
                //Find user
                var utente = db.BookUtenti.Where(x => x.Telefono == CellInputA.Text).ToList();
                if (utente.Count == 0 || utente == null)
                {
                    //Utente non trovato
                    descLabelA.Text = "Non è stato trovato nessun utente con questo numero di telefono";
                    descLabelA.ForeColor = Color.Red;
                    return;
                }

                //Check pasword
                if (utente[0].Password != PasswordInputA.Text)
                {
                    //Password non valida
                    descLabelA.Text = "La password inserita non è corretta";
                    descLabelA.ForeColor = Color.Red;
                    return;
                }

                //Reset old values
                descLabelA.Text = "Accedi se hai già un account";
                descLabelA.ForeColor = Color.Gray;

                //Success
                // Add the cookie.
                HttpCookie userCookie = new HttpCookie("UserId");
                userCookie.Value = utente[0].id.ToString();
                userCookie.Expires = DateTime.Now.AddDays(5);
                Response.Cookies.Add(userCookie);
                Response.Redirect("~/AggiungiLibro", false);
            }
            catch
            {
                //Errore sconosciuto
                descLabelA.Text = "Si è verificato un errore sconosciuto, riprova più tardi";
                descLabelA.ForeColor = Color.Red;
                return;
            }
        }
    }
}