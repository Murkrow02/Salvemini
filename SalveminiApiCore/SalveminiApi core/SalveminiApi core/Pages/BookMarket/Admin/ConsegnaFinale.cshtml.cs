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
    public class ConsegnaFinaleModel : PageModel
    {
        private readonly Salvemini_DBContext db;
        public ConsegnaFinaleModel(Salvemini_DBContext context)
        {
            db = context;
        }

        [BindProperty]
        public List<BookLibri> Books { get; set; }

        [BindProperty]
        public decimal Resto { get; set; }

        [BindProperty]
        public BookUtenti User { get; set; }

        public IActionResult OnGet(int id)
        {
            User = db.BookUtenti.Find(id);
            if (User == null)
                return RedirectToPage("/");

           //Get book rimasti
            Books = db.BookLibri.Where(x => x.IdUtente == id && x.Venduto != true).ToList();

            //Calculate money to give
            var libriVenduti = db.BookLibri.Where(x => x.IdUtente == id && x.Venduto == true && x.Prezzo != null);
            foreach (var libro in libriVenduti)
            {
                Resto += (libro.Prezzo.Value / 2);
            }

            if(Resto > 0)
            Resto = Resto - 1.50M;

            return Page();
        }
    }
}
