using System.Web;
using System.Web.Optimization;

using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using BundleTransformer.Core.Transformers;

namespace UHub
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));


            bundles.Add(new ScriptBundle("~/resources/scripts/master").Include(
                      "~/ClientResources/Master/Scripts/Master.js"));



            var scssBundle = new CustomStyleBundle("~/resources/css/master").Include(
                "~/ClientResources/Master/Styles/Master.scss",
                "~/ClientResources/Master/Styles/Master_Header.scss",
                "~/ClientResources/Master/Styles/Master_Footer.scss");
            bundles.Add(scssBundle);


            BundleTable.EnableOptimizations = true;
        }
    }
}
