using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using BookMarket.Models;
using BookMarket.Data;

namespace BookMarket.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly BookMarket_DBContext db; private readonly IConfiguration configuration;
        public DashboardModel(BookMarket_DBContext context, IConfiguration conf) { db = context; configuration = conf; }


        [BindProperty]
        public BookUtenti user { get; set; }

        [BindProperty]
        public int booksCount { get; set; }

        [BindProperty]
        public decimal balance { get; set; }
        public async Task<IActionResult> OnGet()
        {

            //Check auth
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return RedirectToPage("login");

            //Get logged user
            user = db.BookUtenti.Find(HttpContext.Session.GetInt32("id"));
            if(user == null)
                return RedirectToPage("login");

            //Get books uploaded 
          var userBooks = db.BookLibri.Where(x => x.IdProprietario == user.Id);

            //Get balance
            foreach (var book in userBooks)
            {
                if(book.Prezzo != null && book.IdAcquirente != null)
                balance += book.Prezzo.Value;
            }

            //Get books count
            booksCount = userBooks.Count();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Check auth
            if (!(await AuthHelper.AuthorizeWeb(HttpContext, db)))
                return new JsonResult(new { status = "unauthorized" });

            //Resend mail
            var user_ = db.BookUtenti.Find(HttpContext.Session.GetInt32("id"));
            if (!user_.LastMailSent.HasValue || user_.LastMailSent.Value.AddMinutes(5) < Utility.italianTime())
            {
                var success = await Utility.sendMail(user_.Mail, "Verifica mail BookMarket", configuration, "Ciao " + user_.Nome + ", clicca su questo link per verificare il tuo account: " + "https://www.mysalvemini.meverifymail?id=" + user_.Id + "&token=" + user_.MailToken);
              if(!success)
                {
                    return new JsonResult(new { status = "Non è stato possibile inviare una nuova mail, contattaci se il problema persiste" });
                }
                user_.LastMailSent = Utility.italianTime();
                db.SaveChanges();
            }
            else
            {
                return new JsonResult(new { status = "Hai già chiesto una nuova mail di recente, aspetta 5 minuti prima di poterne chiedere un'altra" });
            }

            return new JsonResult(new { status = "success" });
        }
    }
}
