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

            try
            {
                var utente = (BookUtenti)Session["bookUtente"];
                var libri = db.BookLibri.Where(x => x.idUtente == utente.id).ToList();
                ListView1.DataSource = libri;
                ListView1.DataBind();
                nomeTitolo.Text = "Libri di " + utente.Nome + " " + utente.Cognome;
            }
            catch
            {
                Response.Redirect("AdminCp.aspx", false);
            }

        }
    }
}