using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Pages.BookMarket
{
    public class MeetingCheckModel : PageModel
    {
        private readonly Salvemini_DBContext db; public MeetingCheckModel(Salvemini_DBContext context) { db = context; }


        [BindProperty]
        public BookUtenti user { get; set; }

        public async Task<IActionResult> OnGet()
        {
            //Check auth
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return RedirectToPage("/index");

            //Get user
            user = db.BookUtenti.Find(HttpContext.Session.GetInt32("id").Value);

            return Page();
        }
    }
}
