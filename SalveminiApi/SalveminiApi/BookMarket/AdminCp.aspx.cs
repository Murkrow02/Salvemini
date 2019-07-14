using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalveminiApi.BookMarket
{
    public partial class AdminCp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["loggedAdmin"]?.ToString() != "si")
            {
                Response.Redirect("AdminLogin.aspx", false);
            }
        }

        protected void libriClick(object sender, EventArgs e)
        {
            Response.Redirect("CercaLibro.aspx", false);
        }

        protected void utentiClick(object sender, EventArgs e)
        {
            Response.Redirect("SearchUser.aspx", false);
        }
    }
}