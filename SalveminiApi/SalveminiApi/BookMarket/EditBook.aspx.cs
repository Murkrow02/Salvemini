using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalveminiApi.Models;

namespace SalveminiApi.BookMarket
{
    public partial class EditBook : System.Web.UI.Page
    {
        private SalveminiEntities db = new SalveminiEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["loggedAdmin"]?.ToString() != "si")
            {
                Response.Redirect("AdminLogin.aspx", false);
            }

            try
            {
                var libro = (BookLibri)Session["selectedBook"];
                if (libro == null)
                    throw new Exception();
                //if(!IsPostBack)
                //prezzoTxt.Text = libro.Prezzo?.ToString();
            }
            catch
            {
                Response.Redirect("CercaLibro.aspx", false);
            }
           

        }

        protected void editClick(object sender, EventArgs e)
        {
            try
            {
                var libro = (BookLibri)Session["selectedBook"];
                var findLibro = db.BookLibri.Find(libro.id);
                findLibro.Prezzo = Convert.ToDecimal(prezzoTxt.Text.Trim().Replace(".",","));
                db.SaveChanges();
                Response.Redirect("CercaLibro.aspx", false);
            }
            catch
            {
                errorLabel.Visible = true;
            }
            
        }
    }
}