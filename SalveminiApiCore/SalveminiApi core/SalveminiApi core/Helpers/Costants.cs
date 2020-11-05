﻿using Microsoft.AspNetCore.Http;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SalveminiApi_core
{
    public class Costants
    {
        //Argo static cose
        public static string argoVersion = "2.1.0";
        public static string argoKey = "ax6542sdru3217t4eesd9";
        public static string allegatiUrl(string prgMessaggio, string prgAllegato)
        {
            return "https://www.portaleargo.it/famiglia/api/rest/messaggiobachecanuova?id=FFFSS16836EEEII0000" + prgAllegato + "00000000" + prgMessaggio + "{token}" + argoKey;
        }


        //private readonly Salvemini_DBContext db; public Costants(Salvemini_DBContext context = null) { db = context; }

        //Check if entered mail is valid
        public static bool IsValidEmail(string email) { try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email; } catch { return false; } }

        //Return our dictionary's subject or a formatted string if there isn't one
        public static string ShortSubject(string subject)
        {
            Salvemini_DBContext db = new Salvemini_DBContext();
            var materia = db.Materie.SingleOrDefault(x => x.DesMateria == subject);
            if (materia != null)
            {
                //return SubjectsList[subject];
                return materia.Materia;
            }
            else
            {
                return Utility.FirstCharToUpper(subject.ToLower());
            }
        }

        //Materie mercatino
        public static List<string> MarketSubjects()
        {
            var list = new List<string>();
            list.Add("Letteratura");
            list.Add("Arte (Disegno)");
            list.Add("Filosofia");
            list.Add("Inglese");
            list.Add("Matematica");
            list.Add("Fisica");
            list.Add("Informatica");
            list.Add("Storia");
            list.Add("Scienze");
            list.Add("Francese");
            list.Add("Spagnolo");
            list.Add("Arte (Teoria)");
            list.Add("Biology");
            list.Add("Religione");
            list.Add("Educazione Fisica");
            list.Add("History");
            list.Add("Tedesco");
            list.Add("Literature");
            list.Add("Geography");
            list.Add("Storia e Geografia");
            list.Add("Latino");
            list.Add("Altro");
            return list;
        }

        public static DateTime? FindFreeTimeGap(List<DateTime> gaps, int fase, Salvemini_DBContext db)
        {
            int maxForGap = 10;
            Dictionary<DateTime, int> gapsCount = new Dictionary<DateTime, int>();

            //Fill gaps count
            foreach (var gap in gaps)
            {
                if (fase == 1)
                {
                    var gapCount = db.BookUtenti.Where(x => x.AppuntamentoRilascio == gap).Count();
                    gapsCount.Add(gap, gapCount);
                }
                else if(fase == 2)
                {
                    var gapCount = db.BookUtenti.Where(x => x.AppuntamentoRitiro == gap).Count();
                    gapsCount.Add(gap, gapCount);
                }
                else if (fase == 3)
                {
                    var gapCount = db.BookUtenti.Where(x => x.AppuntamentoFinale == gap).Count();
                    gapsCount.Add(gap, gapCount);
                }
            }

            //Find more free gap
            var moreFreeGap = gapsCount.OrderBy(x => x.Value).First();
            if (moreFreeGap.Value >= maxForGap)
                return null;

            return moreFreeGap.Key;
        }
        public static List<DateTime> timeSpansConsegna()
        {
            var spans = new List<DateTime>();

            //First day
            var startingHour = new DateTime(2020, 8, 24, 9, 0, 0);
            var endingHour = new DateTime(2020, 8, 24, 13, 0, 0);
            var minutesSpan = (endingHour - startingHour).TotalMinutes;
            for (int i = 0; i < minutesSpan; i += 15)
            {
                spans.Add(startingHour.AddMinutes(i));
            }

            //Second day
            var startingHour2 = new DateTime(2020, 8, 25, 9, 0, 0);
            var endingHour2 = new DateTime(2020, 8, 25, 13, 0, 0);
            var minutesSpan2 = (endingHour2 - startingHour2).TotalMinutes;
            for (int i = 0; i < minutesSpan2; i += 15)
            {
                spans.Add(startingHour2.AddMinutes(i));
            }

            return spans;
        }

        public static List<DateTime> timeSpansRitiro()
        {
            var spans = new List<DateTime>();

            //First day
            var startingHour = new DateTime(2020, 8, 28, 9, 0, 0);
            var endingHour = new DateTime(2020, 8, 28, 13, 0, 0);
            var minutesSpan = (endingHour - startingHour).TotalMinutes;
            for (int i = 0; i < minutesSpan; i += 15)
            {
                spans.Add(startingHour.AddMinutes(i));
            }

            //Second day
            var startingHour2 = new DateTime(2020, 8, 31, 9, 0, 0);
            var endingHour2 = new DateTime(2020, 8, 31, 13, 0, 0);
            var minutesSpan2 = (endingHour2 - startingHour2).TotalMinutes;
            for (int i = 0; i < minutesSpan2; i += 15)
            {
                spans.Add(startingHour2.AddMinutes(i));
            }

            return spans;
        }

        public static List<DateTime> timeSpansRitiroFinale()
        {
            var spans = new List<DateTime>();

            //First day
            var startingHour = new DateTime(2020, 9, 1, 9, 0, 0);
            var endingHour = new DateTime(2020, 9, 1, 13, 0, 0);
            var minutesSpan = (endingHour - startingHour).TotalMinutes;
            for (int i = 0; i < minutesSpan; i += 15)
            {
                spans.Add(startingHour.AddMinutes(i));
            }

            //Second day
            var startingHour2 = new DateTime(2020, 9, 2, 9, 0, 0);
            var endingHour2 = new DateTime(2020, 9, 2, 11, 30, 0);
            var minutesSpan2 = (endingHour2 - startingHour2).TotalMinutes;
            for (int i = 0; i < minutesSpan2; i += 15)
            {
                spans.Add(startingHour2.AddMinutes(i));
            }

            return spans;
        }

        static DateTime currentDate = Utility.italianTime();

        //1 = Solo bottone carica; 2 = Solo bottone cerca; 3 = Nessun bottone
        public static int Fase()
        {
            //if (currentDate > new DateTime(2020, 8, 21, 0, 0, 0) &&  currentDate < new DateTime(2020, 8, 24, 0, 0, 0)) {
            //    return 1;
            //}

            //if (currentDate > new DateTime(2020, 8, 25, 14, 0 ,0) && currentDate < new DateTime(2020, 8, 31, 0, 0,0))
            //{
            //    return 2;
            //}

            return 2;
        }

        public static decimal CalculateUserPrice(decimal fullprice)
        {
            return ((Math.Round(fullprice + 1)) / 2) + 1M;
        }
    }
}
