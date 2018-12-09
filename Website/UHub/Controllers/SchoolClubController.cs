﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Attributes;

namespace UHub.Controllers
{
    public class SchoolClubController : Controller
    {
        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult Index()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = int.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult About()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = int.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl(RequireAdmin = true)]
        public ActionResult ModEdit()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = int.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult CreatePost()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = int.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            return View();
        }
    }
}