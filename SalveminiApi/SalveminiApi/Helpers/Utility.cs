using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi.Helpers
{
    public class Utility
    {
        public static DateTime italianTime()
        {
            var todayDate = DateTime.UtcNow;
            var italianDate = todayDate.AddHours(2);
            return italianDate;
        }

       
    }
}