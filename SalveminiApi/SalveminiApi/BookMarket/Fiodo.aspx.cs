using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalveminiApi.Models;

namespace SalveminiApi.BookMarket
{
    public partial class Fiodo : System.Web.UI.Page
    {
        private DatabaseString db = new DatabaseString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                decimal soldi = 0;
                var prezzati = db.BookLibri.Where(x => x.Prezzo != null).ToList();
                foreach(BookLibri libro in prezzati)
                {
                    soldi += libro.Prezzo.Value;
                }
                prezzoPieno.Text = "Totale " + soldi.ToString() + "€";
                soldiTotali.Text = "In cassa " + ((soldi/2) + prezzati.Count) + "€";

            }
        }
    }
}