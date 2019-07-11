using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SalveminiApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //My routes
            routes.Add(new Route("", new PageRouteHandler("~/Home.aspx")));
            routes.Add(new Route("AggiungiLibro", new PageRouteHandler("~/BookMarket/AddBooks.aspx")));
            routes.Add(new Route("BookMarketLogin", new PageRouteHandler("~/BookMarket/Login.aspx")));
        }
    }
}
