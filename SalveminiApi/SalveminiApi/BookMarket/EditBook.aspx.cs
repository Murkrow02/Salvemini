using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalveminiApi.Models;

namespace SalveminiApi.BookMarket
{
    public partial class EditBook : System.Web.UI.Page
    {
        private DatabaseString db = new DatabaseString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["loggedAdmin"]?.ToString() != "si")
            {
                Response.Redirect("AdminLogin.aspx", false);
            }

            try
            {

                if (!Page.IsPostBack)
                {
                    var libro = (BookLibri)Session["selectedBook"];
                    if (libro == null)
                        throw new Exception();
                    title.Text = libro.Nome;
                }
            
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
                var prezzo = prezzoTxt.Text.Trim().Replace(",", ".");
                var newprezzo = decimal.Parse(prezzo, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.InvariantCulture);
                var findLibro = db.BookLibri.Find(libro.id);
                findLibro.Prezzo = newprezzo;
                
                db.SaveChanges();
                Response.Redirect("CercaLibro.aspx", false);
            }
            catch
            {
                errorLabel.Visible = true;
            }

        }

        protected void back_Click(object sender, EventArgs e)
        {
            Response.Redirect("CercaLibro.aspx", false);
        }
    }
}