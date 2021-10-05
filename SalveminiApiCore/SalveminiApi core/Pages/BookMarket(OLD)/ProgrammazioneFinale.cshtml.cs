using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Pages.BookMarket.Admin
{
    public class ProgrammazioneFinaleModel : PageModel
    {
        private readonly Salvemini_DBContext db; public ProgrammazioneFinaleModel(Salvemini_DBContext context) { db = context; }

        [BindProperty]
        public List<BookUtenti> users { get; set; }

        public void OnGet()
        {
                users = db.BookUtenti.Where(x => x.AppuntamentoFinale != null).OrderBy(x => x.Cognome).ToList();
        }
    }
}
