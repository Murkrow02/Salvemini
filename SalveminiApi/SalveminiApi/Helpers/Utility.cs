using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

       public static void addToAnalytics(string valore)
        {
            DatabaseString db2 = new DatabaseString();

            try
            {
                var data = Helpers.Utility.italianTime();
                var esiste = db2.Analytics.Where(x => x.Giorno.Year == data.Year && x.Giorno.Month == data.Month && x.Giorno.Day == data.Day && x.Tipo == valore).ToList();
                if (esiste.Count < 1)
                {
                    var accesso = new Analytics { Giorno = data, Tipo = valore, Valore = 1 };
                    db2.Analytics.Add(accesso);
                }
                else
                {
                    esiste[0].Valore = esiste[0].Valore + 1;
                }
                db2.SaveChanges();
            }
            catch (Exception ex)
            {
                //Fa niente
            }
        }

        public bool authorized(HttpRequestMessage re, bool vip = false)
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
                    {
                        //Vip needed?
                        if(!vip) //No
                        return true;
                        else //Yes
                        {
                            //Check if vip
                            if (utente.Stato < 1)
                                return false;
                            else
                                return true;
                        }

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
        
    }

    public static class Extensions {

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }

}