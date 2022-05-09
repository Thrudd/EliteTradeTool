using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Optimization;
using System.Collections.Generic;

using Newtonsoft.Json;

using EliteTrading.Extensions;

namespace EliteTrading
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private EliteTrading.Services.EDDNChecker EDDNChecker;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            GlobalConfiguration.Configuration.MessageHandlers.Add(new CancelledTaskBugWorkaroundMessageHandler());
            MvcHandler.DisableMvcResponseHeader = true;

            EDDNChecker = new EliteTrading.Services.EDDNChecker();
        }
    }
}
