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
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credentials credentials { get; set; }

        private readonly BookMarket_DBContext db;
        public LoginModel(BookMarket_DBContext context)
        {
            db = context;
        }

        public async Task<IActionResult> OnGet()
        {
            //decimal Resto = 0;

            //foreach(var utente in db.BookUtenti.ToList())
            //{
            //    //Calculate money to give
            //    var libriVenduti = db.BookLibri.Where(x => x.IdUtente == utente.Id && x.Venduto == true && x.Prezzo != null);
            //    foreach (var libro in libriVenduti)
            //    {
            //        Resto += (libro.Prezzo.Value / 2);
            //    }
            //}
           

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {


               // return new JsonResult(new { status = "Il bookmarket è terminato, non sono ammessi ulteriori accessi" });


                //Global check
                if (credentials == null || string.IsNullOrEmpty(credentials.Password) || string.IsNullOrEmpty(credentials.Mail))
                {
                    return new JsonResult(new { status = "Inserisci mail e password" });
                }

                credentials.Mail = credentials.Mail.Trim();
                    var user = db.BookUtenti.FirstOrDefault(x => x.Mail == credentials.Mail);

                    //No instructor with this username exists
                    if (user == null)
                    {
                        return new JsonResult(new { status = "Non è stato trovato nessun utente registrato con questa mail: " + credentials.Mail });
                    }

                    //Verify password
                    bool passwordCorrect = BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password);

                    //Wrong password
                    if (!passwordCorrect)
                    {
                        return new JsonResult(new { status = "La password inserita per " + user.Mail + " non è corretta" });
                    }

                        //Create token
                        var token = Utility.CreateToken(15);


                //Save info to db
                user.Token = token;
                await db.SaveChangesAsync();

                //Save access info locally to create the session
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetInt32("id", user.Id);


                
                //All good, redirect to dashboard
                return new JsonResult(new { status = "success" });
                }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Si è verificato un errore inaspettato, contattaci se il problema persiste" });
            }

        }

    }

    public class Credentials
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
