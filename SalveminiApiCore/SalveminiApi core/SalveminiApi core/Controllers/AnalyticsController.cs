using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalveminiApi_core.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController: ControllerBase
    {
        private readonly Salvemini_DBContext db; public AnalyticsController(Salvemini_DBContext context) { db = context; }

        [Route("all")]
        [HttpGet]
        public IActionResult GetAnalytics()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            var fullAnalytics = new List<Analytics>();

            //Add db saved analytics
            fullAnalytics.AddRange(db.Analytics.ToList());

            //Add user count
            fullAnalytics.Add(new Analytics { Tipo = "UtentiCount", Valore = db.Utenti.Count() });

            return Ok(fullAnalytics);
        }

        [Route("console")]
        [HttpGet]
        public IActionResult GetConsole()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            return Ok(db.EventsLog.OrderByDescending(x => x.Data).Take(500).ToList());
        }
    }
}
