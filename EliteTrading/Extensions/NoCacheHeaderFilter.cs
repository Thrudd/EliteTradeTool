using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace EliteTrading.Extensions {
    public class NoCacheHeaderFilter : ActionFilterAttribute {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
            actionExecutedContext.Response.Content.Headers.Add("Cache-Control", "no-cache,no-store");
        }
    }
}