using EliteTrading.Entities;
using EliteTrading.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EliteTrading.Extensions {
    public static class ExtensionMethods {
        private const string SelectedAttribute = " selected='selected'";
        public static MvcHtmlString SelectedIfMatch(this HtmlHelper helper, string name, object expected, object actual) {
            return new MvcHtmlString(Equals(expected, actual) ? SelectedAttribute : string.Empty);
        }
    }
}