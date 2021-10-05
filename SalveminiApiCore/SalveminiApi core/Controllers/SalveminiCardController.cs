using Microsoft.AspNetCore.Mvc;
using SalveminiApi_core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SalveminiApi_core.Controllers
{
    [Route("api/card")]
    [ApiController]
    public class SalveminiCardController : ControllerBase
    {
        private readonly Salvemini_DBContext db; public SalveminiCardController(Salvemini_DBContext context) { db = context; }

        [Route("offerte")]
        [HttpGet]
        public IActionResult getOfferte()
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db);
            if (!authorized)
                return Unauthorized();

            //Prendi offerte
            var offerte = db.SalveminiCard.ToList();
            return Ok(offerte);
        }

        [Route("offerta")]
        [HttpPost]
        public IActionResult postOfferta([FromBody]SalveminiCard offerta)
        {
            //Check Auth
            bool authorized = AuthHelper.Authorize(Request, db,3);
            if (!authorized)
                return Unauthorized();

            //Add offer to database
            db.SalveminiCard.Add(offerta);
            db.SaveChanges();
            return Ok();
        }

    }
}