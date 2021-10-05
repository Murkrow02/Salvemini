using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Pages.BookMarket.Admin
{
    public class RitiroModel : PageModel
    {
        private readonly Salvemini_DBContext db;
        public RitiroModel(Salvemini_DBContext context)
        {
            db = context;
        }

        [BindProperty]
        public List<BookUtenti> Users { get; set; }
        public IActionResult OnGet(string filter)
        {
            if (HttpContext.Session.GetString("admin") != "yes")
            {
                return RedirectToPage("/bookmarket/login");
            }

            Users = db.BookUtenti.ToList();
            if (!string.IsNullOrEmpty(filter))
            {
                foreach(var user in Users.ToList())
                {
                    var usersBook = db.BookLibri.Where(x => x.IdUtente == user.Id);
                    var notSoldBooks = usersBook.Where(x => x.Venduto != true).Count();
                    if (notSoldBooks == 0)
                        Users.Remove(user);


                }
            }

            return Page();
        }
    }
}
