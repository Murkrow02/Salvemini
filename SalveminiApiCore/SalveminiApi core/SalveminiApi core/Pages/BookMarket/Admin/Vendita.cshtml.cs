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
    public class VenditaModel : PageModel
    {
        private readonly Salvemini_DBContext db;
        public VenditaModel(Salvemini_DBContext context)
        {
            db = context;
        }
       

        [BindProperty]
        public List<BookLibri> books { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("admin") != "yes")
            {
                return RedirectToPage("/bookmarket/login");
            }

            //Create final timetable
            //var utenti = db.BookUtenti.ToList();
            //foreach (var user in utenti.ToList())
            //{
            //    var uploaded = db.BookLibri.Where(x => x.IdUtente == user.Id);
            //    if (uploaded.Count() > 0)
            //    {
            //        var calculatedGap = Costants.FindFreeTimeGap(Costants.timeSpansRitiroFinale(), 3, db);
            //        if (calculatedGap == null)
            //        {
            //            throw new Exception();
            //        }

            //        user.AppuntamentoFinale = calculatedGap;
            //        db.SaveChanges();
            //    }
            //}


            books = db.BookLibri.Where(x => x.CompratoDa != null && x.Venduto != true).OrderBy(x => x.Id).ToList();

            return Page();
        }

        
    }
}
