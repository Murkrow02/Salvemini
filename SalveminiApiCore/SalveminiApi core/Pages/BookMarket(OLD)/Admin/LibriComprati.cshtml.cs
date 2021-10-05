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
    public class LibriCompratiModel : PageModel
    {
        private readonly Salvemini_DBContext db;
        public LibriCompratiModel(Salvemini_DBContext context)
        {
            db = context;
        }

        [BindProperty]
        public List<BookLibri> Books { get; set; }

        [BindProperty]
        public BookUtenti User { get; set; }

        [BindProperty]
        public int BookCode { get; set; }

        public IActionResult OnGet(int id, string filter)
        {
            if (HttpContext.Session.GetString("admin") != "yes")
            {
                return RedirectToPage("/bookmarket/login");
            }

            User = db.BookUtenti.Find(id);
            if (User == null)
                return RedirectToPage("/bookmarket/login");

            Books = db.BookLibri.Where(x => x.CompratoDa == id).ToList();
            if(!string.IsNullOrEmpty(filter))
            {
                Books.RemoveAll(x => x.Venduto == true);
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

                if (book.Venduto == true)
                    book.Venduto = false;
                else
                    book.Venduto = true;
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
