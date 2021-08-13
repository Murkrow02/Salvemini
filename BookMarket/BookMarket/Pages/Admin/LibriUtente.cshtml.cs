using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Infrastructure;
using BookMarket.Models;
using BookMarket.Data;

namespace BookMarket.Pages.Admin
{
    public class LibriUtenteModel : PageModel
    {
        private readonly BookMarket_DBContext db;
        public LibriUtenteModel(BookMarket_DBContext context)
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
                return RedirectToPage("login");
            }

            User = db.BookUtenti.Find(id);
            if(User == null)
                return RedirectToPage("login");

            Books = db.BookLibri.Where(x => x.IdProprietario == id).ToList();

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
