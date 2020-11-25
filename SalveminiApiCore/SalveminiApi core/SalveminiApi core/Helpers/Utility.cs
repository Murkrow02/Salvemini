using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace SalveminiApi_core
{
    public class Utility
    {

        //Salva evento nei log
        public static void saveCrash(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, string name, string error)
        {
            try
            {
                var path = env.WebRootPath + $"/crashes/";
                var fullPath = Path.Combine(path + name + ".txt");
                System.IO.File.WriteAllText(fullPath, error);
            }
            catch { }
        }

            //Salva evento nei log
            public static void saveEvent(HttpRequest re, Salvemini_DBContext db, string name)
        {
            int maxEvents = 5000;

            try
            {
                //Get all events
                var eventi = db.EventsLog.OrderByDescending(x => x.Data).ToList();

                //If elements are more than maxEvents delete first one
                if (eventi.Count > maxEvents)
                    eventi.RemoveAt(0);

                //Add event to log console
                db.EventsLog.Add(new EventsLog { Data = italianTime(), Evento = name });

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                //Fa niente
            }
        }

        public static int getUserId(HttpRequest re)
        {
            //Check headers are present
            if (!re.Headers.ContainsKey("x-user-id"))
                return -1;
            else
                return Convert.ToInt32(re.Headers["x-user-id"].First());
        }

        public static string getToken(HttpRequest re)
        {
            //Check headers are present
            if (!re.Headers.ContainsKey("x-auth-token"))
                return "";
            else
                return re.Headers["x-auth-token"].First();
        }

        

        //Get italian time
        public static DateTime italianTime()
        {
            //CultureInfo.CurrentCulture = new CultureInfo("it-IT");
            var utcNow = DateTime.UtcNow;
            var isDaylight = utcNow.IsDaylightSavingTime();
            var todayDate = isDaylight ? utcNow.AddHours(1) : utcNow.AddHours(2);
            return todayDate;
        }


        public static string FirstCharToUpper(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.  
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.  
            // ... Uppercase the lowercase letters following spaces.  
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        //Token Generator 
        public static string CreateToken(int size)
        {
            var charSet = "abcdefghjkmnopqrstuvwxyz1234567890";

            var chars = charSet.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        //Send mail client
        public static async Task<bool> sendMail(string to, string object_, IConfiguration configuration, string message_ = "", string html = "")
        {
            try
            {
                var client = new SmtpClient();
                client.Connect("mysalvemini.me");
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(configuration.GetValue<string>("MailAddress"), configuration.GetValue<string>("MailPassword"));
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Salvemini Bookmarket", configuration.GetValue<string>("MailAddress")));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = object_;
                var bodyBuilder = new BodyBuilder();

                if (!string.IsNullOrEmpty(message_))
                {
                    bodyBuilder.TextBody = message_;
                }
                else if (!string.IsNullOrEmpty(html))
                {
                    bodyBuilder.HtmlBody = html;
                }
                else
                {
                    return false;
                }
                message.Body = bodyBuilder.ToMessageBody();
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string SpanString(TimeSpan Data)
        {
            //Anni fa
            if (Data.Days >= 365)
                return (Data.Days / 365).ToString() + (Data.Days / 365 == 1 ? " anno fa" : " anni fa");

            //Mesi
            if (Data.Days >= 30)
                return (Data.Days / 30).ToString() + (Data.Days / 30 == 1 ? " mese fa" : " mesi fa");

            //Giorni
            if (Data.Days >= 7)
                return (Data.Days / 7).ToString() + (Data.Days / 7 == 1 ? " settimana fa" : " settimane fa");

            //Giorni
            if (Data.Days >= 1)
                return (Data.Days).ToString() + (Data.Days == 1 ? " giorno fa" : " giorni fa");

            //Ore
            if (Data.Hours >= 1)
                return (Data.Hours).ToString() + (Data.Hours == 1 ? " ora fa" : " ore fa");

            //Minuti
            if (Data.Minutes >= 1)
                return (Data.Minutes).ToString() + (Data.Minutes == 1 ? " minuto fa" : " minuti fa");

            //Secondi
            if (Data.Seconds >= 1)
                return (Data.Seconds).ToString() + (Data.Seconds == 1 ? " secondo fa" : " secondi fa");

            return "Ora";
        }

        private void ProcessImage(int width, int height, String filepath)
        {
           
        }
    }

    public static class DataExtensions
    {
        public static void RemoveRangePredicate<TEntity>(
          this DbSet<TEntity> entities,
            System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            var records = entities
                .Where(predicate)
                .ToList();
            if (records.Count > 0)
                entities.RemoveRange(records);
        }
    }
}
