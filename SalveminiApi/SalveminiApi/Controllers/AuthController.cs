using System;
using System.Collections.Generic;
using SalveminiApi.Argo.Models;
using SalveminiApi.Utility;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace UlysseApi.Controllers
{
    [RoutePrefix("api")]
    public class IstruttoriController : ApiController
    {
        [Route("login")]
        [HttpPost]
        public async Task<string> Auth(AuthBlock authBlock)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-app-code", "APF");
            client.DefaultRequestHeaders.Add("x-cod-min", "SS16836");
            client.DefaultRequestHeaders.Add("x-key-app", "ax6542sdru3217t4eesd9");
            client.DefaultRequestHeaders.Add("x-produttore-software", "ARGO Software s.r.l. - Ragusa");
            client.DefaultRequestHeaders.Add("x-pwd", authBlock.password);
            client.DefaultRequestHeaders.Add("x-user-id", authBlock.username);
            client.DefaultRequestHeaders.Add("x-version", "2.0.12");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Mobile Safari/537.36");
            var response = await client.GetAsync("https://www.portaleargo.it/famiglia/api/rest/login");
            var content = await response.Content.ReadAsStringAsync();
            var Token = JsonConvert.DeserializeObject<AuthResponse>(content).token;
            return "a";
          //  return new HttpResponseMessage(HttpStatusCode.OK);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
