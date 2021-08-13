using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookMarket.Models;
using BookMarket.Data;

namespace BookMarket.Pages.Admin
{
    public class RitiroModel : PageModel
    {
        private readonly BookMarket_DBContext db;
        public RitiroModel(BookMarket_DBContext context)
        {
            db = context;
        }

        [BindProperty]
        public List<BookUtenti> Users { get; set; }
        public IActionResult OnGet(string filter)
        {
            if (HttpContext.Session.GetString("admin") != "yes")
            {
                return RedirectToPage("login");
            }

            Users = db.BookUtenti.ToList();
            if (!string.IsNullOrEmpty(filter))
            {
                foreach(var user in Users.ToList())
                {
                    var usersBook = db.BookLibri.Where(x => x.IdProprietario == user.Id);
                    var notSoldBooks = usersBook.Where(x => x.Venduto != true).Count();
                    if (notSoldBooks == 0)
                        Users.Remove(user);


                }
            }

            return Page();
        }
    }
}
