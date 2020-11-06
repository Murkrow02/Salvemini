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
            int count = 0;
            foreach (var user in db.Utenti.ToList())
            {
                count++;
                if (count < 800)
                    continue;

                Debug.WriteLine(count);

                //Prendi schede
                var argoUtils = new ArgoUtils();
                var schedeClient = argoUtils.ArgoClient(0, user.ArgoToken);
                var schedeResponse = await schedeClient.GetAsync("https://www.portaleargo.it/famiglia/api/rest/schede");
                var schedeContent = await schedeResponse.Content.ReadAsStringAsync();
                var ArgoUser = new List<Utente>();
                try
                {
                    ArgoUser = JsonConvert.DeserializeObject<List<Utente>>(schedeContent);
                }
                catch
                {
                    //return StatusCode(406);
                }

                //Save each user in the db
                try
                {
                    foreach (Utente utente in ArgoUser)
                    {

                        //Conflict not found, create new user
                        var newUser = db.Utenti.Find(utente.prgAlunno);

                        newUser.Classe = Convert.ToInt32(utente.desDenominazione);
                        newUser.Corso = utente.desCorso;
                        //try { newUser.Compleanno = DateTime.ParseExact(utente.alunno.datNascita, "yyyy-MM-dd", new CultureInfo("it-IT")); } catch { newUser.Compleanno = new DateTime(2069,04,20); };
                        //newUser.Residenza = utente.alunno.desComuneResidenza != null ? Utility.FirstCharToUpper(utente.alunno.desComuneResidenza.ToLower()) : "";
                        db.SaveChanges();
                    }
                }
                catch { }
                await Task.Delay(500);
            }


            return RedirectToPage("/bookmarket/login");
            //Users = db.Utenti.Count();
        }
    }
}
