using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SalveminiApi.Models;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalveminiApi.BookMarket
{
    public partial class LibriUtente : System.Web.UI.Page
    {
        DatabaseString db = new DatabaseString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["loggedAdmin"]?.ToString() != "si")
            {
                Response.Redirect("AdminLogin.aspx", false);
            }

            if (!Page.IsPostBack)
            {
                var libri = new List<BookLibri>();
                try
                {
                    var utente = (BookUtenti)Session["bookUtente"];
                    libri = db.BookLibri.Where(x => x.idUtente == utente.id).ToList();
                    ListView1.DataSource = libri;
                    ListView1.DataBind();
                    Session["userLibriList"] = libri;
                    nomeTitolo.Text = "Libri di " + utente.Nome + " " + utente.Cognome;
                }
                catch
                {
                    Response.Redirect("AdminCp.aspx", false);
                }

                try
                {
                    decimal? _totale = 0;

                    foreach(BookLibri libro in libri)
                    {
                        if (libro.Prezzo != null)
                            _totale += libro.Prezzo;
                    }

                    totale.Text = "Totale = " + _totale.Value.ToString("F") + "€";
                    totale.Visible = true;
                }
                catch
                {
                    totale.Text = "Non è stato possibile calcolare il totale";
                }
            }
        }



        protected void Commands(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "approve":
                    var libri = (List<BookLibri>)Session["userLibriList"];
                    var libro = db.BookLibri.Find(libri[e.Item.DisplayIndex].id);
                    libro.Accettato = true;
                    db.SaveChanges();
                    Response.Redirect("LibriUtente.aspx", false);
                    break;

            }
        }

        protected void back_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchUser.aspx", false);
        }
    }
}