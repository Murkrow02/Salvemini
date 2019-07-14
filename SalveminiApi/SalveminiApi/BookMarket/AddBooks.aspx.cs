using System;
using System.Web;
using System.Linq;
using SalveminiApi.Models;
using System.Drawing;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SalveminiApi.BookMarket
{
    public partial class AddBooks : System.Web.UI.Page
    {
        private SalveminiEntities db = new SalveminiEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //User not found in cookies
                HttpCookie userCookie = Request.Cookies["UserId"];
                if (userCookie == null)
                {
                    Response.Redirect("~/BookMarketLogin", false);
                    return;
                }

                //Find user
                var utente = db.BookUtenti.Find(Convert.ToInt32(Request.Cookies["UserId"].Value));

                //User not found
                if (utente == null)
                {
                    Response.Redirect("~/BookMarketLogin", false);
                    return;
                }

                //Ok
                //Find how many books
                var booksAdded = db.BookLibri.Where(x => x.idUtente == utente.id).ToList();


                if (!IsPostBack)
                {
                    ListView1.DataSource = booksAdded;
                    ListView1.DataBind();
                    Session["booksList"] = booksAdded;
                }

                //Add info display
               nomeLbl.Text = utente.Nome + " " + utente.Cognome;
                infoLbl.Text = "Hai aggiunto " + booksAdded.Count().ToString() + " libri, puoi aggiungerne ancora " + (50 - booksAdded.Count()).ToString();
            }
            catch
            {
                //Unexpected error
                Response.Redirect("~/BookMarketLogin", false);
                return;
            }


        }

        protected void Commands(object sender, ListViewCommandEventArgs e)
        {
            //Get books list
            List<BookLibri> libri = (List<BookLibri>)Session["booksList"];
            
            switch (e.CommandName)
            {
                case "remove":
                    //Get selected item and remove it from db
                    var libro = db.BookLibri.Find(libri[e.Item.DisplayIndex].id);
                    db.BookLibri.Remove(libro);
                    db.SaveChanges();
                    libri.Remove(libro);
                    ListView1.DataSource = libri;
                    ListView1.DataBind();
                    Response.Redirect("~/AggiungiLibro", false);
                    break;
                default:
                    return;
            }
        }


        protected void addBook(object sender, EventArgs e)
        {
            //Checks
            try
            {
                if (((List<BookLibri>)Session["booksList"]).Count() >= 50)
                {
                    //Too many books
                    descLabel.Text = "Hai raggiunto il limite massimo di libri per questo account :(";
                    descLabel.ForeColor = Color.Red;
                    Response.Redirect("~/AggiungiLibro", false);
                    return;
                }

                //Check title
                if (string.IsNullOrEmpty(titleInput.Text))
                {
                    //No title
                    descLabel.Text = "Devi inserire il titolo del libro";
                    descLabel.ForeColor = Color.Red;
                    Response.Redirect("~/AggiungiLibro", false);
                    return;
                }

                //Reset desc
                descLabel.Text = "Puoi aggiungere fino a 50 libri!";
                descLabel.ForeColor = Color.Gray;


                //Add to db
                HttpCookie userCookie = Request.Cookies["UserId"];
                var book = new BookLibri();
                book.Nome = titleInput.Text;
                book.Creazione = Helpers.Utility.italianTime();
                book.idUtente = Convert.ToInt32(userCookie.Value.ToString());
                db.BookLibri.Add(book);
                db.SaveChanges();
                Response.Redirect("~/AggiungiLibro", false);
            }
            catch
            {
                //Unexpected error
                descLabel.Text = "Si è verificato un errore sconosciuto, ricarica la pagina e contattaci se il problema persiste";
                descLabel.ForeColor = Color.Red;
                return;
            }

        }

        protected void logOut(object sender, EventArgs e)
        {
            //Removes the cookie.
            Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect("~/BookMarketLogin", false);
            return;
        }
    }
}