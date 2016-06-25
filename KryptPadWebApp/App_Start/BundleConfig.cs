using System.Web;
using System.Web.Optimization;

namespace KryptPadWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/sammy-{version}.js",
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout-loaders.js",
                        "~/Scripts/md5.js",
                        "~/Scripts/functions.js"));
            
            // jQuery validate - not sure if it will be used
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Create a bundle for the ko widgets
            bundles.Add(new ScriptBundle("~/bundles/widgets").Include(
                        "~/Scripts/ko-widgets/*.js"));

            // Create bundle for the main app
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                        "~/Scripts/app/main-app.js"));

            // Sign in specific styles and scripts

            // Bundle for sign in stuff
            bundles.Add(new ScriptBundle("~/bundles/signin").Include(
                        "~/Scripts/app/sign-in.js"));

            bundles.Add(new StyleBundle("~/Content/css/signin").Include(
                      "~/Content/SignIn.css"));

            // Bundle for dashboard
            bundles.Add(new StyleBundle("~/Content/css/dashboard").Include(
                      "~/Content/Dashboard.css"));

        }
    }
}
