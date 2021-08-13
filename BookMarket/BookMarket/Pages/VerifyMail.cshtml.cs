using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookMarket.Models;
using BookMarket.Data;

namespace BookMarket.Pages
{
    public class VerifyMailModel : PageModel
    {
        [BindProperty]
        public string Message { get; set; }

        private readonly BookMarket_DBContext db;
        public VerifyMailModel(BookMarket_DBContext context)
        {
            db = context;
        }

        public IActionResult OnGet(int? id, string token)
        {
            if(id == null || string.IsNullOrEmpty(token))
            {
                Message = "Il link utilizzato non è valido";
                return Page();
            }

            //Check if user exists
            var user = db.BookUtenti.Find(id.Value);
            if(user == null)
            {
                Message = "L'utente da attivare non è stato trovato";
                return Page();
            }

            //Check if token is right
            if(user.MailToken != token)
            {
                Message = "Il link utilizzato non è valido";
                return Page();
            }

            //All right activate user
            user.MailToken = null;
            db.SaveChanges();

            Message = "Il tuo account è stato verificato con successo, ora puoi chiudere questa pagina";
            return Page();
            }
        }
    }

