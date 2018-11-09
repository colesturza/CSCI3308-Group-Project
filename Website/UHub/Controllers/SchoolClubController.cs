using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Attributes;

namespace UHub.Controllers
{
    public class SchoolClubController : Controller
    {
        [MvcAuthControl]
        public ActionResult Index()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }


        [MvcAuthControl]
        public ActionResult CreatePost()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }
    }
}