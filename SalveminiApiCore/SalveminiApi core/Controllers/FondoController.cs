using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;

namespace SalveminiApi_core.Controllers
{

    [Route("api/fondo")]
    [ApiController]
    public class FondoController : ControllerBase
    {
       private readonly Salvemini_DBContext db; public FondoController(Salvemini_DBContext context) { db = context; }

        [Route("transactions")]
        [HttpGet]
        public IActionResult Transactions()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Get transactions
            var transactions = db.FondoStudentesco.ToList();

            //Success
            return Ok(transactions.OrderByDescending(x => x.Data).ToList());
        }

        [Route("transactions")]
        [HttpPost]
        public IActionResult AddTransaction([FromBody] FondoStudentesco transaction)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db, 2);
            if (!authorized)
                return Unauthorized();

            //Add transaction in db
            transaction.Data = Utility.italianTime();
            db.FondoStudentesco.Add(transaction);
            db.SaveChanges();

            //Success
            return Ok("Transazione aggiunta con successo");
        }
    }
}
