using System.Web;
using System.Web.Optimization;

namespace EliteTrading
{
    public class BundleConfig
    {

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/jquery.ui.touch-punch.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.tablesorter.min.js",
                        "~/Scripts/jquery.floatThead.min.js",
                        "~/Scripts/picnet.table.filter.min.js",
                        "~/Scripts/jsviews.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/numericInput.js",
                        "~/Scripts/accounting.js",
                        "~/Scripts/jquery.amaran.min.js",
                        "~/Scripts/typeahead.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/sitescripts").Include(
                        "~/Scripts/scripts.js",
                        "~/Scripts/admin.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.validate.change-messages.js",
                        "~/Scripts/bootstrap.validator.fix.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin").Include(
                        "~/Scripts/papaparse.min.js",
                        "~/Scripts/bootstrap.file-input.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery-ui*",
                      "~/Content/bootstrap.css",
                      "~/Content/amaran.min.css",
                      "~/content/site.css"));


           BundleTable.EnableOptimizations = true;
        }
    }
}
