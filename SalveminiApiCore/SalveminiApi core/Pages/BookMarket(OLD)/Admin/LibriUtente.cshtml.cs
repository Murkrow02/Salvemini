using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Pages.BookMarket.Admin
{
    public class LibriUtenteModel : PageModel
    {
        private readonly Salvemini_DBContext db;
        public LibriUtenteModel(Salvemini_DBContext context)
        {
            db = context;
        }

        [BindProperty]
        public List<BookLibri> Books { get; set; }

        [BindProperty]
        public BookUtenti User { get; set; }

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("admin") != "yes")
            {
                return RedirectToPage("/bookmarket/login");
            }

            User = db.BookUtenti.Find(id);
            if(User == null)
                return RedirectToPage("/bookmarket/login");

            Books = db.BookLibri.Where(x => x.IdUtente == id).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (HttpContext.Session.GetString("admin") != "yes")
            {
                return new JsonResult(new { status = "unauthorized" });
            }

            try
            {
                foreach (var book in Books)
                {
                    var book_ = db.BookLibri.Find(book.Id);
                    book_.Prezzo = book.Prezzo;
                }

                db.SaveChanges();

                return new JsonResult(new { status = "success" });

            }
            catch(Exception ex)
            {
                return new JsonResult(new { status = ex });
            }
         
        }
    }
}
