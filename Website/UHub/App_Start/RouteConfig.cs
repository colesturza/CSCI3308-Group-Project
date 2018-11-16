using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UHub
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(CoreLib.Common.API_ROUTE_PREFIX + "/*");

            routes.MapRoute(
                name: "ErrorHandler",
                url: "Error/{id}",
                defaults: new { controller = "Error", action = "Index", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "DefaultHomePage",
                url: "",
                defaults: new { controller = "Account", action = "Login"}
            );

            routes.MapRoute(
                name: "DefaultControllerAction",
                url: "{controller}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DefaultController",
                url: "{controller}/{action}/{id}",
                defaults: new {id = UrlParameter.Optional }
            );


        }
    }
}
