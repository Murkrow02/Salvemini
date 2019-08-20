using System;
using System.Collections.Generic;
using System.Linq;
using SalveminiApi.Models;
using System.Web.UI.WebControls;

namespace SalveminiApi.BookMarket
{
    public partial class CercaLibro : System.Web.UI.Page
    {
        private DatabaseString db = new DatabaseString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["loggedAdmin"]?.ToString() != "si")
            {
                Response.Redirect("AdminLogin.aspx", false);
            }

            if (!Page.IsPostBack)
            {
                BindListView();
            }
        }

        private void BindListView()
        {
            var libri = db.BookLibri.Where(x=> x.Accettato == true).ToList();

            //Bind listview
            ListView1.DataSource = null;
            ListView1.DataSource = libri;
            Session["idLibriList"] = libri;
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
            var libri = db.BookLibri.Where(x => x.id.ToString() == searchBar.Text).ToList();
            if (libri == null)
                libri = libri.Where(x => x.Nome.ToLower().Contains(searchBar.Text.ToLower())).ToList();
            libri = libri.Where(x => x.Accettato == true).ToList();
            ListView1.DataSource = null;
            ListView1.DataSource = libri;
            Session["idLibriList"] = libri;
            ListView1.DataBind();
            searchBar.Focus();
        }

        protected void Commands(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "view":
                    var libri = (List<BookLibri>)Session["idLibriList"];
                    var libro = db.BookLibri.Find(libri[e.Item.DisplayIndex].id);
                    Session["selectedBook"] = libro;
                    Response.Redirect("EditBook.aspx", false);
                    break;

            }
        }

        protected void back_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminCp.aspx", false);
        }
    }
}