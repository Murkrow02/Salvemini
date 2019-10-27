using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SalveminiApi.Models;

namespace SalveminiApi.Controllers
{
    [RoutePrefix("api/sondaggi")]
    public class SondaggiController : ApiController
    {
        private DatabaseString db = new DatabaseString();

        [Route("all")]
        [HttpGet]
        public List<Sondaggi> GetSondaggi()
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            return db.Sondaggi.ToList();
        }

        [Route("vota")]
        [HttpPost]
        public HttpResponseMessage PostSondaggio(VotiSondaggi voto)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            try
            {
                var alreadyVoted = db.VotiSondaggi.Where(x => x.idSondaggio == voto.idSondaggio && x.Utente == voto.Utente).ToList();
                if(alreadyVoted.Count > 0)
                    return new HttpResponseMessage(HttpStatusCode.Conflict); //Already voted

                db.VotiSondaggi.Add(voto);
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Route("risultati/{id}")]
        [HttpGet]
        public Dictionary<int,int> ReturnRisultati(int id)
        {
            //Check Auth
            var authorize = new Helpers.Utility();
            bool authorized = authorize.authorized(Request);
            if (!authorized)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

            //Create new dictionary
            var voti = new Dictionary<int, int>();
            
            try
            {
                //Find sondaggio
                var sondaggio = db.Sondaggi.Find(id);

                //Get voti vor sondaggio
                var votiSondaggio = sondaggio.VotiSondaggi.ToList();

                //Count each vote
                for (int i = votiSondaggio.Count() - 1; i >= 0; i--)
                {
                    int contoVoti = votiSondaggio.Where(x => x.Voto == votiSondaggio[i].Voto).Count();
                    voti.Add(votiSondaggio[i].Voto, contoVoti);
                    votiSondaggio.RemoveAll(x => x.Voto == votiSondaggio[i].Voto);
                }

                return voti;
               
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

       
    }
}