using System.Dynamic;
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
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js"));



            //---------------------------------MASTER------------------------------------------------------
            var masterBundle = new ScriptBundle("~/resources/scripts/master")
                .IncludeDirectory("~/ClientResources/Master/Scripts", "*.js", true);
            bundles.Add(masterBundle);




            //---------------------------------ACCOUNT------------------------------------------------------
            //LOGIN
            bundles.Add(
                new ScriptBundle("~/resources/account/loginScript")
                .IncludeDirectory("~/ClientResources/Master/Scripts", "*.js", true)
                .Include("~/ClientResources/Account/Scripts/Login.js")
                .Include("~/ClientResources/Security/Scripts/RecaptchaPartial.js")
                .Include("~/Scripts/jquery.validate*")
                );


            //CREATE
            bundles.Add(
                new ScriptBundle("~/resources/account/createScript")
                .IncludeDirectory("~/ClientResources/Master/Scripts", "*.js", true)
                .Include("~/ClientResources/Account/Scripts/CreateAccount.js")
                .Include("~/ClientResources/Security/Scripts/RecaptchaPartial.js")
                );
            bundles.Add(
                new CustomStyleBundle("~/resources/account/createStyle")
                .IncludeDirectory("~/ClientResources/Master/Styles", "*css", true)
                .Include("~/ClientResources/Account/Styles/CreateAccount.css")
                );



            bundles.Add(new CustomStyleBundle("~/resources/css/master")
                .IncludeDirectory("~/ClientResources/Master/Styles", "*css", true));



            BundleTable.EnableOptimizations = true;
        }
    }
}
