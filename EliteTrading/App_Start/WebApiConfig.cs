using EliteTrading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EliteTrading {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            // Attribute routing.
            config.MapHttpAttributeRoutes();


            // Convention-based routing.
            config.Routes.MapHttpRoute(
                name: "Api",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "EliteTradingTool" , id = RouteParameter.Optional }
            );
        }
    }
}