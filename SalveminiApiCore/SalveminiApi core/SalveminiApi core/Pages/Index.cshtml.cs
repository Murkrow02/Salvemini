using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SalveminiApi_core.Argo.Models;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Salvemini_DBContext db; public IndexModel(Salvemini_DBContext context) { db = context; }

        [BindProperty]
        public int Users { get; set; }

        public async Task<IActionResult> OnGet()
        {
            return RedirectToPage("/bookmarket/login");
            //Users = db.Utenti.Count();
        }
    }
}
