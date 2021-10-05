using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Pages.BookMarket.Admin
{
    public class ProgrammazioneModel : PageModel
    {
        private readonly Salvemini_DBContext db; public ProgrammazioneModel(Salvemini_DBContext context) { db = context; }

        [BindProperty]
        public List<BookUtenti> users { get; set; }

        public void OnGet()
        {
            if(Costants.Fase() == 3)
               users = db.BookUtenti.Where(x => x.AppuntamentoFinale != null).OrderBy(x => x.AppuntamentoFinale).ToList();
            else
            users = db.BookUtenti.Where(x => x.AppuntamentoRitiro != null).OrderBy(x => x.AppuntamentoRitiro).ToList();
        }
    }
}
