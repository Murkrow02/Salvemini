using Microsoft.AspNetCore.Http;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalveminiApi_core
{
    public class AuthHelper
    {
        public static bool Authorize(HttpRequest re, Salvemini_DBContext db, int minStatus = 0)
        {
            string token;
            string id;
            var headers = re.Headers;
            if (headers.ContainsKey("x-auth-token"))
            {
                token = headers["x-auth-token"].First();

                //Null Token
                if (string.IsNullOrEmpty(token))
                    return false;

                if (headers.ContainsKey("x-user-id"))
                {
                    id = headers["x-user-id"].First();

                    //Null id
                    if (string.IsNullOrEmpty(id))
                        return false;

                    //Check token with user
                    var utente = db.Utenti.Find(Convert.ToInt32(id));
                    if (utente == null || utente.ArgoToken != token)
                        return false;
                    else
                    {
                        //Check user status
                        if (utente.Stato >= minStatus)
                            return true;
                        else
                            return false;
                    }
                }
                else
                {
                    //No id in headers
                    return false;
                }
            }
            else
            {
                //No token in headers
                return false;
            }
        }


        //Passing session to get saved token and ip this bool returns true if the connection is authorized
        public static async Task<bool> AuthorizeWeb(HttpContext context, Salvemini_DBContext db)
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

