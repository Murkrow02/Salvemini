using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class RegistrationModel : PageModel
    {
        [BindProperty]
        public BookUtenti user { get; set; }

        [BindProperty]
        public string address { get; set; }

        private readonly BookMarket_DBContext db;
        private readonly IConfiguration configuration;
        public RegistrationModel(BookMarket_DBContext context, IConfiguration conf)
        {
            db = context;
            configuration = conf;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
               // return new JsonResult(new { status = "Le registrazioni apriranno il 27 agosto" });

                //Check if all data is entered
                if (string.IsNullOrEmpty(user.Nome) || string.IsNullOrEmpty(user.Cognome))
                {
                    return new JsonResult(new { status = "Inserisci nome e cognome per continuare" });
                }

                //Check if mail is entered correctly
                if (string.IsNullOrEmpty(user.Mail) || !Costants.IsValidEmail(user.Mail))
                {
                    return new JsonResult(new { status = "Inserisci una mail valida" });
                }

                if(string.IsNullOrEmpty(user.Password) || user.Password.Length < 8)
                {
                    return new JsonResult(new { status = "La password deve contenere minimo 8 caratteri" });
                }

                //Check if already registred
                var conflict = db.BookUtenti.Where(x => x.Mail == user.Mail);
                    if(conflict.Count() > 0)
                {
                    return new JsonResult(new { status = "Esiste già un account registrato con questa mail: " + user.Mail });
                }

                //Format data
                user.Nome = user.Nome.NameFormat();
                user.Cognome = user.Cognome.NameFormat();


                //Get local address
                if (string.IsNullOrEmpty(address)) { 
                    return new JsonResult(new { status = "Non è stato possibile reperire il tuo indirizzo ip, controlla la tua connessione ed il tuo browser" });
                }

                //Check account created from this ip
                var accountWithThisIp = db.BookUtenti.Where(x => x.Ip == address);
                if (accountWithThisIp.Count() >= 3)
                    return new JsonResult(new { status = "Hai gia creato molti account da questo indirizzo, prova ad accedere con uno creato in precedenza" });
                user.Ip = address;

                //Encrypt password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                //Generate token
                user.Token = Utility.CreateToken(15);
              //  user.MailToken = Utility.CreateToken(50);
                user.Mail = user.Mail.Trim();

                //Save user in db
                db.BookUtenti.Add(user);
                db.SaveChanges();


                //Send mail with verification code
                //var request = HttpContext.Request;
                //var _baseURL = $"{request.Scheme}://{request.Host}";
                //var success = await Utility.sendMail(user.Mail,"Verifica mail BookMarket", configuration, "Ciao " + user.Nome + ", clicca su questo link per verificare il tuo account: " + _baseURL + "/verifymail?id=" + user.Id + "&token=" + user.MailToken);
                //if(!success)
                //{
                //    db.BookUtenti.Remove(user);
                //    db.SaveChanges();
                //    return new JsonResult(new { status = "Non è stato possibile inviare una mail di conferma al tuo account, riprova più tardi o contattaci se il problema persiste" });
                //}

                //Save access info locally to create the session
                HttpContext.Session.SetString("token", user.Token);
                HttpContext.Session.SetInt32("id", user.Id);

                return new JsonResult(new { status = "success" });
            }
            catch(Exception ex)
            {
                try
                {
                    await Utility.sendMail("marcocoppola02.mc@gmail.com", "Crash bookmarket", configuration, $"Exception:{ex}" + Environment.NewLine + user.Nome + " " + user.Cognome + " " + user.Mail);
                }
                catch
                {

                }
                return new JsonResult(new { status = "Si è verificato un errore, contattaci se il problema persiste" });

            }
        }
    }
    public static class StringExtension
    {
        public static string FirstCharToUpper(this string value)
        {
            var culture = new CultureInfo("it-IT");
            return culture.TextInfo.ToTitleCase(value);

        }

        public static string Truncate(this string value, int maxChars)
        {
            try
            {
                return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
            }
            catch
            {
                return value;
            }
        }

        public static string NameFormat(this string value)
        {
            try
            {
                //Remove tail and start spaces
                value.Trim();

                //Remove multiple spaces
                string cleanedString = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", " ");

                //Split by apostrophe or space
                var parts = System.Text.RegularExpressions.Regex.Split(cleanedString, @"(?<=[ '’])");

                //Make string
                string result = "";
                foreach (var part in parts)
                {
                    result += char.ToUpper(part[0]) + part.Substring(1).ToLower();
                }

                //Remove non alpha
                result = new string(result.Where(c => Char.IsLetter(c) || c == '\'' || c == ' ' || c == '’').ToArray());

                //Return final result
                return result;
            }
            catch
            {
                return value;
            }
          
        }
    }

   

}
