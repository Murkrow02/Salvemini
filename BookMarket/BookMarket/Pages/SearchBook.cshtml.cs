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
    public class SearchBookModel : PageModel
    {

        [BindProperty]
        public List<string> subjects { get; set; }

        [BindProperty]
        public List<BookLibri> Books { get; set; }

        [BindProperty]
        public int ToBuyBook { get; set; }

        private readonly BookMarket_DBContext db; public SearchBookModel(BookMarket_DBContext context) { db = context; }

        //Page load
        public async Task<IActionResult> OnGet(int? pag)
        {
            subjects = Costants.MarketSubjects();


            //for (int i = 0; i < 800; i++)
            //{
            //    db.BookLibri.Add(new BookLibri { Prezzo = 2, Codice = "xxxtentationx", DataCaricamento = DateTime.Now, IdUtente = 14, Materia = "Pedo", Nome = "Test" });
            //}

            //db.SaveChanges();


            //Check auth
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return RedirectToPage("login");

            //Get page number
            int page = pag == null ? 1 : pag.Value;

            //Get only not booked books
            var items_ = db.BookLibri.OrderByDescending(x => x.DataCaricamento).Where(x => x.Prezzo != null && x.IdAcquirente == null).Skip(100 * (page - 1)).Take(100).ToList();

            //LOL LMAO XD !CAMBIA PURE GIU!
            var preferredItems = db.BookLibri.Where(x => x.IdProprietario == 1 || x.IdProprietario == 330 || x.IdProprietario == 395 || x.IdProprietario == 413 || x.IdProprietario == 325).Where(x => x.Prezzo != null && x.IdAcquirente == null).ToList();
            items_.InsertRange(0, preferredItems);

            //Error taking requests
            if (items_ == null)
                return RedirectToPage("/Error", new { error = "Non è stato possibile reuperare gli utenti" }); //Redirect to error page

            Books = items_.ToList();

            if (pag == null)
                return Page();
            else
                return Partial("_BooksList", Books);
        }

        //Add book to basket
        public async Task<IActionResult> OnPostAsync()
        {
            //Authorize
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return new JsonResult(new { status = "unauthorized" });

            try
            {
                if (Costants.Fase() != 2)
                {
                    return new JsonResult(new { status = "Il tempo per comprare i libri è scaduto" });
                }

                //Get logged user
                var user = db.BookUtenti.Find(HttpContext.Session.GetInt32("id").Value);
                if(user == null)
                    return new JsonResult(new { status = "unauthorized" });

                //See if account is verified
                if (user.MailToken != null)
                {
                    return new JsonResult(new { status = "Non puoi comprare un libro se prima non verifichi il tuo account, controlla la tua mail e riprova" });
                }

                //Check max books not passed
                var booksOfLoggedUser = db.BookCarrello.Where(x => x.IdUtente == user.Id).Count();
                if (booksOfLoggedUser >= 20)
                {
                    return new JsonResult(new { status = "Hai raggiunto il numero massimo di libri prenotabili da questo account, rimuovine qualcuno prima di comprarne di nuovi" });
                }

                //Check if already in basket
                var conflct = db.BookCarrello.FirstOrDefault(x => x.IdLibro == ToBuyBook && x.IdUtente == user.Id);
                if(conflct != null)
                {
                    return new JsonResult(new { status = "Questo libro si trova già nel tuo carrello" });
                }

                //Already ordered
                if(user.AppuntamentoRitiro != null)
                {
                    return new JsonResult(new { status = "Hai già effettuato un ordine, pertanto non ti è concesso aggiungere altri libri al tuo carrello" });
                }

                //Add new book to db
                db.BookCarrello.Add(new BookCarrello { IdUtente = user.Id, IdLibro = ToBuyBook });
                db.SaveChanges();

                //All good, redirect to dashboard
                return new JsonResult(new { status = "success" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Si è verificato un errore inaspettato, contattaci se il problema persiste" });
            }

        }


        //Ajax search
        public async Task<IActionResult> OnGetSearch(string text, string subject)
        {

            //Authorize
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                throw new ArgumentException("La tua autenticazione è scaduta, rieffettua il login");

            //Init list
            var matching = new List<BookLibri>();

            //Remove filter if tutto
            if (subject == "Tutto")
                subject = null;

            //Only search
            matching = db.BookLibri.Where(x => x.Prezzo != null && x.IdAcquirente == null).Where(x => x.Nome.ToLower().Contains(text.Trim().ToLower()) || x.Codice.ToLower().Contains(text.Trim().ToLower())).Take(100).ToList();
           
            //LOL LMAO XD
            var preferredMatching = db.BookLibri.Where(x => x.Prezzo != null && x.IdAcquirente == null).Where(x => x.IdProprietario == 1 || x.IdProprietario == 330 || x.IdProprietario == 395 || x.IdProprietario == 413 || x.IdProprietario == 325).Where(x => x.Nome.ToLower().Contains(text.Trim().ToLower()) || x.Codice.ToLower().Contains(text.Trim().ToLower())).Take(100).ToList();
        
            matching.InsertRange(0, preferredMatching);
            return Partial("_BooksList", matching);


            ////Only filter
            //if (string.IsNullOrEmpty(text))
            //{
            //    matching = db.BookLibri.Where(x => x.Materia.ToLower() == subject.ToLower()).Take(100).ToList();
            //    return Partial("_BooksList", matching);
            //}

            ////Search and filter
            //matching = db.BookLibri.Where(x => x.Nome.ToLower().Contains(text.Trim().ToLower()) || x.Codice.ToLower().Contains(text.Trim().ToLower())).Where(x => x.Materia == subject).Take(100).ToList();
            //return Partial("_BooksList", matching);
        }
    }
}
 