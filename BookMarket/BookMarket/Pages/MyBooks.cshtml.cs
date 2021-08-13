using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookMarket.Models;
using BookMarket.Data;

namespace BookMarket.Pages
{
    public class MyBooksModel : PageModel
    {
        [BindProperty]
        public List<BookLibri> Books { get; set; }

        [BindProperty]
        public int ToDeleteBook { get; set; }

        private readonly BookMarket_DBContext db; public MyBooksModel(BookMarket_DBContext context) { db = context; }

        //Page load
        public async Task<IActionResult> OnGet()
        {
            //Check auth
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)) || Costants.Fase() != 1)
                return RedirectToPage("login");

            //Get saved books
            var userId = HttpContext.Session.GetInt32("id").Value;
            var myBooks = db.BookLibri.Where(x => x.IdProprietario == userId).ToList();
            Books = myBooks;



            return Page();
        }

        //Remove book from list
        public async Task<IActionResult> OnPostAsync()
        {
            //Authorize
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return RedirectToPage("login");

            try
            {
                //Remove book from basket
                var userId = HttpContext.Session.GetInt32("id").Value;
                var toRemoveBook = db.BookLibri.FirstOrDefault(x => x.Id == ToDeleteBook);
                if(toRemoveBook.IdProprietario == userId)
                {
                    db.BookLibri.Remove(toRemoveBook);
                    db.SaveChanges();
                }
               

                //All good, redirect to dashboard
                return RedirectToPage("mybooks");
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Si è verificato un errore inaspettato, contattaci se il problema persiste" });
            }

        }
    }
}
