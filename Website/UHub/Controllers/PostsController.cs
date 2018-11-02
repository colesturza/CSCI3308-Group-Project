﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Attributes;

namespace UHub.Controllers
{
    public class PostsController : Controller
    {
        [MvcAuthControl]
        public ActionResult Index()
        {
            return View();
        }

        [MvcAuthControl]
        public ActionResult Create()
        {
            return View();
        }
    }
}