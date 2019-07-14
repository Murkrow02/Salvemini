using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalveminiApi.BookMarket
{
    public partial class AdminLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void loginClick(object sender, EventArgs e)
        {
            bool passwordCorrect = BCrypt.Net.BCrypt.Verify(passwordTxt.Text, "$2a$11$It0X1aXEuwcXG5rd7XlZ3uZvJ3kU4EVGMubY5eqkLPbmtTKbso.Uy");
            if (passwordCorrect)
            {
                Session["loggedAdmin"] = "si";
                Response.Redirect("AdminCp.aspx", false);
            }
            else
            {
                passwordTxt.BorderColor = Color.Red;
            }

        }
    }
}