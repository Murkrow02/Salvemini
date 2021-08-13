using Microsoft.AspNetCore.Http;
using BookMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookMarket.Data;
namespace BookMarket
{
    public class AuthHelper
    {


        //Passing session to get saved token and ip this bool returns true if the connection is authorized
        public static async Task<bool> AuthorizeWeb(HttpContext context, BookMarket_DBContext db)
        {
            var session = context.Session;

            //Check if both values are present in the session
            var token = session.GetString("token");
            var id = session.GetInt32("id");
            if (string.IsNullOrEmpty(token) || id == null)
            {
                //No ip or token or id found
                context.Response.Redirect("login");
                session.SetString("timeoutReason", "Effettua il login prima di visitare questa pagina");
                return false;
            }

            //Get user and see if token and ip are the same
            var utente = db.BookUtenti.Find(id);
            if (utente == null)
            {
                //User not found
                context.Response.Redirect("login");
                session.SetString("timeoutReason", "Effettua il login prima di visitare questa pagina");
                return false;
            }

            return true;
        }

    }
}

