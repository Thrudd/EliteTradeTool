using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EliteTrading.Extensions {
    public class jQueryPartial : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            // Verify if a XMLHttpRequest is fired.  
            // This can be done by checking the X-Requested-With  
            // HTTP header.  
            AjaxAwareController myController = filterContext.Controller as AjaxAwareController;
            if (myController != null) {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] != null
                    && filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest") {
                    myController.IsAjaxRequest = true;
                } else {
                    myController.IsAjaxRequest = false;
                }
            }
        }
    }

    [jQueryPartial]
    public abstract class AjaxAwareController : Controller {
        public bool IsAjaxRequest { get; set; }
    }
}