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
    public class BasketModel : PageModel
    {
        [BindProperty]
        public List<BookLibri> Books { get; set; }

        [BindProperty]
        public decimal Price { get; set; }

        [BindProperty]
        public int ToDeleteBook { get; set; }

        private readonly BookMarket_DBContext db; public BasketModel(BookMarket_DBContext context) { db = context; }

        //Page load
        public async Task<IActionResult> OnGet()
        {
            //Check auth
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return RedirectToPage("login");

            //Get saved books
            var userId = HttpContext.Session.GetInt32("id").Value;
            var booked = db.BookCarrello.Where(x => x.IdUtente == userId).ToList();

            //Check duplicates
            var duplicates = booked.GroupBy(x => x.IdLibro).SelectMany(X => X.Skip(1));
            db.BookCarrello.RemoveRange(duplicates);
            db.SaveChanges();

            //Recount 
            if(duplicates.Count() > 0)
            booked = db.BookCarrello.Where(x => x.IdUtente == userId).ToList();

            Books = new List<BookLibri>();
           foreach(var book in booked)
            {
              var book_ = db.BookLibri.Find(book.IdLibro);
                Books.Add(book_);
                Price += Costants.CalculateUserPrice(book_.Prezzo.Value);
            }

            return Page();
        }

        //Remove book from basket
        public async Task<IActionResult> OnPostAsync()
        {
            //Authorize
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return new JsonResult(new { status = "unauthorized" });

            try
            {
                //Already ordered
                var user = db.BookUtenti.Find(HttpContext.Session.GetInt32("id").Value);
                if (user.AppuntamentoRitiro != null)
                {
                    return new JsonResult(new { status = "Hai già confermato il tuo ordine, pertanto non ti è concesso rimuovere libri dal carrello" });
                }

                //Remove book from basket
                db.BookCarrello.Remove(db.BookCarrello.FirstOrDefault(x => x.IdLibro == ToDeleteBook));
                db.SaveChanges();

                //All good, redirect to dashboard
                return new JsonResult(new { status = "success" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Si è verificato un errore inaspettato, contattaci se il problema persiste" });
            }

        }

        public async Task<IActionResult> OnGetBuy()
        {
            //Authorize
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return new JsonResult(new { status = "unauthorized" });

            //Get user 
            var userId = HttpContext.Session.GetInt32("id").Value;
            var user = db.BookUtenti.Find(userId);
            if(user == null)
                return new JsonResult(new { status = "unauthorized" });

            //Already ordered
            if (user.AppuntamentoRitiro != null)
            {
                return new JsonResult(new { status = "Hai già confermato il tuo ordine" });
            }

            //Get user basket
            var booked = db.BookCarrello.Where(x => x.IdUtente == userId).ToList();
            foreach (var book in booked)
            {
                var book_ = db.BookLibri.Find(book.IdLibro);
                if(book_.IdAcquirente != null)
                {
                    var toRemove = db.BookCarrello.FirstOrDefault(x => x.IdUtente == userId && x.IdLibro == book_.Id);
                    db.BookCarrello.Remove(toRemove);
                    db.SaveChanges();
                    return new JsonResult(new { status = "Il libro " + book_.Nome + " è stato già comprato da qualcuno, pertanto verrà rimosso dal tuo carrello (aggiorna la pagina per vedere i cambiamenti)" });
                }
                book_.IdAcquirente = userId;
            }

            var calculatedGap = Costants.FindFreeTimeGap(Costants.timeSpansRitiro(), 2, db);
            if (calculatedGap == null)
            {
                return new JsonResult(new { status = "Tutte le fasce orarie disponibili sono state riempite, pertanto non sono accettate ulteriori consegne" });
            }

            user.AppuntamentoRitiro = calculatedGap;

            db.SaveChanges();
            return new JsonResult(new { status = "success" });
        }


       


    }
}
