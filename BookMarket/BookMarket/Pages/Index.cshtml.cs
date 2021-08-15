using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RandomDataGenerator.Randomizers;
namespace BookMarket.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            Response.Redirect("Dashboard");
        }
    }
}
