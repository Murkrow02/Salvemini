using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookMarket.Data;

namespace BookMarket.Pages.Admin
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Password { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (Password == "parapendiocaneritiro")
            {
                HttpContext.Session.SetString("admin", "yes");
                return RedirectToPage("/admin/ritiro");
            }
            else if (Password == "parapendiocanevendita")
            {
                HttpContext.Session.SetString("admin", "yes");
                return RedirectToPage("/admin/programmazione");
            }

            return RedirectToPage("login");
        }
    }
}
