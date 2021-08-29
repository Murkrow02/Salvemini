using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query.Internal;
using BookMarket.Models;
using BookMarket.Data;

namespace BookMarket.Pages
{
    public class UploadBookModel : PageModel
    {
        private readonly BookMarket_DBContext db; public UploadBookModel(BookMarket_DBContext context) { db = context; }

        [BindProperty]
        public BookLibri newBook { get; set; }

        [BindProperty]
        public List<string> subjects { get; set; }

         public async Task<IActionResult> OnGet()
        {
            //Check auth
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return RedirectToPage("login");

            subjects = Costants.MarketSubjects();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Authorize
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return new JsonResult(new { status = "unauthorized" });

            try
            {
                var idProprietario = HttpContext.Session.GetInt32("id").Value;

                if (Costants.Fase() != 1 && idProprietario != 413)
                {
                    return new JsonResult(new { status = "Il tempo per caricare i libri è scaduto" });
                }

                //Check that all data is entered
                if (string.IsNullOrEmpty(newBook.Nome) || string.IsNullOrEmpty(newBook.Codice) || string.IsNullOrEmpty(newBook.Materia))
                {
                    return new JsonResult(new { status = "Inserisci tutti i dati prima di procedere" });
                }

                if (newBook.Codice.Count() < 10)
                {
                    return new JsonResult(new { status = "Il codice del libro è troppo corto" });
                }

                //Get logged user
                newBook.IdProprietario = idProprietario;

                //See if account is verified
                var user = db.BookUtenti.Find(newBook.IdProprietario);
                if (user.MailToken != null)
                {
                    return new JsonResult(new { status = "Non puoi caricare un libro se prima non verifichi il tuo account, controlla la tua mail e riprova" });
                }

                //Check if user edited subjects
                if(!Costants.MarketSubjects().Contains(newBook.Materia))
                {
                    return new JsonResult(new { status = "La materia selezionata non è valida" });
                }


                //Check max books not passed
                var booksOfLoggedUser = db.BookLibri.Where(x => x.IdProprietario == newBook.IdProprietario).Count();
                if (booksOfLoggedUser >= 50)
                {
                    return new JsonResult(new { status = "Hai raggiunto il numero massimo di libri caricabili per questo account, rimuovine qualcuno prima di caricarne di nuovi" });
                }

                //Add new book to db
                newBook.DataCaricamento = Utility.italianTime();
                newBook.Nome = newBook.Nome.Trim();
                db.BookLibri.Add(newBook);
                db.SaveChanges();

                //Detect if is first book uploaded and redirect to meeting check page
                if (booksOfLoggedUser == 0 && user.AppuntamentoRilascio == null)
                {
                    var calculatedGap = Costants.FindFreeTimeGap(Costants.timeSpansConsegna(), 1, db);
                    if(calculatedGap == null)
                    {
                        db.BookLibri.Remove(newBook);
                        db.SaveChanges();
                        return new JsonResult(new { status = "Tutte le fasce orarie disponibili sono state riempite, pertanto non sono accettate ulteriori consegne" });                     
                    }

                    user.AppuntamentoRilascio = calculatedGap;
                    db.SaveChanges();
                    return new JsonResult(new { status = "meetingcheck"});
                }


                //All good, redirect to dashboard
                return new JsonResult(new { status = "success" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Si è verificato un errore inaspettato, contattaci se il problema persiste" });
            }
        }
       
    }
}
