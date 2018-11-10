using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UHub.Controllers
{
    public class PostController : Controller
    {
        public ActionResult Index()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }


        public ActionResult Edit()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }

    }
}