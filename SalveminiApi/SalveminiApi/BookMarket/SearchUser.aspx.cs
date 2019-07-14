using System;
using System.Collections.Generic;
using System.Linq;
using SalveminiApi.Models;
using System.Web.UI.WebControls;

namespace SalveminiApi.BookMarket
{
    public partial class SearchUser : System.Web.UI.Page
    {
        private SalveminiEntities db = new SalveminiEntities();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindListView();
            }
        }

        private void BindListView()
        {
            var utenti = db.BookUtenti.ToList();

            //Bind listview
            ListView1.DataSource = null;
            ListView1.DataSource = utenti;
            Session["UtentiList"] = utenti;
            ListView1.DataBind();
        }

        protected void searching(object sender, EventArgs e)
        {
            //Empty bar
            if (string.IsNullOrEmpty(searchBar.Text))
            {
                BindListView();
                return;
            }

            //Search by name and surname
            var utenti = db.BookUtenti.Where(x => x.Nome.ToLower().Contains(searchBar.Text.ToLower()) || x.Cognome.ToLower().Contains(searchBar.Text.ToLower()) || (x.Nome + " " + x.Cognome).ToLower().Contains(searchBar.Text.ToLower())).ToList();
            ListView1.DataSource = null;
            ListView1.DataSource = utenti;
            Session["UtentiList"] = utenti;
            ListView1.DataBind();
            searchBar.Focus();
        }

        protected void Commands(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "view":
                    var utenti = (List<BookUtenti>)Session["UtentiList"];
                    var utente = db.BookUtenti.Find(utenti[e.Item.DisplayIndex].id);
                    Session["bookUtente"] = utente;
                    Response.Redirect("LibriUtente.aspx", false);
                    break;

            }
        }
    }
}