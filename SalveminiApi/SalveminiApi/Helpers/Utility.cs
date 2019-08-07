using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using SalveminiApi.Models;

namespace SalveminiApi.Helpers
{
    public class Utility
    {
        DatabaseString db = new DatabaseString();

        public static DateTime italianTime()
        {
            var todayDate = DateTime.UtcNow;
            var italianDate = todayDate.AddHours(2);
            return italianDate;
        }

       public bool authorized(HttpRequestMessage re)
        {
            string token;
            string id;
            var headers = re.Headers;
            if (headers.Contains("x-auth-token"))
            {
                token = headers.GetValues("x-auth-token").First();

                //Null Token
                if (string.IsNullOrEmpty(token))
                    return false;

                if (headers.Contains("x-user-id")){
                    id = headers.GetValues("x-user-id").First();

                    //Null id
                    if (string.IsNullOrEmpty(id))
                        return false;

                    //Check token with user
                    var utente = db.Utenti.Find(Convert.ToInt32(id));
                    if (utente.ArgoToken != token)
                        return false;
                    else
                        return true;
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
    }
}