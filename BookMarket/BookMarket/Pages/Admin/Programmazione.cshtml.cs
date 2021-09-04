using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookMarket.Models;
using BookMarket.Data;

namespace BookMarket.Pages.Admin
{
    public class ProgrammazioneModel : PageModel
    {
        private readonly BookMarket_DBContext db; public ProgrammazioneModel(BookMarket_DBContext context) { db = context; }

        [BindProperty]
        public List<BookUtenti> users { get; set; }

        public void OnGet()
        {
            //if (Costants.Fase() == 3)
            //    users = db.BookUtenti.Where(x => x.AppuntamentoFinale != null).OrderBy(x => x.AppuntamentoFinale).ToList();
            //else if (Costants.Fase() == 1)
            //    users = db.BookUtenti.Where(x => x.AppuntamentoRilascio != null).OrderBy(x => x.AppuntamentoRilascio).ToList();
            //else if (Costants.Fase() == 2)
                users = db.BookUtenti.Where(x => x.AppuntamentoRitiro != null).OrderBy(x => x.AppuntamentoRitiro).ToList();
        }
    }
}
