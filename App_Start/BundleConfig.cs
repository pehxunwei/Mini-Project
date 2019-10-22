using System.Web;
using System.Web.Optimization;

namespace Mini_Project
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.min.js",
                        "~/Scripts/jquery-ui.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/Custom-main/custom-main.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/Custom-Style/custom-style.css",
                      "~/Content/Custom-Style-SCSS/common/extend.scss",
                      "~/Content/Custom-Style-SCSS/common/fonts.scss",
                      "~/Content/Custom-Style-SCSS/common/global.scss",
                      "~/Content/Custom-Style-SCSS/common/minxi.scss",
                      "~/Content/Custom-Style-SCSS/common/variables.scss",
                      "~/Content/Custom-Style-SCSS/layouts/main.scss",
                      "~/Content/Custom-Style-SCSS/layouts/responsive.scss",
                      "~/Content/Custom-Style-SCSS/style.scss"
                      ));
        }
    }
}
