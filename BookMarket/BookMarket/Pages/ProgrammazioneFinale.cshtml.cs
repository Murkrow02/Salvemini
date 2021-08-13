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
    public class ProgrammazioneFinaleModel : PageModel
    {
        private readonly BookMarket_DBContext db; public ProgrammazioneFinaleModel(BookMarket_DBContext context) { db = context; }

        [BindProperty]
        public List<BookUtenti> users { get; set; }

        public void OnGet()
        {
                users = db.BookUtenti.Where(x => x.AppuntamentoFinale != null).OrderBy(x => x.Cognome).ToList();
        }
    }
}
