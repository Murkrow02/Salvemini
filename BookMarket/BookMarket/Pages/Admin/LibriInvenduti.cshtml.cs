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
    public class LibriInvendutiModel : PageModel
    {
        private readonly BookMarket_DBContext db;
        public LibriInvendutiModel(BookMarket_DBContext context)
        {
            db = context;
        }

        public int HalfPrice(BookLibri original)
        {
            return original.Prezzo != null ? Convert.ToInt32(original.Prezzo.Value / 2) : 0;
        }

        [BindProperty]
        public List<BookLibri> Books { get; set; }

        [BindProperty]
        public BookUtenti User { get; set; }

        [BindProperty]
        public int BookCode { get; set; }

        public decimal? Vendita = 0;

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("admin") != "yes")
            {
                return RedirectToPage("login");
            }

            User = db.BookUtenti.Find(id);
            if (User == null)
                return RedirectToPage("login");


            Books = db.BookLibri.Where(x => x.IdProprietario == id && x.Venduto != true).ToList();


            var SoldBooks = db.BookLibri.Where(x => x.IdProprietario == id && x.Venduto == true).ToList();
            SoldBooks.RemoveAll(x => x.Prezzo == null);
            foreach (var book in SoldBooks)
            {
                Vendita += book.Prezzo / 2;
            }



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
                var book = db.BookLibri.Find(BookCode);
                if (book == null)
                    return new JsonResult(new { status = "not found" });

                if (book.Prezzo != null)
                    book.Prezzo = null;
                db.SaveChanges();
                return new JsonResult(new { status = "success" });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = ex });
            }

        }
    }
}
