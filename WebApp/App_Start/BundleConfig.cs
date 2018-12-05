using System.Web;
using System.Web.Optimization;

namespace WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.cookie").Include(
                        "~/Scripts/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.flashmessage").Include(
                        "~/Scripts/jquery.flashmessage.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.timepicker").Include(
                        "~/Scripts/jquery.timepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
                        "~/Scripts/fullcalendar.js"));

            bundles.Add(new ScriptBundle("~/bundles/gcal").Include(
                        "~/Scripts/gcal.js"));

            bundles.Add(new ScriptBundle("~/bundles/locale-all").Include(
                        "~/Scripts/locale-all.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/Scripts/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment-with-locales").Include(
                        "~/Scripts/moment-with-locales.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery.timepicker.css",
                      "~/Content/fullcalendar.css"));
        }
    }
}
